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
}