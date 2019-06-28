using System.Collections;
using static GFramework.UIDefine;

namespace GFramework
{
    internal class UIType
    {
        //是否需要清空“反向切换”
        public bool isClearReverseChange = false;
        //UI类型
        public UIDefine.UIType uiType = UIDefine.UIType.Normal;
        //UI显示类型
        public UIShowMode showMode = UIShowMode.Normal;
        //UI透明度类型
        public UILucencyType lucencyType = UILucencyType.Lucency;
    }
}