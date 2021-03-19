using System;
using System.Collections.Generic;
using System.IO.Enumeration;

namespace Animated.CPU.Parsers
{
    public class SyntaxMarkDown : Syntax
    {
        public SyntaxMarkDown()
        {
            // Pass 1
            Phases.Add( new List<Identifier>()
            {
                new LineStartIdentifier("header", "#", "yellow"),
                new StringQuoteIdentifier("strings", "` \"", "cyan")
            });
            
            
            // Pass 2
            Phases.Add( new List<Identifier>()
            {
                new Identifier("line-prefix", "> -", "gray"),
                new Identifier("block", "`", "darkgray"),
                new LinkIdentifier("link", "lightgreen")
            });
        }

        public class LinkIdentifier : LineIdentifier
        {
            public LinkIdentifier(string name, string clr) : base(name, new List<string>() { "[", "]", "(", ")", "!"})
            {
                Colour = clr;
            }

            public class Token : ParserToken
            {
                public string Url { get; set; }
            }

            public override IEnumerable<ParserToken> ParseLine(string s, int lineIdx)
            {
                var pos = 0;
                while (true)
                {
                    var match = FindAt(s, pos, lineIdx);
                    if (match is null) break;
                    yield return match;

                    var o = match.Range.GetOffsetAndLength(s.Length);
                    pos += o.Offset + o.Length + 1;
                }
            }

            private Token? FindAt(string s, int pos, int lineIdx)
            {
                var idx0 = s.IndexOf('[', pos);
                if (idx0 < 0) return null;
                
                var idx1 = s.IndexOf(']', idx0);
                if (idx1 < 0) return null;
                
                var idx2 = s.IndexOf('(', idx1);
                if (idx2 < 0) return null;
                
                var idx3 = s.IndexOf(')', idx2);
                if (idx3 < 0) return null;

                var name = s[(idx0+1)..(idx1)];
                var url  = s[idx2..idx3];
                
                return new Token()
                {
                    Ident      = this,
                    LineIdx    = lineIdx,
                    Range      = new Range(idx0, idx3+1),
                    OutputText = name,
                    Url        = url
                };
            }
        }
        
    }
}