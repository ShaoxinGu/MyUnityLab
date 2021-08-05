using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GFramework;

public class BagItem : BasePanel
{
    public void InitItemInfo(Item info)
    {
        GetWidget<Text>("TextNumber").text = info.num.ToString();
    }
}
