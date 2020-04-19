using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GFramework
{
    public class PoolMgr : Singleton<PoolMgr>
    {
        private Dictionary<string, List<GameObject>> poolDic = new Dictionary<string, List<GameObject>>();


        public GameObject GetObj(string name)
        {
            GameObject obj = null;
            if (poolDic.ContainsKey(name) && poolDic[name].Count > 0)
            {
                obj = poolDic[name][0];
                poolDic[name].RemoveAt(0);
            }
            else
            {
                obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
                obj.name = name;
            }
            obj.SetActive(true);
            return obj;
        }

        public void PushObj(string name, GameObject obj)
        {
            obj.SetActive(false);
            if (poolDic.ContainsKey(name))
            {
                poolDic[name].Add(obj);
            }
            else
            {
                poolDic[name] = new List<GameObject>() { obj };
            }
        }
    }
}
