using System;
using System.Globalization;

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

    public static class DisplayHelper
    {
        public static string ToHex(ulong v) => v.ToString("X");
        public static string ToHex64(ulong v) => v.ToString("X").PadLeft(64/8*2, '0');
    }

    public static class ParseHelper
    {
        public static ulong ParseHexWord(string txt) => ulong.Parse(txt, NumberStyles.HexNumber);
        public static byte[] ParseHexByteArray(string txt) => Convert.FromHexString(txt);
    }
}