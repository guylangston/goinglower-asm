using System;
using System.Collections.Generic;

namespace GoingLower.CPU.Parsers
{
    public class WordPrefixIdentifier : Identifier
    {
        public WordPrefixIdentifier(string name, List<string> tokens) : base(name, tokens)
        {
        }

        public WordPrefixIdentifier(string name, string tokensSpaceSep, Func<char, bool> valid, string clr) : base(name, tokensSpaceSep, clr)
        {
            Valid = valid;
        }

        public Func<char, bool> Valid { get; set; } = char.IsLetterOrDigit;

        public override bool TryParse(string txt, int lineIdx, int cc, out ParserToken newToken)
        {
            foreach (var token in Tokens)
            {
                if (cc >= token.Length)
                {
                    if (txt[(cc-token.Length)..cc] == token)
                    {
                        var end = cc + 1;
                        while (end < txt.Length)
                        {
                            if (!Valid(txt[end]))
                            {
                                break;
                            }
                            end++;
                        }
                        newToken = new SyntaxMarkDown.LinkIdentifier.Token()
                        {
                            Ident   = this,
                            LineIdx = lineIdx,
                            Range   = new Range(cc-token.Length, end)
                        };
                        return true;
                    }
                }
                

            }

            newToken = default;
            return false;
        }
    }
}