using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GFramework
{
    public class BasePanel : MonoBehaviour
    {
        private Dictionary<string, List<UIBehaviour>> widgetDic = new Dictionary<string, List<UIBehaviour>>();

        void Start()
        {
            FindChidrenWidget<Button>();
            FindChidrenWidget<Image>();
            FindChidrenWidget<Text>();
            FindChidrenWidget<Toggle>();
            FindChidrenWidget<Slider>();
            FindChidrenWidget<ScrollRect>();
        }

        public virtual void Show()
        {

        }


        public virtual void Hide()
        {

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
