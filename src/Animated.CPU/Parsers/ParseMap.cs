using System;
using System.Collections.Generic;

namespace Animated.CPU.Parsers
{
    public class ParseMap
    {
        public IReadOnlyList<string>   Lines      { get; set; }
        public List<List<ParserToken>> LineTokens { get; set; }
        
        public struct Section
        {
            public bool        StartLine { get; set; }
            public Identifier? Ident     { get; set; }
            public string      Text      { get; set;}
            public uint        LineNo    { get; set;}
            public ParserToken Token     { get; set; }
        }

        public IEnumerable<Section> Walk()
        {
            for (int cc = 0; cc < Lines.Count; cc++)
            {
                var line = Lines[cc];
                var lineTokens = LineTokens[cc];

                var s = 0;
                foreach (var token in lineTokens)
                {
                    var tt = token.Range.GetOffsetAndLength(line.Length);
                    if (tt.Offset > s)
                    {
                        yield return new Section()
                        {
                            StartLine = s == 0,
                            Ident = null,
                            Text = line[s..tt.Offset],
                            LineNo = (uint)cc
                        };
                    }
                    yield return new Section()
                    {
                        StartLine = tt.Offset == 0,
                        Ident     = token.Ident,
                        Text      = token.OutputText ?? line[token.Range],
                        LineNo    = (uint)cc,
                        Token     = token
                    };
                    s = tt.Offset + tt.Length;
                }
                
                if (s < line.Length -1)
                {
                    yield return new Section()
                    {
                        StartLine = s == 0,
                        Ident     = null,
                        Text      = line[s..^0],
                        LineNo    = (uint)cc
                    };
                }
                
                yield return new Section()
                {
                    StartLine = s == 0,
                    Ident     = null,
                    Text      = Environment.NewLine,
                    LineNo    = (uint)cc
                };
            }
        }
    }
}