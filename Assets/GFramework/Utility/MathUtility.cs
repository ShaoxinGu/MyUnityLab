using UnityEngine;

namespace GFramework.Utility
{
    public class MathUtility
    {
        public static bool Percent(int percent)
        {
            return Random.Range(0, 100) < percent;
        }
    }
}