using System;
using Animated.CPU.Animation;
using SkiaSharp;

namespace Animated.CPU.Model
{
    public class ALUElement : Element<Scene>
    {

        public ALUElement(Scene scene, DBlock b) : base(scene, b)
        {
        }
        public override void Init(SKSurface surface)
        {
            
        }
        public override void Step(TimeSpan step)
        {
            
        }
        public override void Draw(SKSurface surface)
        {
            // var canvas = surface.Canvas;
            // var draw   = new Drawing(canvas);
            //
            // // map to scheme
            // int cc = 0;
            //
            //
            // var itemPaint = new DBlockProps().Set(2, 2, 2, new SKPaint()
            // {
            //     Style = SKPaintStyle.Stroke,
            //     Color = SKColors.SkyBlue,
            // });
            //
            //
            // var stack = new DStack(this.Block, DOrient.Horz);
            // stack.Divide(4);
            //
            // foreach (var panel in stack.Children)
            // {
            //     draw.DrawRect(panel);
            //     
            // }
        }
    }
}