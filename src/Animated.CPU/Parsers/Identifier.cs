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

    public class StringQuoteIdentifier : Identifier
    {
        public StringQuoteIdentifier(string name, List<string> tokens) : base(name, tokens)
        {
        }

        public StringQuoteIdentifier(string name, string tokensSpaceSep, string clr) : base(name, tokensSpaceSep, clr)
        {
        }
        
        public override bool TryParse(string s, int lineIdx, int cc,  out ParserToken newToken)
        {
            foreach (var token in Tokens)
            {
                var backlen = cc - token.Length + 1;
                if (backlen < 0) continue;

                var end = cc + 1;
                if (s[backlen..end].Equals(token))
                {
                    // Scan forward for next quote
                    var endq = s.IndexOf(token, cc + 1);
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