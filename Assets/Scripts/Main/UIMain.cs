using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GFramework;
using UnityEngine.UI;

public class UIMain : UIBase
{

    public Button ButtonMail;

    // Start is called before the first frame update
    void Start()
    {
        ButtonMail.onClick.AddListener(OnClickMail);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnClickMail()
    {
        UIMgr.Instance.OpenUI("UIMail");
    }
}
