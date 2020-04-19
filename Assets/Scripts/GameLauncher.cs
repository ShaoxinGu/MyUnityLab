using UnityEngine;

namespace GFramework
{
    public class GameLauncher : MonoBehaviour
    {
        void Awake()
        {
            RedDotManager.Instance.Initilize();
        }

        void Start()
        {
            UIMgr.Instance.OpenUI("UIMain");
        }
    }
}