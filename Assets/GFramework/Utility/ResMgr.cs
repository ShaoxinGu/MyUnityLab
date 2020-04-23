using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace GFramework
{
    public class ResMgr : Singleton<ResMgr>
    {
        private Hashtable hashTable = null;

        public ResMgr()
        {
            hashTable = new Hashtable();
        }

        public T Load<T>(string path) where T : Object
        {
            T res = Resources.Load<T>(path);
            if (res is GameObject)
                return Object.Instantiate(res);
            else
                return res;
        }

        public void LoadAsyn<T>(string name, UnityAction<T> callBack) where T : Object
        {
            MonoMgr.Instance.StartCoroutine(RealLoadAsyn<T>(name, callBack));
        }

        public IEnumerator RealLoadAsyn<T>(string name, UnityAction<T> callBack) where T : Object
        {
            ResourceRequest rq = Resources.LoadAsync<T>(name);
            yield return rq;

            if(rq.asset is GameObject)
                callBack(Object.Instantiate(rq.asset) as T);
            else
                callBack(rq.asset as T);
        }
    }
}