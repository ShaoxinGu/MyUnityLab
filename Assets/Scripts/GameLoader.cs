using UnityEngine;

namespace GFramework
{
    public class GameLoader : MonoBehaviour
    {
        private void Awake()
        {
            RedDotManager.Instance.Initilize();
        }

        void Start()
        {
            UIManager.Instance.OpenUI("UIMain");
        }
    }
}