using System;
using System.Collections.Generic;

namespace GFramework.MyAlgorithm
{
    public static class MyQuickSort
    {
        public static void QuickSort(this List<int> list)
        {
            QuickSortHelper(list, 0, list.Count - 1, (a, b) => a.CompareTo(b));
        }

        public static void QuickSort(this List<int> list, Comparison<int> comparison)
        {
            QuickSortHelper(list, 0, list.Count - 1, comparison);
        }

        private static void QuickSortHelper(IList<int> list, int left, int right, Comparison<int> comparison)
        {
            if (left >= right) return;
            var pivotIndex = Division(list, left, right, comparison);
            //对枢轴的左边部分进行排序
            QuickSortHelper(list, left, pivotIndex - 1, comparison);
            //对枢轴的右边部分进行排序
            QuickSortHelper(list, pivotIndex + 1, right, comparison);
        }

        //获取按枢轴值左右分流后枢轴的位置
        private static int Division(IList<int> list, int left, int right, Comparison<int> comparison)
        {
            while (left < right)
            {
                var pivot = list[left]; //将首元素作为枢轴
                if (comparison(pivot, list[left + 1]) > 0)
                {
                    list[left] = list[left + 1];
                    list[left + 1] = pivot;
                    left++;
                }
                else
                {
                    var temp = list[right];
                    list[right] = list[left + 1];
                    list[left + 1] = temp;
                    right--;
                }
            }
            return left;
        }
    }
}

