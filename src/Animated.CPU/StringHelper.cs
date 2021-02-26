namespace Animated.CPU
{
    public static class StringHelper
    {



        public static bool TrySplitExclusive(string line, string s, out (string l, string r) result)
        {
            var idx = line.IndexOf(s);
            if (idx < 0)
            {
                result = default;
                return false;
            }
            
            result = (line[0..idx], line[(idx + s.Length)..]);
            return true;

        }
    }
}