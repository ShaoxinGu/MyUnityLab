using UnityEngine;

namespace GFramework
{
    public class StartGame : MonoBehaviour
    {
        void Start()
        {
            UIMgr.GetInstance().OpenUI("TestPanel");
        }
    }
}