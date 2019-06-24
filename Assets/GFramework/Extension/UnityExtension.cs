using UnityEngine;

namespace GFramework.Extension
{
    public static class UnityExtension
    {
        public static void AddChild(this Transform parentTransform, Transform childTransform)
        {
            childTransform.SetParent(parentTransform);
        }

        public static void Reset(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
            transform.localRotation = Quaternion.identity;
        }

        public static void Show(this GameObject gameObject)
        {
            gameObject.SetActive(true);
        }

        public static void Hide(this GameObject gameObject)
        {
            gameObject.SetActive(false);
        }

        public static void Show(this Transform transform)
        {
            transform.gameObject.SetActive(true);
        }

        public static void Hide(this Transform transform)
        {
            transform.gameObject.SetActive(false);
        }

        public static void SetLocalPosX(this Transform transform, float x)
        {
            var localPosition = transform.localPosition;
            localPosition.x = x;
            transform.localPosition = localPosition;
        }

        public static void SetLocalPosY(this Transform transform, float y)
        {
            var localPosition = transform.localPosition;
            localPosition.y = y;
            transform.localPosition = localPosition;
        }

        public static void SetLocalPosZ(this Transform transform, float z)
        {
            var localPosition = transform.localPosition;
            localPosition.z = z;
            transform.localPosition = localPosition;
        }

        public static void SetLocalPosXY(this Transform transform, float x, float y)
        {
            var localPosition = transform.localPosition;
            localPosition.x = x;
            localPosition.y = y;
            transform.localPosition = localPosition;
        }

        public static void SetLocalPosXZ(this Transform transform, float x, float z)
        {
            var localPosition = transform.localPosition;
            localPosition.x = x;
            localPosition.z = z;
            transform.localPosition = localPosition;
        }

        public static void SetLocalPosYZ(this Transform transform, float y, float z)
        {
            var localPosition = transform.localPosition;
            localPosition.y = y;
            localPosition.z = z;
            transform.localPosition = localPosition;
        }
    }
}