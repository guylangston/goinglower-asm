using System;
using System.Linq;
using Animated.CPU.Animation;

namespace Animated.CPU.Model
{
    public class CodeSection : Section<Scene, string>
    {
        private TextBlockElement text;

        public CodeSection(IElement parent, string model, DBlock block) : base(parent, model, block)
        {
            Title = model;
        }

        public override void Init()
        {
            text = Add(new TextBlockElement(this, this.Block, Scene.Styles.FixedFont));
        }

        protected override void Step(TimeSpan step)
        {
            text.Clear();
            
            if (Scene.Model.Story != null)
            {
                var primary = Scene.Model.Story.Source.Files.FirstOrDefault().Value;
                if (primary != null)
                {
                    text.WriteLine($"---[{primary.ShortName}]---");
                }

                var cc = 1;
                foreach (var line in primary.Lines)
                {
                    text.WriteLine($"L{cc.ToString().PadLeft(3)}: {line}");
                    cc++;
                }
            }

            text.WriteLine();
            text.WriteLine("--- Help ---");
            text.WriteLine("Key n - Next");
            text.WriteLine("Key p - Back/Previous");

        }
    }
}