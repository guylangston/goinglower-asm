using System;
using GoingLower.Core.Actions;
using GoingLower.Core.Drawing;
using GoingLower.Core.Primitives;

namespace GoingLower.Core.Elements
{
   
    public class ButtonElement : ElementBase
    {
        public ButtonElement(IElement parent, ActionModel actionModel, DBlock b) : base(parent, b)
        {
            base.Model = actionModel;
        }

        public new ActionModel Model => (ActionModel)base.Model;

        protected override void Step(TimeSpan step)
        {
            
        }

        protected override void Draw(DrawContext surface)
        {
            surface.DrawRect(Block.Outer, Scene.StyleFactory.GetPaint(this, "ButtonBg"));
            surface.DrawRect(Block.Outer, Scene.StyleFactory.GetPaint(this, "border"));
            var bg = surface.DrawText(Model.Name, Scene.StyleFactory.GetPaint(this, "ButtonText"), Block, BlockAnchor.MM);

            Block.W = bg.Width + 4 + Block.Padding.All;
        }
    }
}