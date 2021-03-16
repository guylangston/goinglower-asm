using System;
using System.Collections.Generic;

namespace Animated.CPU.Parsers
{
    public class LineCommentIdentifier : Identifier
    {
        public LineCommentIdentifier(string name, List<string> tokens) : base(name, tokens)
        {
        }

        public LineCommentIdentifier(string name, string tokensSpaceSep, string clr) : base(name, tokensSpaceSep, clr)
        {
        }

        protected override ParserToken NewToken(int lineIdx, int backlen, int end) => new ParserToken()
        {
            Ident   = this,
            LineIdx = lineIdx,
            Range   = new Range(backlen, new Index(0, true))
        };
    }
    
    
    public class LineStartIdentifier : Identifier
    {
        public LineStartIdentifier(string name, List<string> tokens) : base(name, tokens)
        {
        }

        public LineStartIdentifier(string name, string tokensSpaceSep, string clr) : base(name, tokensSpaceSep, clr)
        {
        }

        public override bool TryParse(string s, int lineIdx, int cc, out ParserToken newToken)
        {
            foreach (var token in Tokens)
            {
                if (s.Trim().StartsWith(token))
                {
                    newToken = new ParserToken()
                    {
                        Ident = this,
                        LineIdx = lineIdx,
                        Range = new Range(0, new Index(0, true))
                    };
                    return true;
                }
            }

            newToken = default;
            return false;
        }
    }
}