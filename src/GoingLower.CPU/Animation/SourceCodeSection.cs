using System.Collections.Generic;
using GoingLower.Core;
using GoingLower.Core.Primitives;
using GoingLower.CPU.Elements;
using GoingLower.CPU.Parsers;
using GoingLower.CPU.Model;

namespace GoingLower.CPU.Animation
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