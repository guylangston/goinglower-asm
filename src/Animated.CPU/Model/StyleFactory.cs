using System;
using Animated.CPU.Animation;
using SkiaSharp;

namespace Animated.CPU.Model
{
    internal class StyleFactory : IStyleFactory
    {
        private SKPaint def = new SKPaint()
        {
            Color       = SKColors.Pink,
            StrokeWidth = 1,
            PathEffect  = SKPathEffect.CreateDash(new []{5f,5f}, 20)
        };
        
            
        internal SKPaint p1 = new SKPaint()
        {
            Style       = SKPaintStyle.Stroke,
            Color       = new SKColor(255,0,0),
            StrokeWidth = 2
                    
        };
            
        internal SKColor bg = SKColor.Parse("#333");
        internal SKPaint b1 = new SKPaint()
        {
            Style       = SKPaintStyle.Stroke,
            Color       = new SKColor(200,200,200),
            StrokeWidth = 2
                    
        };
        internal SKPaint t1    =  new SKPaint { TextSize = 15, Color = SKColor.Parse("#00d0fa")};
        internal SKPaint t1a   =  new SKPaint { TextSize = 15, Color = SKColor.Parse("#00fa00")};
        internal SKPaint t2    =  new SKPaint { TextSize = 20, Color = SKColor.Parse("#00ff00")};
        internal SKPaint debug =  new SKPaint { TextSize = 10, Color = SKColor.Parse("#ffffff")};

        

        private SKPaint borderGray = new SKPaint()
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColor.Parse("#444")
        };
        
        public SKPaint GetPaint(IElement e, string id)
        {
            if (e.Model is Register r)
            {
                var grad_clr = new[]
                {
                    SKColor.Parse("#C0666666"),
                    SKColor.Parse("#C0444444")
                };

                return new SKPaint()
                {
                    Style  = SKPaintStyle.StrokeAndFill,
                    Color  = SKColors.SkyBlue,
                    Shader = SKShader.CreateLinearGradient(e.Block.Outer.TL, e.Block.Outer.BR, grad_clr, SKShaderTileMode.Repeat)
                    // Shader = SKShader.CreateRadialGradient(panel.Outer.MM, panel.Outer.W/2, new[]
                    // {
                    //     SKColor.Parse("#666"),
                    //     SKColor.Parse("#444")
                    // }, SKShaderTileMode.Repeat)
                };
            }
            
            switch (id)
            {
                case "debug": return debug;
                case "border": return borderGray;
            }
            
            //Console.WriteLine("Unknown Paint Style {e}#{id");
            return def;
        }
        
        public SKColor GetColor(IElement e, string id)
        {
            switch (id)
            {
                case "bg": return bg;
                
            }
            
            //Console.WriteLine("Unknown Color Style {e}#{id");
            return def.Color;
        }
    }
}