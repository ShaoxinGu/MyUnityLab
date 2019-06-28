using System.Collections.Generic;
using UnityEngine;
using static GFramework.UIDefine;

namespace GFramework
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager instance = null;
        
        private Dictionary<string, string> dictUIPaths;     //UI路径
        private Dictionary<string, UIBase> dictUIBases;     //UI信息
        private Dictionary<string, UIBase> dictShowUIBases; //显示的UI信息
        
        private Transform transRoot = null;     //UI根节点
        private Transform transNormal = null;   //Normal节点
        private Transform transFixed = null;    //Fixed节点
        private Transform transPopUp = null;    //PopUp节点

        /// <summary>
        /// 获取实例
        /// </summary>
        /// <returns>实例</returns>
        public static UIManager GetInstance()
        {
            if (instance == null)
            {
                instance = new GameObject("UIManager").AddComponent<UIManager>();
            }
            return instance;
        }
        
        public void Awake()
        {
            dictUIBases = new Dictionary<string, UIBase>();
            dictShowUIBases = new Dictionary<string, UIBase>();
            dictUIPaths = new Dictionary<string, string>();

            GameObject uiRoot = InitRootCanvasLoading();
            transRoot = uiRoot.transform;
            DontDestroyOnLoad(transRoot);

            transNormal = transRoot.Find("Normal");
            transFixed = transRoot.Find("Fixed");
            transPopUp = transRoot.Find("PopUp");

            dictUIPaths.Add("TestPanel", "Prefab/TestPanel");
        }

        /// <summary>
        /// 显示UI
        /// </summary>
        /// <param name="uiName">UI名称</param>
        public void ShowUI(string uiName)
        {
            if (string.IsNullOrEmpty(uiName))
            {
                return;
            }

            UIBase uiBase = null;
            uiBase = GetUI(uiName);
            if (uiBase == null)
            {
                return;
            }

            //根据不同的UI的显示模式，分别作不同的加载处理
            switch (uiBase.CurrentUIType.showMode)
            {
                case UIShowMode.Normal:
                    AddToShowCache(uiName);
                    break;
                case UIShowMode.ReverseChange:

                    break;
                case UIShowMode.HideOther:

                    break;
                default:
                    break;
            }
        }

        #region 私有方法
        //实例化UIRoot预制体并返回
        private GameObject InitRootCanvasLoading()
        {
            return ResourcesMgr.GetInstance().LoadAsset(UIRootPath, false);
        }

        /// <summary>
        /// 优先从缓存中获取，获取失败再加载
        /// </summary>
        /// <param name="uiName">UI名称</param>
        /// <returns></returns>
        private UIBase GetUI(string uiName)
        {
            UIBase res = null;
            if (dictUIBases.TryGetValue(uiName, out res))
            {
                return res;
            }
            return LoadUIForm(uiName);
        }

        /// <summary>
        /// 加载指定名称的UI
        /// </summary>
        /// <param name="uiName">UI名称</param>
        private UIBase LoadUIForm(string uiName)
        {
            string uiPath = null;       //UI路径
            GameObject uiObject = null; //创建的UI对象
            UIBase uiBase = null;       //基类
            
            dictUIPaths.TryGetValue(uiName, out uiPath);
            if (!string.IsNullOrEmpty(uiPath))
            {
                uiObject = ResourcesMgr.GetInstance().LoadAsset(uiPath, false);
            }

            if (transRoot != null && uiObject != null)
            {
                uiBase = uiObject.GetComponent<UIBase>();
                if (uiBase == null)
                {
                    Debug.Log("baseUiForm == null! ,请先确认预设对象上是否加载了baseUIForm的子类脚本！ 参数 uiFormName=" + uiName);
                    return null;
                }
                switch (uiBase.CurrentUIType.uiType)
                {
                    case UIDefine.UIType.Normal: //普通节点
                        uiObject.transform.SetParent(transNormal, false);
                        break;
                    case UIDefine.UIType.Fixed: //固定节点
                        uiObject.transform.SetParent(transFixed, false);
                        break;
                    case UIDefine.UIType.PopUp: //弹出节点
                        uiObject.transform.SetParent(transPopUp, false);
                        break;
                    default:
                        break;
                }

                //设置隐藏
                uiObject.SetActive(false);
                //把克隆体，加入到“所有UI”（缓存）集合中。
                dictUIBases.Add(uiName, uiBase);
                return uiBase;
            }
            else
            {
                Debug.Log("_TraCanvasTransfrom == null Or goCloneUIPrefabs==null!! ,Plese Check!, 参数uiFormName=" + uiName);
            }

            Debug.Log("出现不可以预估的错误，请检查，参数 uiFormName=" + uiName);
            return null;
        }//Mehtod_end

        /// <summary>
        /// 把UI加入显示缓存
        /// </summary>
        /// <param name="uiName">预设的名称</param>
        private void AddToShowCache(string uiName)
        {
            UIBase uiBase;
            
            dictShowUIBases.TryGetValue(uiName, out uiBase);
            if (uiBase != null)
            {
                return;
            }

            dictUIBases.TryGetValue(uiName, out uiBase);
            if (uiBase != null)
            {
                dictShowUIBases.Add(uiName, uiBase);
                uiBase.Show();
            }
        }
        #endregion
    }
}