using System.Collections.Generic;

namespace GFramework.Extension
{
    public static class CollectionExtension
    {
        public static string GetString<T>(this List<T> list)
        {
            var res = "";
            list.ForEach(i => res += i.ToString() + " ");
            return res;
        }
    }
}
