using System;
using System.Collections.Generic;

namespace GFramework.MyAlgorithm
{
    public static class MyMergeSort
    {
        public static void MergeSort(this List<int> list)
        {
            MergeSortHelper(list, 0, list.Count - 1, (a, b) => a.CompareTo(b));
        }

        public static void MergeSort(this List<int> list, Comparison<int> comparison)
        {
            MergeSortHelper(list, 0, list.Count - 1, comparison);
        }

        private static void MergeSortHelper(IList<int> list, int left, int right, Comparison<int> comparison)
        {
            if (left >= right) return;
            var mid = (left + right) / 2;
            MergeSortHelper(list, left, mid, comparison);
            MergeSortHelper(list, mid + 1, right, comparison);
            Merge(list, left, mid, right, comparison);
        }

        //整合左右两个有序列表
        private static void Merge(IList<int> list, int left, int mid, int right, Comparison<int> comparison)
        {
            var leftCount = mid - left + 1;
            var rightCount = right - mid;
            var leftList = new List<int>(leftCount);
            var rightList = new List<int>(rightCount);

            for (var i = 0; i < leftCount; i++)
            {
                leftList.Add(list[left + i]);
            }

            for (var i = 0; i < rightCount; i++)
            {
                rightList.Add(list[mid + 1 + i]);
            }

            var curLeft = 0;
            var curRight = 0;
            for (var i = 0; i < leftCount + rightCount; i++)
            {
                if (curLeft < leftCount && curRight < rightCount)
                {
                    if (comparison(leftList[curLeft], rightList[curRight]) <= 0)
                    {
                        list[left + i] = leftList[curLeft];
                        curLeft++;
                    }
                    else
                    {
                        list[left + i] = rightList[curRight];
                        curRight++;
                    }
                }
                else if (curLeft < leftCount && curRight >= rightCount)
                {
                    for (var m = curLeft; m < leftCount; m++)
                    {
                        list[left + i + m - curLeft] = leftList[m];
                    }
                    return;
                }
                else if (curLeft >= leftCount && curRight < rightCount)
                {
                    for (var n = curRight; n < rightCount; n++)
                    {
                        list[left + i + n - curRight] = rightList[n];
                    }
                    return;
                }
            }
        }
    }
}