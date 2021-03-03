using System.Collections.Generic;

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
    }
}