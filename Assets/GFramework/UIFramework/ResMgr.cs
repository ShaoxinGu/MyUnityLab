﻿using UnityEngine;
using System.Collections;


namespace GFramework
{
    public class ResMgr : MonoBehaviour
    {
        private static ResMgr _instance;
        public static ResMgr Instance()
        {
            if (_instance == null)
            {
                _instance = new GameObject("ResMgr").AddComponent<ResMgr>();
            }
            return _instance;
        }

        private Hashtable hashTable = null;         //容器键值对集合

        private void Awake()
        {
            hashTable = new Hashtable();
        }

        /// <summary>
        /// 加载资源（自动缓存）
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
        /// 加载并实例化资源
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