using System.Collections.Generic;

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
                new LineStartIdentifier("list", "-", "lightblue"),
                
            });
            
            
            // Pass 2
            Phases.Add( new List<Identifier>()
            {
                new Identifier("separator", "( ) [ ] # @", "lightgreen"),
            });
        }
        
    }
}