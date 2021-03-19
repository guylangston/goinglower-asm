using System;
using Animated.CPU.Animation;
using SkiaSharp;

namespace Animated.CPU.Model
{
    public class StringElement : ElementBase
    {
        public string  Text  { get; set; }
        public SKPaint Style { get; set; }
        public SKRect Bounds;
        public LineAnchor Anchor { get; set; } 
        
        protected override void Step(TimeSpan step)
        {
            
        }

        protected override void Draw(DrawContext surface)
        {
            var w = Style.MeasureText(Text, ref Bounds);

            surface.Canvas.DrawText(Text, Block.X - w / 2, Block.Y + Bounds.Height, Style);
        }
    }
}