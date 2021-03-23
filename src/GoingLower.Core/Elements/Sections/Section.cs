using System;
using GoingLower.Core.Actions;
using GoingLower.Core.Drawing;
using GoingLower.Core.Primitives;
using SkiaSharp;

namespace GoingLower.Core.Elements.Sections
{
    
    public class SimpleSection<TScene, TModel> : Section<TScene, TModel> where TScene:IScene
    {
        public SimpleSection(IElement parent, TModel model, DBlock block) : base(parent, model, block)
        {
            Title = model?.ToString();
        }
    }

    public interface IHighlightElement
    {
        public bool IsHighlighted { get; set; }
    }
    
    
    public abstract class Section<TScene, TModel> : Element<TScene, TModel>, IHighlightElement  where TScene:IScene
    {
        private SKRect posTitle;

        protected Section(IElement parent, TModel model, DBlock? block) : base(parent, model, block)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            if (model == null) throw new ArgumentNullException(nameof(model));
        }
        
        public string? Title         { get; set; }
        public string? TitleAction   { get; set; }
        public bool    IsHighlighted { get; set; }

        protected override void Draw(DrawContext surface)
        {
            if (Block == null) return;
            
            // Background
            surface.Canvas.DrawRect(Block.BorderDRect.ToSkRect(), Scene.StyleFactory.GetPaint(this, "bg"));

            if (Block.Border.All > 0)
            {
                if (IsHighlighted)
                {
                    surface.DrawHighlight(this);
                }
                else
                {
                    surface.Canvas.DrawRect(Block.BorderDRect.ToSkRect(),
                        Scene.StyleFactory.GetPaint(this, "border"));    
                }
                
                    
            }
            
            

            if (Title != null)
            {
                var    h1     = Scene.StyleFactory.GetPaint(this, "h1");
                var    h1bg   = Scene.StyleFactory.GetPaint(this, "h1bg");

                var size = new SKRect();
                h1.MeasureText(Title, ref size);

                posTitle = surface.DrawTextAndBGAtTopMiddle(Title, 
                    Block.Outer.TM + new SKPoint(0,  
                        Block.Margin.Top + Block.Border.Top - (size.Height*2/3)),
                    h1, h1bg, new SKPoint(10, 4));

            }
        }

        protected override void Step(TimeSpan step)
        {
            
        }

        public override PointSelectionResult? GetSelectionAtPoint(SKPoint p)
        {
            if (Block == null) return null;
            
            var hit =  base.GetSelectionAtPoint(p);
            if (hit != null && posTitle.Contains(p))
            {
                if (TitleAction != null)
                {
                    hit.Selection = new ActionModel()
                    {
                        Name = TitleAction,
                        Arg  = this
                    };    
                }
                
                return hit;
            }
            
            return null;

        }
    }
}