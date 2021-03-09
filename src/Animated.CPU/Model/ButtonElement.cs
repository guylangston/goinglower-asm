using System;
using Animated.CPU.Animation;

namespace Animated.CPU.Model
{
    public class Action
    {
        public string Name { get; set; }
        public object Arg  { get; set; }
    }
    
    public class ButtonElement : ElementBase
    {
        public ButtonElement(IElement parent, Action action, DBlock b) : base(parent.Scene, parent, b)
        {
            base.Model = action;
        }

        public new Action Model => (Action)base.Model;

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