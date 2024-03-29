﻿using UnityEngine;
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
        
        env = new LuaEnv();
        env.DoString("require 'Lua/Main'");
    }
}