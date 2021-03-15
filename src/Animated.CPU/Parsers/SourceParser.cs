using System;
using System.Collections.Generic;
using System.Text;

namespace Animated.CPU.Parsers
{
   

    
    public class SourceParser
    {
        private readonly Syntax syntax;

        public SourceParser(Syntax syntax)
        {
            this.syntax = syntax;
        }

        public ParseMap Parse(IReadOnlyList<string> lines)
        {
            var ret = new ParseMap()
            {
                Lines = lines,
                LineTokens =   new List<List<ParserToken>>()
            };
            for (int ln = 0; ln < lines.Count; ln++)
            {
                var tok = new List<ParserToken>();
                var l   = lines[ln];

                if (!string.IsNullOrWhiteSpace(l))
                {
                    foreach (var phase in syntax.Phases)
                    {
                        Tokenize(phase, tok, l, ln);    
                    }
                    tok.Sort(((a, b) => a.Range.Start.Value.CompareTo(b.Range.Start.Value)));    
                }
                
                
                ret.LineTokens.Add(tok);
            }
            return ret;
        }

        int RangeContains(string s, Range r, int idx)
        {
            var (offset, length) = r.GetOffsetAndLength(s.Length);
            if (idx >= offset && idx <= offset + length)
            {
                return offset + length;
            }
            return 0;
        }
        
        private void Tokenize(IReadOnlyList<Identifier> phase, List<ParserToken> tok, string s, int lineIdx)
        {
            var cc   = 0;
            
            while (cc < s.Length)
            {
                
                // Already in range
                foreach (var t in tok)
                {
                    var endL = RangeContains(s, t.Range, cc);
                    if (endL > 0)
                    {
                        cc = endL;
                    }
                }

                if (cc >= s.Length) return;
                
                Identifier? curr = null;
                foreach (var ident in phase)
                {
                    if (ident.TryParse(s, lineIdx, cc, out var nToken))
                    {
                        curr = nToken.Ident;
                        tok.Add(nToken);

                        var (offset, length) = nToken.Range.GetOffsetAndLength(s.Length);
                        cc = offset + length;
                        
                        break;
                    }

                }
                cc++;

            }
        }
    }
}