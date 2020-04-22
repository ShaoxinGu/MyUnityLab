using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Internal;

namespace GFramework
{
    public class MonoController : MonoBehaviour
    {
        private event UnityAction updateEvent;

        void Start()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        // Update is called once per frame
        void Update()
        {
            updateEvent?.Invoke();
        }

        public void AddUpdateListner(UnityAction func)
        {
            updateEvent += func;
        }

        public void RemoveUpdateListner(UnityAction func)
        {
            updateEvent -= func;
        }
    }

    public class MonoMgr:Singleton<MonoMgr>
    {
        private MonoController monoController;

        public MonoMgr()
        {
            GameObject obj = new GameObject("MonoController");
            monoController = obj.AddComponent<MonoController>();
        }

        public void AddUpdateListner(UnityAction func)
        {
            monoController.AddUpdateListner(func);
        }

        public void RemoveUpdateListner(UnityAction func)
        {
            monoController.RemoveUpdateListner(func);
        }

        public Coroutine StartCoroutine(string methodName)
        {
            return monoController.StartCoroutine(methodName);
        }

        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return monoController.StartCoroutine(routine);
        }
        public Coroutine StartCoroutine(string methodName, [DefaultValue("null")] object value)
        {
            return monoController.StartCoroutine(methodName, value);
        }
    }
}

