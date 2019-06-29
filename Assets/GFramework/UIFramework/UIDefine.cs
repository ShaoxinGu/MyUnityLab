namespace GFramework
{
    public class UIDefine
    {
        public static string UIRootPath = "UIRoot";
    }

    //UI类型
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
        Normal,     //普通
        Overlying,  //层叠弹出
        HideOther   //独占
    }

    //UI的透明度类型
    public enum UILucencyType
    {
        Lucency,//完全透明，不能穿透
        Translucence,//半透明，不能穿透
        ImPenetrable,//低透明度，不能穿透
        Pentrate//可以穿透
    }
}