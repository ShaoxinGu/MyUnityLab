using UnityEngine;
using GFramework;

public class GameLauncher : MonoBehaviour
{
    void Awake()
    {
        //RedDotManager.Instance.Initilize();
    }

    void Start()
    {
        //UIMgr.Instance.OpenUI("UIMain");
        BagMgr.Instance.InitItemInfo();
        UIManager.Instance.ShowPanel<BagPanel>("Bag/BagPanel");

    }
}