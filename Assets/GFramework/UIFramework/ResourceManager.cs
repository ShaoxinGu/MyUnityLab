﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;


namespace GFramework
{
    public class ResourcesMgr : MonoBehaviour
    {
        private static ResourcesMgr _Instance;              //本脚本私有单例实例
        private Hashtable hashTable = null;                        //容器键值对集合

        /// <summary>
        /// 得到单例
        /// </summary>
        /// <returns></returns>
        public static ResourcesMgr GetInstance()
        {
            if (_Instance == null)
            {
                _Instance = new GameObject("ResourceMgr").AddComponent<ResourcesMgr>();
            }
            return _Instance;
        }

        void Awake()
        {
            hashTable = new Hashtable();
        }

        /// <summary>
        /// 加载资源，并自动缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">资源路径</param>
        /// <param name="isCache">是否缓存</param>
        /// <returns></returns>
        public T LoadResource<T>(string path, bool isCache) where T : UnityEngine.Object
        {
            if (hashTable.Contains(path))
            {
                return hashTable[path] as T;
            }

            T TResource = Resources.Load<T>(path);
            if (TResource == null)
            {
                Debug.LogError("LoadResource Fail, path=" + path);
            }
            else if (isCache)
            {
                hashTable.Add(path, TResource);
            }

            return TResource;
        }

        /// <summary>
        /// 实例化资源
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="isCache">是否缓存</param>
        /// <returns></returns>
        public GameObject LoadAsset(string path, bool isCache)
        {
            GameObject go = LoadResource<GameObject>(path, isCache);
            GameObject goClone = Instantiate(go);
            if (goClone == null)
            {
                Debug.LogError("LoadAsset Fail, path=" + path);
            }
            return goClone;
        }
    }
}