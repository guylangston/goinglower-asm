using System;
using System.Collections.Generic;

namespace Animated.CPU.Parsers
{
    public class ParseMap
    {
        public IReadOnlyList<string>   Lines      { get; set; }
        public List<List<ParserToken>> LineTokens { get; set; }

        public IEnumerable<(Identifier? ident, string txt)> Walk()
        {
            for (int cc = 0; cc < Lines.Count; cc++)
            {
                var l = Lines[cc];
                var t = LineTokens[cc];

                var s = 0;
                foreach (var token in t)
                {
                    var tt = token.Range.GetOffsetAndLength(l.Length);
                    if (tt.Offset > s)
                    {
                        yield return (null, l[s..tt.Offset]);
                    }
                    yield return (token.Ident, l[token.Range]);
                    s = tt.Offset + tt.Length;
                }
                if (s < l.Length -1)
                {
                    yield return (null, l[s..^0]);
                }
                yield return (null, Environment.NewLine);
            }
        }
    }
}