using System.Collections.Generic;
using UnityEngine;

namespace GFramework
{
    public class UIMgr : MonoBehaviour
    {
        private static UIMgr _instance = null;
        public static UIMgr Instance()
        {
            if (_instance == null)
            {
                _instance = new GameObject("UIManager").AddComponent<UIMgr>();
            }
            return _instance;
        }

        private Dictionary<string, string> dictPath;    //UI路径
        private Dictionary<string, UIBase> dictAllUI;   //所有UI的索引表
        private Dictionary<string, UIBase> dictShowing; //显示界面的索引表
        private Stack<UIBase> stackShowing;             //显示弹窗的索引表

        private Transform transScript = null;           //脚本挂载节点
        private Transform transRoot = null;             //UI根节点
        private Transform transNormal = null;           //Normal节点
        private Transform transFixed = null;            //Fixed节点
        private Transform transPopUp = null;            //PopUp节点

        private void Awake()
        {
            dictAllUI = new Dictionary<string, UIBase>();
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

        #region Public
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
                    OpenUIForm(uiName);
                    break;
                case UIShowMode.Overlying:
                    PushUIFormToStack(uiName);
                    break;
                case UIShowMode.HideOther:
                    HideOtherAndShow(uiName);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 关闭指定UI窗体
        /// </summary>
        /// <param name="uiName"></param>
        public void CloseUI(string uiName)
        {
            #region assertion
            if (string.IsNullOrEmpty(uiName))
            {
                Debug.LogError("uiName cannot be null or empty!");
                return;
            }
            if (!dictAllUI.ContainsKey(uiName))
            {
                Debug.LogError("UI doesn't exist! uiName = " + uiName);
                return;
            }
            #endregion

            UIBase curUI = GetUI(uiName);
            if (curUI == null) { return; }
            switch (curUI.showMode)
            {
                case UIShowMode.Normal:
                    CloseUIForm(uiName);
                    break;
                case UIShowMode.Overlying:
                    PopUI();
                    break;
                case UIShowMode.HideOther:
                    CloseUIAndShowOther(uiName);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Private
        //实例化UIRoot预制体并返回
        private GameObject InitRootCanvas()
        {
            return ResMgr.Instance().LoadAsset(UIDefine.UIRootPath, false);
        }

        /// <summary>
        /// 优先从缓存中获取，获取失败再加载
        /// </summary>
        /// <param name="uiName">UI名称</param>
        /// <returns></returns>
        private UIBase GetUI(string uiName)
        {
            if (dictAllUI.TryGetValue(uiName, out UIBase res))
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
                uiObject = ResMgr.Instance().LoadAsset(uiPath, false);
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
                dictAllUI.Add(uiName, uiBase);
                return uiBase;
            }
            else
            {
                Debug.LogError("Create UI Failed, Please Check! uiName : " + uiName);
            }
            return null;
        }

        /// <summary>
        /// 打开界面
        /// </summary>
        /// <param name="uiFormName">界面名称</param>
        private void OpenUIForm(string uiFormName)
        {
            dictShowing.TryGetValue(uiFormName, out UIBase showUI);
            if (showUI != null) return;
            dictAllUI.TryGetValue(uiFormName, out var uiForm);
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
        private void PushUIFormToStack(string uiFormName)
        {
            if (stackShowing.Count > 0)
            {
                UIBase topUI = stackShowing.Peek();
                topUI.Freeze();
            }
            dictAllUI.TryGetValue(uiFormName, out UIBase curUI);
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
            dictAllUI.TryGetValue(uiName, out UIBase curUI);
            if (curUI != null)
            {
                dictShowing.Add(uiName, curUI);
                curUI.Show();
            }
        }

        /// <summary>
        /// 关闭界面
        /// </summary>
        /// <param name="uiFormName"></param>
        private void CloseUIForm(string uiFormName)
        {
            dictShowing.TryGetValue(uiFormName, out UIBase showUI);
            if (showUI == null) return;

            showUI.Hide();
            dictShowing.Remove(uiFormName);
        }

        /// <summary>
        /// 弹出窗体的出栈逻辑
        /// <param name="uiName"></param>
        /// </summary>
        private void PopUI()
        {
            UIBase topUI = stackShowing.Peek();
            if (topUI != null)
            {
                stackShowing.Pop();
                topUI.Hide();
                if (stackShowing.Count >= 1)
                {
                    UIBase nextUI = stackShowing.Peek();
                    nextUI.Resume();
                }
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

            foreach (UIBase baseUI in dictShowing.Values)
            {
                baseUI.Show();
            }
            foreach (UIBase popupUI in stackShowing)
            {
                popupUI.Show();
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
        #endregion
    }
}