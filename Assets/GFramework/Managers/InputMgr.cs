using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GFramework
{
    public class InputMgr : Singleton<InputMgr>
    {
        private bool IsCheck = false;

        public InputMgr()
        {
            MonoMgr.Instance.AddUpdateListner(InputUpdate);
        }

        public void SetCheckEnabled(bool isCheck)
        {
            IsCheck = isCheck;
        }

        private void InputUpdate()
        {
            if (!IsCheck)
                return;
            CheckKeyCode(KeyCode.W);
            CheckKeyCode(KeyCode.A);
            CheckKeyCode(KeyCode.S);
            CheckKeyCode(KeyCode.D);
        }

        private void CheckKeyCode(KeyCode keyCode)
        {
            if (Input.GetKeyDown(keyCode))
            {
                EventMgr.Instance.DispatchEvent("KEY_DOWN", keyCode);
            }
            if (Input.GetKeyUp(keyCode))
            {
                EventMgr.Instance.DispatchEvent("KEY_UP", keyCode);
            }
        }
    }
}