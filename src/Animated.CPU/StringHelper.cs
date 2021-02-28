using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    }
}