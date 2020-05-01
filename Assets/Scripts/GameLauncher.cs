using UnityEngine;
using GFramework;

public class GameLauncher : MonoBehaviour
{
    void Awake()
    {
        RedDotManager.Instance.Initilize();
    }

    void Start()
    {
        //this.gameObject.GetComponents<UnityEngine.UI.Text>("txtTest");
        BagMgr.Instance.InitItemInfo();
        UIMgr.Instance.OpenUI("UIMain");
    }
}