using System;
using System.Collections.Generic;
using Animated.CPU.Model;

namespace Animated.CPU
{
    public static class GeneralHelper
    {
        public static IEnumerable<(int index, T val)> WithIndex<T>(this IEnumerable<T> items)
        {
            int cc = 0;
            foreach (var x in items)
            {
                yield return (cc++, x);
            }
        }

        public static T? ByIndexOrDefault<T>(this IReadOnlyList<T>? items, int idx)
        {
            if (items == null) return default;
            if (idx < 0 || idx >= items.Count) return default;
            return items[idx];

        }
        
        public static int IndexOf<T>(this IReadOnlyList<T>? items, T item)
        {
            if (items == null) throw new Exception();

            for (int cc = 0; cc < items.Count; cc++)
            {
                if (item.Equals(items[cc]))
                {
                    return cc;
                }
            }

            throw new Exception("Not found");
        }
    }
}