using UnityEngine;
using UnityEngine.UI;
using GFramework;

public class UIEntry : UIBase
{
    void Start()
    {
        GetWidget<Button>("ButtonEnter").onClick.AddListener(OnClickEnter);
        GetWidget<Button>("ButtonExit").onClick.AddListener(OnClickExit);
    }

    private void OnClickEnter()
    {
        UIMgr.Instance.OpenUI("UIMain");
    }

    private void OnClickExit()
    {
        Application.Quit();
    }
}
