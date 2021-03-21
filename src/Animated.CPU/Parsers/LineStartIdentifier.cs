using System;
using System.Collections.Generic;

namespace Animated.CPU.Parsers
{
    public class LineStartIdentifier : Identifier
    {
        public LineStartIdentifier(string name, List<string> tokens) : base(name, tokens)
        {
        }

        public LineStartIdentifier(string name, string tokensSpaceSep, string clr) : base(name, tokensSpaceSep, clr)
        {
        }

        public override bool TryParse(string txt, int lineIdx, int cc, out ParserToken newToken)
        {
            foreach (var token in Tokens)
            {
                if (txt.Trim().StartsWith(token))
                {
                    newToken = new ParserToken()
                    {
                        Ident   = this,
                        LineIdx = lineIdx,
                        Range   = new Range(0, new Index(0, true)),
                        OutputText = txt.Remove(0, token.Length).Trim()
                    };
                    return true;
                }
            }

            newToken = default;
            return false;
        }
    }
}