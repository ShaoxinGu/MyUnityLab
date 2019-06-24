using System.Collections.Generic;

namespace GFramework.MyAlgorithm
{
    public static class MyCountingSort
    {
        public static void CountingSort(this List<int> list)
        {
            if (list.Count == 0) return;
            int min = list[0];
            int max = min;
            foreach (int number in list)
            {
                if (number > max)
                {
                    max = number;
                }
                else if (number < min)
                {
                    min = number;
                }
            }
            int[] counting = new int[max - min + 1];
            for (int i = 0; i < list.Count; i++)
            {
                counting[list[i] - min] += 1;
            }
            int index = -1;
            for (int i = 0; i < counting.Length; i++)
            {
                for (int j = 0; j < counting[i]; j++)
                {
                    index++;
                    list[index] = i + min;
                }
            }
        }
    }
}