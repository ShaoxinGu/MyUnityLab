using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GFramework
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static T Instance { get; private set; }

        /// <summary>
        /// 赋值
        /// </summary>
        public virtual void Awake()
        {
            //单例必须唯一，重复就抛错
            if (Instance != null)
                throw new Exception("Repeated Singleton");

            Instance = this as T;
        }
    }
}