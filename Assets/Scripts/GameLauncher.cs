using UnityEngine;
using GFramework;
using XLua;

public class GameLauncher : MonoBehaviour
{
    private LuaEnv env;
    void Awake()
    {
        RedDotManager.Instance.Initilize();
    }

    void Start()
    {
        BagMgr.Instance.InitItemInfo();
        UIMgr.Instance.OpenUI("UIEntry");

        //test ABMgr
        ABMgr.Instance.LoadResAsync<GameObject>("model", "cube", (obj) =>
        {
            Instantiate(obj);
        });
        
        env = new LuaEnv();
        env.DoString("require 'Lua/Main'");
    }
}