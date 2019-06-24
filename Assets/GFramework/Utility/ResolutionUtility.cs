using UnityEngine;

namespace GFramework.Utility
{
    public class ResolutionUtility
    {
        public static float GetAspectRatio()
        {
            var isLandscape = Screen.width > Screen.height;
            return isLandscape ? (float)Screen.width / Screen.height : (float)Screen.height / Screen.width;
        }

        public static bool IsPadResolution()
        {
            return InAspectRange(4.0f / 3);
        }

        public static bool IsPhoneResolution()
        {
            return InAspectRange(16.0f / 9);
        }

        public static bool IsIPhone4SResolution()
        {
            return InAspectRange(3.0f / 2);
        }

        public static bool IsIPhoneXResolution()
        {
            return InAspectRange(2436.0f / 1125);
        }

        public static bool InAspectRange(float dstAspectRadio)
        {
            var aspect = GetAspectRatio();
            return aspect > (dstAspectRadio - 0.05f) && aspect < (dstAspectRadio + 0.05f);
        }
    }
}

