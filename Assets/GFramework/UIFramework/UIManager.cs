using System.Collections.Generic;
using UnityEngine;

namespace GFramework
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager instance = null;
        
        private Dictionary<string, string> dictUIPath;  //UI路径
        private Dictionary<string, UIBase> dictUI;     //UI信息
        private Stack<UIBase> stackShow;
        private Dictionary<string, UIBase> dictShowUI; //显示的UI信息
        
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
            dictUI = new Dictionary<string, UIBase>();
            dictShowUI = new Dictionary<string, UIBase>();
            stackShow = new Stack<UIBase>();
            dictUIPath = new Dictionary<string, string>();

            GameObject uiRoot = InitRootCanvas();
            transRoot = uiRoot.transform;
            DontDestroyOnLoad(transRoot);

            transform.SetParent(transRoot);
            transNormal = transRoot.Find("Normal");
            transFixed = transRoot.Find("Fixed");
            transPopUp = transRoot.Find("PopUp");

            GetUIPathInfoFromJson();
            //dictUIPath.Add("TestPanel", "Prefab/TestPanel");
        }


        /// <summary>
        /// 显示UI
        /// </summary>
        /// <param name="uiName">UI名称</param>
        public void OpenUI(string uiName)
        {
            if (string.IsNullOrEmpty(uiName)) { return; }

            UIBase curUI = null;
            curUI = GetUI(uiName);

            if (curUI == null) { return; }
            
            switch (curUI.showMode)
            {
                case UIShowMode.Normal:
                    dictShowUI.Add(uiName, curUI);
                    curUI.Show();
                    break;
                case UIShowMode.Overlying:
                    if(stackShow.Count>0)
                    {
                        UIBase topUI = stackShow.Peek();
                        topUI.Freeze();
                    }
                    stackShow.Push(curUI);
                    curUI.Show();
                    break;
                case UIShowMode.HideOther:
                    foreach (var ui in dictShowUI.Values)
                    {
                        ui.Hide();
                    }
                    foreach(var ui in stackShow)
                    {
                        ui.Hide();
                    }
                    dictShowUI.Add(uiName, curUI);
                    curUI.Show();
                    break;
                default:
                    break;
            }
        }
        
        //实例化UIRoot预制体并返回
        private GameObject InitRootCanvas()
        {
            return ResourcesMgr.GetInstance().LoadAsset(UIDefine.UIRootPath, false);
        }

        /// <summary>
        /// 优先从缓存中获取，获取失败再加载
        /// </summary>
        /// <param name="uiName">UI名称</param>
        /// <returns></returns>
        private UIBase GetUI(string uiName)
        {
            UIBase res = null;
            if (dictUI.TryGetValue(uiName, out res))
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
            
            dictUIPath.TryGetValue(uiName, out uiPath);
            if (!string.IsNullOrEmpty(uiPath))
            {
                uiObject = ResourcesMgr.GetInstance().LoadAsset(uiPath, false);
            }

            if (transRoot != null && uiObject != null)
            {
                uiBase = uiObject.GetComponent<UIBase>();
                if (uiBase == null)
                {
                    Debug.Log("预制体上未绑定UIBase脚本，UI名称:" + uiName);
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

                //设置隐藏
                uiObject.SetActive(false);
                //把克隆体，加入到“所有UI”（缓存）集合中。
                dictUI.Add(uiName, uiBase);
                return uiBase;
            }
            else
            {
                Debug.Log("_TraCanvasTransfrom == null Or goCloneUIPrefabs==null!! ,Plese Check!, 参数uiFormName=" + uiName);
            }

            Debug.Log("出现不可以预估的错误，请检查，参数 uiFormName=" + uiName);
            return null;
        }

        private void GetUIPathInfoFromJson()
        {
            TextAsset textAsset = Resources.Load<TextAsset>("Json/UIPanelPath");
            Debug.Log(textAsset.text);
            UIPathInfos uiPathInfo = JsonUtility.FromJson<UIPathInfos>(textAsset.text);
            foreach (var pathInfo in uiPathInfo.UIPathList)
            {
                dictUIPath.Add(pathInfo.UIName, pathInfo.UIPath);
            }
        }
    }
}