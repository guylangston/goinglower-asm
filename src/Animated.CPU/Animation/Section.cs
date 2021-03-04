using System;
using SkiaSharp;

namespace Animated.CPU.Animation
{
    public abstract class Section<TScene, TModel> : Element<TScene, TModel> where TScene:IScene
    {
        
        protected Section(IElement parent, TModel model, DBlock block) : base(parent, model, block)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (block == null) throw new ArgumentNullException(nameof(block));
        }
        
        public string? Title { get; set; }

        protected override void Draw(DrawContext surface)
        {
            // Background
            surface.Canvas.DrawRect(Block.BorderRect.ToSkRect(), Scene.StyleFactory.GetPaint(this, "bg"));
            surface.Canvas.DrawRect(Block.BorderRect.ToSkRect(), Scene.StyleFactory.GetPaint(this, "border"));

            if (Title != null)
            {
                var    h1 = Scene.StyleFactory.GetPaint(this, "h1");
                SKRect bounds  = new SKRect();
                h1.MeasureText(Title, ref bounds);

                
                var p = Block.Outer.TM + new SKPoint(-bounds.Width/2, 0);
                
                // BG
                var   h1bg = Scene.StyleFactory.GetPaint(this, "h1bg");
                float xx   = 10;
                float yy   = 3;
                surface.Canvas.DrawRect(new SKRect(p.X-xx, p.Y-yy, p.X+xx + bounds.Width, p.Y+yy + bounds.Height), h1bg);

                
                surface.Canvas.DrawText(Title, p + new SKPoint(0, bounds.Height), h1);
                
            }
        }

        protected override void Step(TimeSpan step)
        {
            
        }
    }
}