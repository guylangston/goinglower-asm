using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoingLower.Core.Helpers
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
        
        public static int IndexOfElse<T>(this IReadOnlyList<T>? items, T item, int elseVal)
        {
            if (items == null) throw new Exception();

            for (int cc = 0; cc < items.Count; cc++)
            {
                if (item.Equals(items[cc]))
                {
                    return cc;
                }
            }

            return elseVal;
        }

        public static IEnumerable<(T a, T b)> PairWise<T>(IEnumerable<T> chain)
        {
            T    last  = default;
            bool first = true;
            foreach (var item in chain)
            {
                if (first)
                {
                    last  = item;
                    first = false;
                }
                else
                {
                    yield return (last, item);
                    last = item;
                }
            }
        }

        public static async Task<List<T>> ToList<T>(IAsyncEnumerable<T> readLines)
        {
            var res = new List<T>();
            await foreach (var line in readLines)
            {
                res.Add(line);
            }
            return res;
        }
    }
}