namespace GFramework
{
    internal class UIDefine
    {
        public static string UIRootPath = "UIRoot";

        //UI（位置）类型
        public enum UIType
        {
            //普通
            Normal,
            //固定                              
            Fixed,
            //弹出
            PopUp
        }

        //UI的显示类型
        public enum UIShowMode
        {
            //普通
            Normal,
            //反向切换
            ReverseChange,
            //隐藏其他
            HideOther
        }

        //UI透明度类型
        public enum UILucencyType
        {
            //完全透明，不能穿透
            Lucency,
            //半透明，不能穿透
            Translucence,
            //低透明度，不能穿透
            ImPenetrable,
            //可以穿透
            Pentrate
        }
    }
}