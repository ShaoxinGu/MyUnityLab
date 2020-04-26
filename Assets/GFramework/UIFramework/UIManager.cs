using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GFramework
{
    public enum UI_LAYER
    {
        Bottom,
        Middle,
        Top,
        System,
    }

    public class UIManager : Singleton<UIManager>
    {
        private Dictionary<string, BasePanel> panelDic = new Dictionary<string, BasePanel>();
        private Transform canvas;
        private Transform bottom;
        private Transform midlle;
        private Transform top;
        private Transform system;


        public UIManager()
        {
            GameObject obj1 = ResMgr.Instance.Load<GameObject>("UI/Canvas");
            GameObject.DontDestroyOnLoad(obj1);
            GameObject obj2 = ResMgr.Instance.Load<GameObject>("UI/EventSystem");
            GameObject.DontDestroyOnLoad(obj2);
            bottom = canvas.Find("Bottom");
            midlle = canvas.Find("Middle");
            top = canvas.Find("Top");
            system = canvas.Find("System");
        }

        public void ShowPanel(string name, UI_LAYER layer)
        {
            ResMgr.Instance.LoadAsyn<GameObject>("UI/" + name, (obj)=>
            {
                Transform father = null;
                switch(layer)
                {
                    case UI_LAYER.Bottom:
                        father = bottom;
                        break;
                    case UI_LAYER.Middle:
                        father = midlle;
                        break;
                    case UI_LAYER.Top:
                        father = top;
                        break;
                    case UI_LAYER.System:
                        father = system;
                        break;
                }
                obj.transform.SetParent(father);

                obj.transform.localPosition = Vector3.zero;
                obj.transform.localScale = Vector3.one;
                (obj.transform as RectTransform).offsetMax = Vector3.zero;
                (obj.transform as RectTransform).offsetMin = Vector3.zero;
            });
        }

        public void HidePanel(string name)
        {

        }
    }
}
