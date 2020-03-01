using UnityEngine;

namespace GFramework
{
    public class UIBase : MonoBehaviour
    {
        public string uiName;
        public bool isClearPopup = false;                           //是否清除弹窗
        public UIType uiType = UIType.Normal;                       //UI类型
        public UIShowMode showMode = UIShowMode.Normal;             //UI显示类型
        public UiTransparencyType transparencyType = UiTransparencyType.Transparent;   //UI透明度类型
        
        /// <summary>
        /// 显示
        /// </summary>
        public virtual void Show()
        {
            gameObject.SetActive(true);
            if (uiType == UIType.PopUp)
            {
                MaskMgr.Instance().SetMaskWindow(gameObject, transparencyType);
            }
        }

        /// <summary>
        /// 隐藏
        /// </summary>
        public virtual void Hide()
        {
            gameObject.SetActive(false);
            if (uiType == UIType.PopUp)
            {
                MaskMgr.Instance().CancelMaskWindow();
            }
        }

        /// <summary>
        /// 被盖住的弹窗冻结
        /// </summary>
        public virtual void Freeze()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 被盖住的弹窗恢复
        /// </summary>
        public virtual void Resume()
        {
            gameObject.SetActive(true);
            if (uiType == UIType.PopUp)
            {
                MaskMgr.Instance().SetMaskWindow(gameObject, transparencyType);
            }
        }
    }
}