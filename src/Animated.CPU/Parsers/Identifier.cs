using System;
using System.Collections.Generic;
using System.Linq;

namespace Animated.CPU.Parsers
{
    
      
    public class ParserToken 
    {   
        public int        LineIdx { get; set; }
        public Range      Range   { get; set; }
        public Identifier Ident   { get; set; }

        public override string ToString() => $"{Ident.Name}:{Range}";
    }
    
    
    public abstract class Syntax
    {
        public List<List<Identifier>> Phases { get; } = new List<List<Identifier>>();

        public const string CommentGray = "#666";
    }

    
    public class Identifier
    {
        public Identifier(string name, List<string> tokens)
        {
            Name   = name;
            Tokens = tokens;
        }

        public Identifier(string name, string tokensSpaceSep, string clr)
        {
            Name   = name;
            Tokens = tokensSpaceSep.Split(' ').ToList();
            Colour = clr;
        }

        public string       Colour { get; set; }
        public string       Name   { get; set; }
        public List<string> Tokens { get; set; }

        public virtual bool TryParse(string s, int lineIdx, int cc,  out ParserToken newToken)
        {
            foreach (var token in Tokens)
            {
                var backlen = cc - token.Length + 1;
                if (backlen < 0) continue;

                var end = cc + 1;
                if (s[backlen..end].Equals(token))
                {
                    newToken  = NewToken(lineIdx, backlen, end);
                    return true;
                }    
            }

            newToken = default;
            return false;
        }

        protected virtual ParserToken NewToken(int lineIdx, int backlen, int end) => new ParserToken()
        {
            Ident   = this,
            LineIdx = lineIdx,
            Range   = new Range(backlen, end)
        };
    }
}