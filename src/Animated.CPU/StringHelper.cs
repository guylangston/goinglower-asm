using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Animated.CPU.Model;

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

        /// <summary>
        /// Skips Nulls
        /// </summary>
        public static string Join<T>(IEnumerable<T?> items, Func<T, string?> toString = null,  string join = ", ")
        {
            if (items == null) return null;
            var sb = new StringBuilder();

            foreach (var b in items)
            {
                if (b is null) continue;
                
                if (sb.Length > 0) sb.Append(join);
                sb.Append(toString == null
                    ? b.ToString()
                    : toString(b) ?? "");
            }
            return sb.ToString();
        }
    }

    public static class DisplayHelper
    {
        public static string ToHex(ulong v) => v.ToString("X");
        public static string ToHex64(ulong v) => v.ToString("X").PadLeft(64/8*2, '0');

        public static string ToHex(byte[] arr)
        {
            var sb = new StringBuilder();

            foreach (var b in arr)
            {
                sb.Append(b.ToString("X").PadLeft(2, '0'));
            }
            return sb.ToString();
        }
    }

    public static class ParseHelper
    {
        public static ulong ParseHexWord(string txt) => ulong.Parse(txt, NumberStyles.HexNumber);
        public static byte[] ParseHexByteArray(string txt) => Convert.FromHexString(txt);
    }
}