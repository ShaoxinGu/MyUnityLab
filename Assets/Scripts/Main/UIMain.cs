using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GFramework;
using UnityEngine.UI;

public class UIMain : UIBase
{
    void Start()
    {
        GetWidget<Button>("ButtonMail").onClick.AddListener(OnClickMail);
        GetWidget<Button>("ButtonBag").onClick.AddListener(OnClickBag);
    }

    private void OnClickMail()
    {
        UIMgr.Instance.OpenUI("UIMail");
    }

    private void OnClickBag()
    {
        UIMgr.Instance.OpenUI("UIBag");
    }
}
