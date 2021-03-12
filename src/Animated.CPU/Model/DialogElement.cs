using System;
using System.Collections.Generic;
using Animated.CPU.Animation;
using SkiaSharp;

namespace Animated.CPU.Model
{
    public class Dialog
    {
        public string?       Title { get; set; }
        public IReadOnlyList<string>? Lines { get; set; }
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

            if (Image != null)
            {
                if (Image.Width > Block.W) Block.W = Image.Width;
                if (Image.Height > Block.H) Block.H = Image.Height;
            }

            Block.CenterAt(Scene.Block);
            
            this.text.Clear();
            if (Model.Lines != null)
            {
                text.WriteLine("OxGoingLower", Scene.Styles.FixedFontHuge);
                foreach (var line in Model.Lines)
                {
                    text.WriteLine(line);
                }    
            }
        }

        protected override void Draw(DrawContext surface)
        {
            surface.DrawRect(Scene.Block.Outer, Scene.Styles.BackGround);
            
            base.Draw(surface);

            if (Image != null)
            {
                var dest = Block.Inner.ToSkRect();
                surface.Canvas.DrawBitmap(Image, dest);    
            }
            
        }
    }
}