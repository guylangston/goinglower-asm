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

        public static T? ByIndexOrDefault<T>(IReadOnlyList<T>? items, int idx)
        {
            if (items == null) return default;
            if (idx < 0 || idx >= items.Count) return default;
            return items[idx];

        }
    }
}