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
        BagMgr.Instance.InitItemInfo();
        UIMgr.Instance.OpenUI("UIMain");
        //UIMgr.Instance.OpenUI("UIBag");
    }
}