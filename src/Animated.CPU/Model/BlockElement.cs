using System;
using Animated.CPU.Animation;
using SkiaSharp;

namespace Animated.CPU.Model
{
    public class BlockElement : Element<Scene>
    {
        
        public BlockElement(Scene scene, DBlock block) : base(scene, block)
        {
        }
        public BlockElement(IElement parent, DBlock block) : base(parent, block)
        {
        }
        
        public BlockAnchor Anchor { get; set; }
        
        public override void Step(TimeSpan step)
        { 
            
        }
        public override void Draw(SKSurface surface)
        {
            var canvas = surface.Canvas;
            var draw   = new Drawing(canvas);
            
            var sBorder = Scene.StyleFactory.GetPaint(this, "border");
            var sText = Scene.StyleFactory.GetPaint(this, "h1");
            draw.DrawRect(Block, sBorder);
            draw.DrawText(Model?.ToString(), sText, Block, Anchor);
        }
        
    }
}