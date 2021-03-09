using System;
using System.Linq;
using Animated.CPU.Animation;
using SkiaSharp;

namespace Animated.CPU.Model
{
    public class CodeSection : Section<Scene, SourceFile>
    {
        private TextBlockElement text;

        public CodeSection(IElement parent, SourceFile model, DBlock block) : base(parent, model, block)
        {
            Title = "Source Code";
        }

        protected override void Init()
        {
            text = Add(new TextBlockElement(this, this.Block, Scene.Styles.FixedFont));

            
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