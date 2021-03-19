using System;
using Animated.CPU.Animation;
using SkiaSharp;

namespace Animated.CPU.Model
{
    public class StringElement : ElementBase
    {
        public StringElement(IElement parent, DBlock? b) : base(parent, b)
        {
        }

        public SKRect Bounds;
        
        public PropFloat  Size          { get; set; }
        public string     Text          { get; set; }
        public SKPaint    Style         { get; set; }
        public LineAnchor Anchor        { get; set; } 
        public SKPaint?   Shaddow       { get; set; }
        public SKPoint    ShaddowOffset { get; set; }
        
        protected override void Step(TimeSpan step)
        {
            
        }

        protected override void Draw(DrawContext surface)
        {
            var w = Style.MeasureText(Text, ref Bounds);
            Block.W = w;

            Style.TextSize = Shaddow.TextSize = Size.Value;

            surface.Canvas.DrawText(Text, Block.X, Block.Y, Style);

            if (Shaddow != null)
            {
                surface.Canvas.DrawText(Text, Block.X + ShaddowOffset.X, Block.Y + ShaddowOffset.Y, Shaddow);
            }
        }
    }
}