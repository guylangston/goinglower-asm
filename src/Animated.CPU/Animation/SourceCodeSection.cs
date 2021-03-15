using System.Collections.Generic;
using Animated.CPU.Model;
using Animated.CPU.Parsers;

namespace Animated.CPU.Animation
{
    public class SourceCodeSection : TextSection<IScene, SourceFile>
    {
        public SourceCodeSection(IElement parent, SourceFile model, DBlock block) : base(parent, model, block)
        {
            Title  = model.Title ?? model.Name;
            
        }
        
        

        protected override IReadOnlyList<string> GetLines(SourceFile model) => model.Lines;
    }
}