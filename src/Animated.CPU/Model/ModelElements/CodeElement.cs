using System;
using System.Linq;
using Animated.CPU.Animation;
using SkiaSharp;

namespace Animated.CPU.Model.ModelElements
{
    public class CodeElement : Section<Scene, SourceFile>
    {
        private TextBlockElement text;

        public CodeElement(IElement parent, SourceFile model, DBlock block) : base(parent, model, block)
        {
            Title = model.Title ?? model.Name;
        }

        protected override void Init()
        {
            text = Add(new TextBlockElement(this, this.Block, Scene.Styles.FixedFontSource));

            uint cc = 1;
            foreach (var line in Model.Lines)
            {
                text.Write($"{cc.ToString().PadLeft(3)}: ", Scene.Styles.FixedFontDarkGray)
                    .SetModel(cc);
                text.WriteLine(line);
                    
                cc++;
            }
        }

        protected override void Step(TimeSpan step) 
        {
            

        }

        public TextBlockElement.Span? GetLine(uint line)
        {
            if (text.TryGetSpanFromModel(line, out var s))
            {
                return s;
            }
            return null;
        }
    }
}