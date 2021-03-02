using System;

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
        
        public string Title { get; set; }

        protected override void Draw(DrawContext surface)
        {
            // Background
            surface.Canvas.DrawRect(Block.BorderRect.ToSkRect(), Scene.StyleFactory.GetPaint(this, "bg"));
            surface.Canvas.DrawRect(Block.BorderRect.ToSkRect(), Scene.StyleFactory.GetPaint(this, "border"));

            if (Title != null)
            {
                surface.DrawText(Title, Scene.StyleFactory.GetPaint(this, "h1"), Block, BlockAnchor.TM);
            }
        }

        protected override void Step(TimeSpan step)
        {
            
        }
    }
}