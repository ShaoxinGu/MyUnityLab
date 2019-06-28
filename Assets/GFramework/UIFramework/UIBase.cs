using UnityEngine;

namespace GFramework
{
    public class UIBase : MonoBehaviour
    {
        internal UIType CurrentUIType { set; get; } = new UIType();
        
        public virtual void Show()
        {
            gameObject.SetActive(true);
        }
        
        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}