using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace GFramework
{
    public class SceneMgr : Singleton<SceneMgr>
    {
        public void LoadScene(string name, UnityAction func)
        {
            SceneManager.LoadScene(name);
            func();
        }
        public void LoadSceneAsyn(string name, UnityAction func)
        {
            MonoMgr.Instance.StartCoroutine(RealLoadSceneSync(name, func));
        }

        private IEnumerator RealLoadSceneSync(string name, UnityAction func)
        {
            AsyncOperation ao = SceneManager.LoadSceneAsync(name);
            while(!ao.isDone)
            {
                EventMgr.Instance.DispatchEvent("ProgressUpdate", ao.progress);
                yield return ao.progress;
            }
            func();
        }
    }
}
