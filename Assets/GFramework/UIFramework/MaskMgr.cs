using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GFramework
{
    public class MaskMgr : MonoBehaviour
    {
        private static MaskMgr instance = null;   //本脚本私有单例
        private Transform transUIRoot = null;       //UI根节点对象
        private GameObject goMask;                  //遮罩面板
        private Camera uiCamera;                    //UI摄像机
        private float uiCameralDepth;               //UI摄像机原始的“层深”

        //得到实例
        public static MaskMgr Instance()
        {
            if (instance == null)
            {
                instance = new GameObject("MaskManager").AddComponent<MaskMgr>();
            }
            return instance;
        }

        void Awake()
        {
            transUIRoot = UIMgr.GetInstance().GetUIRoot();
            gameObject.transform.SetParent(transUIRoot.Find("ScriptHolder"));
            goMask = transUIRoot.Find("Popup/UIMaskPanel").gameObject;
            uiCamera = transUIRoot.Find("UICamera").GetComponent<Camera>();
            if (uiCamera != null)
            {
                uiCameralDepth = uiCamera.depth;
            }
            else
            {
                Debug.Log("UI_Camera is Null!, Please Check!");
            }
        }

        /// <summary>
        /// 设置遮罩状态
        /// </summary>
        /// <param name="goDisplayUIForms">需要显示的UI窗体</param>
        /// <param name="lucenyType">显示透明度属性</param>
        public void SetMaskWindow(GameObject goDisplayUIForms, UiTransparencyType lucenyType = UiTransparencyType.Transparent)
        {
            //顶层窗体下移
            transUIRoot.transform.SetAsLastSibling();
            //启用遮罩窗体以及设置透明度
            switch (lucenyType)
            {
                //完全透明，不能穿透
                case UiTransparencyType.Transparent:
                    goMask.SetActive(true);
                    Color newColor1 = new Color(255 / 255F, 220 / 255F, 220 / 255F, 50F / 255F);
                    goMask.GetComponent<Image>().color = newColor1;
                    break;
                //半透明，不能穿透
                case UiTransparencyType.Translucence:
                    goMask.SetActive(true);
                    Color newColor2 = new Color(220 / 255F, 220 / 255F, 220 / 255F, 50F / 255F);
                    goMask.GetComponent<Image>().color = newColor2;
                    break;
                //低透明，不能穿透
                case UiTransparencyType.ImPenetrable:
                    goMask.SetActive(true);
                    Color newColor3 = new Color(50 / 255F, 50 / 255F, 50 / 255F, 200F / 255F);
                    goMask.GetComponent<Image>().color = newColor3;
                    break;
                //可以穿透
                case UiTransparencyType.Penetrable:
                    if (goMask.activeInHierarchy)
                    {
                        goMask.SetActive(false);
                    }
                    break;
                default:
                    break;
            }

            //遮罩窗体下移
            goMask.transform.SetAsLastSibling();
            //显示窗体的下移
            goDisplayUIForms.transform.SetAsLastSibling();
            //增加当前UI摄像机的层深（保证当前摄像机为最前显示）
            if (uiCamera != null)
            {
                uiCamera.depth += 100;    //增加层深
            }

        }

        /// <summary>
        /// 取消遮罩状态
        /// </summary>
        public void CancelMaskWindow()
        {
            //顶层窗体上移
            transUIRoot.transform.SetAsFirstSibling();
            //禁用遮罩窗体
            if (goMask.activeInHierarchy)
            {
                //隐藏
                goMask.SetActive(false);
            }

            //恢复当前UI摄像机的层深 
            if (uiCamera != null)
            {
                uiCamera.depth = uiCameralDepth;  //恢复层深
            }
        }
    }
}