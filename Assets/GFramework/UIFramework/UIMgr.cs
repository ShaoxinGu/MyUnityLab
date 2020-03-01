using System.Collections.Generic;
using UnityEngine;

namespace GFramework
{
    public class UIMgr : MonoBehaviour
    {
        private static UIMgr instance = null;
        
        private Dictionary<string, string> dictPath;    //UI路径
        private Dictionary<string, UIBase> dictUI;      //所有UI的索引表
        private Dictionary<string, UIBase> dictShowing; //显示界面的索引表
        private Stack<UIBase> stackShowing;             //显示弹窗的索引表

        private Transform transScript = null;           //脚本挂载节点
        private Transform transRoot = null;             //UI根节点
        private Transform transNormal = null;           //Normal节点
        private Transform transFixed = null;            //Fixed节点
        private Transform transPopUp = null;            //PopUp节点

        /// <summary>
        /// 获取实例
        /// </summary>
        /// <returns>实例</returns>
        public static UIMgr GetInstance()
        {
            if (instance == null)
            {
                instance = new GameObject("UIManager").AddComponent<UIMgr>();
            }
            return instance;
        }
        
        void Awake()
        {
            dictUI = new Dictionary<string, UIBase>();
            dictShowing = new Dictionary<string, UIBase>();
            stackShowing = new Stack<UIBase>();
            dictPath = new Dictionary<string, string>();

            GameObject uiRoot = InitRootCanvas();
            transRoot = uiRoot.transform;
            DontDestroyOnLoad(transRoot);

            transScript = transRoot.Find("ScriptHolder");
            transNormal = transRoot.Find("Normal");
            transFixed = transRoot.Find("Fixed");
            transPopUp = transRoot.Find("Popup");
            transform.SetParent(transScript);

            GetUIPathInfoFromJson();
            //dictUIPath.Add("TestPanel", "Prefab/TestPanel");
        }

        //实例化UIRoot预制体并返回
        private GameObject InitRootCanvas()
        {
            return ResMgr.GetInstance().LoadAsset(UIDefine.UIRootPath, false);
        }

        public Transform GetUIRoot()
        {
            return transRoot;
        }

        /// <summary>
        /// 显示UI
        /// </summary>
        /// <param name="uiName">UI名称</param>
        public void OpenUI(string uiName)
        {
            if (string.IsNullOrEmpty(uiName)) { return; }

            UIBase curUI = GetUI(uiName);
            if (curUI == null) { return; }
            
            switch (curUI.showMode)
            {
                case UIShowMode.Normal:
                    LoadUiToCurrentCache(uiName);
                    break;
                case UIShowMode.Overlying:
                    PushUiFormToStack(uiName);
                    break;
                case UIShowMode.HideOther:
                    HideOtherAndShow(uiName);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 优先从缓存中获取，获取失败再加载
        /// </summary>
        /// <param name="uiName">UI名称</param>
        /// <returns></returns>
        private UIBase GetUI(string uiName)
        {
            if (dictUI.TryGetValue(uiName, out UIBase res))
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
            GameObject uiObject = null; //创建的UI对象

            dictPath.TryGetValue(uiName, out string uiPath);
            if (!string.IsNullOrEmpty(uiPath))
            {
                uiObject = ResMgr.GetInstance().LoadAsset(uiPath, false);
            }

            if (transRoot != null && uiObject != null)
            {
                UIBase uiBase = uiObject.GetComponent<UIBase>();
                if (uiBase == null)
                {
                    Debug.Log("预制体上未绑定UIBase脚本，uiName : " + uiName);
                    return null;
                }
                switch (uiBase.uiType)
                {
                    case UIType.Normal: //普通节点
                        uiObject.transform.SetParent(transNormal, false);
                        break;
                    case UIType.Fixed: //固定节点
                        uiObject.transform.SetParent(transFixed, false);
                        break;
                    case UIType.PopUp: //弹出节点
                        uiObject.transform.SetParent(transPopUp, false);
                        break;
                    default:
                        break;
                }
                uiObject.SetActive(false);
                dictUI.Add(uiName, uiBase);
                return uiBase;
            }
            else
            {
                Debug.Log("Create UI Failed, Please Check! uiName : " + uiName);
            }
            return null;
        }

        /// <summary>
        /// 把当前窗体加载到“当前窗体”集合中
        /// </summary>
        /// <param name="uiFormName">窗体预设的名称</param>
        private void LoadUiToCurrentCache(string uiFormName)
        {
            dictShowing.TryGetValue(uiFormName, out UIBase showUI);
            if (showUI != null) return;
            dictUI.TryGetValue(uiFormName, out var uiForm);
            if (uiForm != null)
            {
                dictShowing.Add(uiFormName, uiForm);
                uiForm.Show();
            }
        }

        /// <summary>
        /// UI窗体入栈
        /// </summary>
        /// <param name="uiFormName">窗体的名称</param>
        private void PushUiFormToStack(string uiFormName)
        {
            if (stackShowing.Count > 0)
            {
                UIBase topUI = stackShowing.Peek();
                topUI.Freeze();
            }
            dictUI.TryGetValue(uiFormName, out UIBase curUI);
            if (curUI != null)
            {
                stackShowing.Push(curUI);
                curUI.Show();
            }
        }

        /// <summary>
        /// 打开窗体，且隐藏其他窗体
        /// </summary>
        /// <param name="uiName">打开的指定窗体名称</param>
        private void HideOtherAndShow(string uiName)
        {
            dictShowing.TryGetValue(uiName, out UIBase showUI);
            if (showUI != null) return;

            foreach (var ui in dictShowing.Values)
            {
                ui.Hide();
            }
            foreach (var ui in stackShowing)
            {
                ui.Hide();
            }
            dictUI.TryGetValue(uiName, out UIBase curUI);
            if (curUI != null)
            {
                dictShowing.Add(uiName, curUI);
                curUI.Show();
            }
        }

        /// <summary>
        /// 关闭指定UI窗体
        /// </summary>
        /// <param name="uiName"></param>
        private void CloseUI(string uiName)
        {
            dictShowing.TryGetValue(uiName, out UIBase showUI);
            if (showUI == null) return;

            showUI.Hide();
            dictShowing.Remove(uiName);
        }

        /// <summary>
        /// 弹出窗体的出栈逻辑
        /// </summary>
        private void PopUI()
        {
            if (stackShowing.Count >= 2)
            {
                UIBase topUI = stackShowing.Pop();
                topUI.Hide();
                UIBase nextUI = stackShowing.Peek();
                nextUI.Resume();
            }
            else if (stackShowing.Count == 1)
            {
                UIBase topUI = stackShowing.Pop();
                topUI.Hide();
            }
        }

        /// <summary>
        /// 关闭窗体，且显示其他窗体
        /// </summary>
        /// <param name="uiName">窗体名称</param>
        private void CloseUIAndShowOther(string uiName)
        {
            dictShowing.TryGetValue(uiName, out var showUI);
            if (showUI == null) return;

            showUI.Hide();
            dictShowing.Remove(uiName);

            foreach (UIBase baseUi in dictShowing.Values)
            {
                baseUi.Show();
            }
            foreach (UIBase popupUi in stackShowing)
            {
                popupUi.Show();
            }
        }

        private void GetUIPathInfoFromJson()
        {
            TextAsset textAsset = Resources.Load<TextAsset>("UI/Json/UIPanelPath");
            UIPathInfos uiPathInfo = JsonUtility.FromJson<UIPathInfos>(textAsset.text);
            foreach (var pathInfo in uiPathInfo.UIPathList)
            {
                dictPath.Add(pathInfo.UIName, pathInfo.UIPath);
            }
        }
    }
}