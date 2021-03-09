using System;
using System.Collections.Generic;
using Animated.CPU.Animation;
using SkiaSharp;

namespace Animated.CPU.Model
{
    public class Dialog
    {
        public string       Title { get; set; }
        public List<string> Lines { get; } = new List<string>();
    }
    
    public class DialogElement : Section<Scene, Dialog>
    {
        private TextBlockElement text;
        
        public DialogElement(IElement parent, Dialog model, DBlock block) : base(parent, model, block)
        {
            IsHidden = true;
            Title    = Model.Title;
        }

        
        public SKBitmap? Image { get; set; }

        protected override void Init()
        {
            this.text = this.Add(new TextBlockElement(this, Block, Scene.Styles.FixedFontGray));
            
            Block.Z      = 100;
            text.Block.Z = 100;
        }

        protected override void Step(TimeSpan step)
        {
            Title = Model.Title;
            this.text.Clear();
            foreach (var line in Model.Lines)
            {
                text.WriteLine(line);
            }
        }

        protected override void Draw(DrawContext surface)
        {
            base.Draw(surface);

            if (Image != null)
            {
                var dest = Block.Inner.Inset(20, 20).ToSkRect();
                surface.Canvas.DrawBitmap(Image, dest);    
            }
            
        }
    }
}