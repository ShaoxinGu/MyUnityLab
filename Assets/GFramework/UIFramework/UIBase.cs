using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

namespace GFramework
{
    public class UIBase : MonoBehaviour
    {
        public string uiName;
        public bool isClearPopup = false;                                               //是否清除弹窗
        public UIType uiType = UIType.Normal;                                           //UI类型
        public UIShowMode showMode = UIShowMode.Normal;                                 //UI显示类型
        public UiTransparencyType transparencyType = UiTransparencyType.Transparent;   //UI透明度类型

        private Dictionary<string, List<UIBehaviour>> widgetDic = new Dictionary<string, List<UIBehaviour>>();

        void Awake()
        {
            FindChidrenWidget<Button>();
            FindChidrenWidget<Image>();
            FindChidrenWidget<Text>();
            FindChidrenWidget<Toggle>();
            FindChidrenWidget<Slider>();
            FindChidrenWidget<ScrollRect>();
            FindChidrenWidget<InputField>();
        }

        /// <summary>
        /// 显示
        /// </summary>
        public virtual void OnEnter()
        {
            gameObject.SetActive(true);
            if (uiType == UIType.PopUp)
            {
                //TODO 设置遮罩
            }
        }

        /// <summary>
        /// 隐藏
        /// </summary>
        public virtual void OnExit()
        {
            gameObject.SetActive(false);
            if (uiType == UIType.PopUp)
            {
                //清除遮罩
            }
        }

        /// <summary>
        /// 被盖住的弹窗冻结
        /// </summary>
        public virtual void OnPause()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 被盖住的弹窗恢复
        /// </summary>
        public virtual void OnResume()
        {
            gameObject.SetActive(true);
            if (uiType == UIType.PopUp)
            {
                //TODO 设置遮罩
            }
        }

        protected T GetWidget<T>(string widgetName) where T : UIBehaviour
        {
            if (widgetDic.ContainsKey(widgetName))
            {
                foreach (T widget in widgetDic[widgetName])
                {
                    if (widget is T)
                        return widget;
                }
            }
            return null;
        }

        private void FindChidrenWidget<T>() where T : UIBehaviour
        {
            T[] widgets = this.GetComponentsInChildren<T>();
            string objName;
            foreach (T widget in widgets)
            {
                objName = widget.gameObject.name;
                if (widgetDic.ContainsKey(objName))
                    widgetDic[objName].Add(widget);
                else
                    widgetDic.Add(objName, new List<UIBehaviour>() { widget });
            }
        }
    }
}