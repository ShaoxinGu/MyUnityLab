using UnityEngine;

namespace GFramework
{
    public class UIBase : MonoBehaviour
    {
        public bool isClearReverseChange = false;                   //是否需要清空“反向切换”
        public UIType uiType = UIType.Normal;                       //UI类型
        public UIShowMode showMode = UIShowMode.Normal;             //UI显示类型
        public UILucencyType lucencyType = UILucencyType.Lucency;   //UI透明度类型
        
        public virtual void Show()
        {
            gameObject.SetActive(true);
        }
        
        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        public virtual void Freeze()
        {
            gameObject.SetActive(false);
        }
    }
}