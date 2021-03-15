using System.Collections.Generic;

namespace Animated.CPU.Parsers
{
    public class SyntaxCSharp : Syntax
    {
        public SyntaxCSharp()
        {
            // Pass 1
            Phases.Add( new List<Identifier>()
            {
                new LineCommentIdentifier("comment", "//", "darkgray"),
                new StringQuoteIdentifier("quote", "\" ' `", "green")
            });
            
            
            // Pass 2
            Phases.Add( new List<Identifier>()
            {
                new Identifier("definition", "private protected public void class interface", "lightblue"),
                new Identifier("control", "for foreach while do return if else", "lightgreen"),
                new Identifier("type", "var string bool int byte long ulong uint int32 uint32", "cyan"),
                new Identifier("operator", "= <= >= < > + - * / ++ -- += -= *= /=", "yellow"),
                new Identifier("separator", "[ ] { } ( ) ; , . :", "orange"),
            });
        }
        
    }
    
    public class SyntaxAsm : Syntax
    {
        public SyntaxAsm()
        {
            // Pass 1
            Phases.Add( new List<Identifier>()
            {
                new LineCommentIdentifier("comment", ";", "darkgray"),
                new Identifier("quote", "\" ' `", "green")
            });
            
            
            // Pass 2
            Phases.Add( new List<Identifier>()
            {
                new Identifier("separator", "{ } ( ) ; , . : [ ]", "lightgreen"),
            });
        }
        
    }
    
    public class SyntaxIL : Syntax
    {
        public SyntaxIL()
        {
            // Pass 1
            Phases.Add( new List<Identifier>()
            {
                new LineCommentIdentifier("comment", "//", "darkgray"),
                new Identifier("quote", "\" ' `", "green")
            });
            
            
            // Pass 2
            Phases.Add( new List<Identifier>()
            {
                new Identifier("separator", "{ } ( ) ; , . : [ ]", "lightgreen"),
            });
        }
        
    }
}