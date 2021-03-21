using System.Collections.Generic;

namespace Animated.CPU.Parsers
{
    public class StringQuoteIdentifier : Identifier
    {
        public StringQuoteIdentifier(string name, List<string> tokens) : base(name, tokens)
        {
        }

        public StringQuoteIdentifier(string name, string tokensSpaceSep, string clr) : base(name, tokensSpaceSep, clr)
        {
        }
        
        public override bool TryParse(string txt, int lineIdx, int cc,  out ParserToken newToken)
        {
            foreach (var token in Tokens)
            {
                var backlen = cc - token.Length + 1;
                if (backlen < 0) continue;

                var end = cc + 1;
                if (txt[backlen..end].Equals(token))
                {
                    // Scan forward for next quote
                    var endq = txt.IndexOf(token, cc + 1);
                    if (endq > cc)
                    {
                        newToken = NewToken(lineIdx, backlen, endq + 1);
                        return true;    
                    }
                }    
            }

            newToken = default;
            return false;
        }
    }
}