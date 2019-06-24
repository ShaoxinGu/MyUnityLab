using System.Collections.Generic;
using GFramework.Extension;
using GFramework.MyAlgorithm;
using UnityEditor;
using UnityEngine;

namespace GFramework.Editor
{
    public class TestAlgorithm
    {
        [MenuItem("GFramework/算法测试/测试计数排序算法")]
        private static void MenuClicked1()
        {
            var test = new List<int> { 95, 94, 91, 98, 99, 90, 99, 93, 91, 92 };
            Debug.Log("排序前：" + test.GetString());
            test.CountingSort();
            Debug.Log("排序后：" + test.GetString());
        }


        [MenuItem("GFramework/算法测试/测试归并排序算法")]
        private static void MenuClicked2()
        {
            var test = new List<int> { 5, 6, 8, 3, 4 };
            Debug.Log("排序前：" + test.GetString());
            test.MergeSort();
            Debug.Log("升序排序后：" + test.GetString());
            test.MergeSort((a, b) => -a.CompareTo(b));
            Debug.Log("降序排序后：" + test.GetString());
        }

        [MenuItem("GFramework/算法测试/测试快速排序算法")]
        private static void MenuClicked3()
        {
            var test = new List<int> { 5, 6, 8, 3, 4 };
            Debug.Log("排序前：" + test.GetString());
            test.QuickSort();
            Debug.Log("升序排序后：" + test.GetString());
            test.QuickSort((a, b) => -a.CompareTo(b));
            Debug.Log("降序排序后：" + test.GetString());
        }
    }
}