using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Animated.CPU.Model;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.XPath;


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

        public class Block
        {
            public Block(string name)
            {
                Name = name;
                Values = new List<(string name, string val)>();
            }

            public string Name { get;  }
            public List<(string name, string val)> Values { get;  }
        }

        public static IEnumerable<Block> ParseNameValueBlocks(IEnumerable<string> lines)
        {
            Block? curr = null;
            foreach (var l in lines)
            {
                if (l.Trim().StartsWith("//")) continue;
                
                if (curr is null)
                {
                    curr = new Block(l);
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(l))
                    {
                        yield return curr;
                        curr = null;
                    }
                    else
                    {
                        if (TrySplitExclusive(l, ":", out var pair))
                        {
                            curr.Values.Add((pair.l.Trim(), pair.r.Trim()));
                        }
                        else
                        {
                            throw new Exception($"Must be in the format 'name: value' but was '{l}'");
                        }
                    }
                }
            }
        }

        public static IEnumerable<string> ToLines(string txt)
        {
            using var reader = new StringReader(txt);
            var l = string.Empty;
            while ((l = reader.ReadLine()) != null)
            {
                yield return l;
            }
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

        public static string ToHex(this byte[] arr)
        {
            var sb = new StringBuilder();

            foreach (var b in arr)
            {
                if (sb.Length> 0) sb.Append(":");
                
                sb.Append(b.ToString("X").PadLeft(2, '0'));
                
            }
            return sb.ToString();
        }
    }

    public static class ParseHelper
    {
        public static ulong ParseHexWord(string txt)
        {
            var clean = new string(txt.TakeWhile(x => char.IsLetterOrDigit(x)).ToArray());
            try
            {
                return ulong.Parse(clean, NumberStyles.HexNumber);
            }
            catch (Exception e)
            {
                throw new Exception($"Inp: {clean}", e);
            }
            
        }

        public static byte[] ParseHexByteArray(string txt) => Convert.FromHexString(txt);
    }
}