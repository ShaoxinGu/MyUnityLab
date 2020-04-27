using System;
using System.Collections.Generic;

namespace GFramework
{
    public class UIDefine
    {
        public static string UIRootPath = "UI/Common/UIRoot";
    }

    [Serializable]
    class UIPathInfos
    {
        public List<UIPathInfo> UIPathList = new List<UIPathInfo>();
    }

    [Serializable]
    public class UIPathInfo
    {
        public string UIName;
        public string UIPath;
    }

    //UI类型
    public enum UIType
    {
        Normal, //普通                
        Fixed,  //固定
        PopUp   //弹出
    }

    //UI的显示类型
    public enum UIShowMode
    {
        Normal,     //普通
        Overlying,  //层叠
        HideOther   //独占
    }

    //UI的透明度类型
    public enum UiTransparencyType
    {
        Transparent,    //完全透明，不能穿透
        Translucence,   //半透明，不能穿透
        ImPenetrable,   //低透明度，不能穿透
        Penetrable,     //可以穿透
    }
}