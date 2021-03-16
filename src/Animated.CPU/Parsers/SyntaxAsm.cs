using System.Collections.Generic;

namespace Animated.CPU.Parsers
{
    public class SyntaxAsm : Syntax
    {
        public SyntaxAsm()
        {
            // Pass 1
            Phases.Add( new List<Identifier>()
            {
                new LineCommentIdentifier("comment", ";", Syntax.CommentGray),
                new StringQuoteIdentifier("quote", "\" ' `", "green")
            });
            
            
            // Pass 2
            Phases.Add( new List<Identifier>()
            {
                new Identifier("separator", "{ } ( ) ; , . : [ ]", "lightgreen"),
            });
        }
        
    }
}