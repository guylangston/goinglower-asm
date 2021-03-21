using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Animated.CPU.Parsers
{
   

    
    public class SourceParser
    {
        private readonly Syntax syntax;

        public Syntax Syntax => syntax;

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

                foreach (var phase in syntax.Phases)
                {
                    Tokenize(phase, tok, l, ln);    
                }
                tok.Sort(((a, b) => a.Range.Start.Value.CompareTo(b.Range.Start.Value)));
                
                
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
        
        private void Tokenize(IReadOnlyList<Identifier> phase, List<ParserToken> tok, string txt, int lineIdx)
        {
            var cc   = 0;

            foreach (var byLine in phase.Where(x => x is LineIdentifier).Cast<LineIdentifier>())
            {
                tok.AddRange(byLine.ParseLine(txt, lineIdx));
            }
            
            while (cc < txt.Length)
            {
                // Already in range
                foreach (var t in tok)
                {
                    var endL = RangeContains(txt, t.Range, cc);
                    if (endL > 0)
                    {
                        cc = endL;
                    }
                }

                if (cc >= txt.Length) return;
                
                Identifier? curr = null;
                foreach (var ident in phase.Where(x=>x is not LineIdentifier))
                {
                    if (ident.TryParse(txt, lineIdx, cc, out var nToken))
                    {
                        curr = nToken.Ident;
                        tok.Add(nToken);

                        var (offset, length) = nToken.Range.GetOffsetAndLength(txt.Length);
                        cc = offset + length;
                        
                        break;
                    }

                }
                cc++;

            }
        }
    }
}