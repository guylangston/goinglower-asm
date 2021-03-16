using System;
using System.Collections.Generic;
using Animated.CPU.Animation;
using Animated.CPU.Parsers;
using SkiaSharp;

namespace Animated.CPU.Model
{
    public class Dialog
    {
        public string?       Title { get; set; }
        public IReadOnlyList<string>? Lines { get; set; }
    }
    
    public class DialogElement : TextSection<SceneExecute, Dialog>
    {

        public DialogElement(IElement parent, Dialog model, DBlock block) : base(parent, model, block)
        {
            IsHidden        = true;
            Title           = Model.Title;
            ShowLineNumbers = false;
            Parser          = new SourceParser(new SyntaxMarkDown());
        }

        
        public SKBitmap? Image { get; set; }

        protected override IReadOnlyList<string> GetLines(Dialog model)
        {
            return Model?.Lines;
        }

        protected override void Init()
        {
            base.Init();
        }

        protected override void Step(TimeSpan step)
        {
            Title = Model.Title;
            
            if (Image != null)
            {
                if (Image.Width > Block.W) Block.W = Image.Width;
                if (Image.Height > Block.H) Block.H = Image.Height;
            }

            Block.Z      = 100;
            text.Block.Z = 100;
            Block.CenterAt(Scene.Block);
            
            base.Step(step);
            
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