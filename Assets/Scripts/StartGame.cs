﻿using UnityEngine;

namespace GFramework
{
    public class StartGame : MonoBehaviour
    {
        void Start()
        {
            UIManager.GetInstance().OpenUI("TestPanel");
        }
    }
}