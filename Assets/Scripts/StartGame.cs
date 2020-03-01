using UnityEngine;

namespace GFramework
{
    public class StartGame : MonoBehaviour
    {
        private void Awake()
        {
            RedDotManager.Instance.Initilize();
        }

        void Start()
        {
            UIMgr.Instance().OpenUI("UIMain");
        }
    }
}