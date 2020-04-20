using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GFramework
{
    public class PoolData
    {
        public GameObject parentObj;
        public List<GameObject> poolList;

        public PoolData(GameObject obj, GameObject poolObj)
        {
            parentObj = new GameObject(obj.name);
            parentObj.transform.parent = poolObj.transform;
            poolList = new List<GameObject>();
            PushObj(obj);
        }

        public void PushObj(GameObject obj)
        {
            obj.SetActive(false);
            poolList.Add(obj);
            obj.transform.parent = parentObj.transform;
        }

        public GameObject GetObj()
        {
            GameObject obj = poolList[0];
            poolList.RemoveAt(0);
            obj.SetActive(true);
            obj.transform.parent = null;
            return obj;
        }
    }

    public class PoolMgr : Singleton<PoolMgr>
    {
        private Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();
        private GameObject PoolObj = null;

        public GameObject GetObj(string name)
        {
            GameObject obj = null;
            if (poolDic.ContainsKey(name) && poolDic[name].poolList.Count > 0)
            {
                obj = poolDic[name].GetObj();
            }
            else
            {
                obj = Object.Instantiate(Resources.Load<GameObject>(name));
                obj.name = name;
            }
            return obj;
        }

        public void PushObj(string name, GameObject obj)
        {
            if (PoolObj == null)
                PoolObj = new GameObject("Pool");
            obj.transform.parent = PoolObj.transform;
            obj.SetActive(false);
            if (poolDic.ContainsKey(name))
            {
                poolDic[name].poolList.Add(obj);
            }
            else
            {
                poolDic.Add(name, new PoolData(obj, PoolObj));
            }
        }

        public void Clear()
        {
            poolDic.Clear();
            PoolObj = null;
        }
    }
}
