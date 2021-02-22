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
            Style = SKPaintStyle.Stroke,
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

        public SKPaint text = new SKPaint()
        {
            TextSize = 15,
            Color    = SKColor.Parse("#eee")
        };
        
        public SKPaint hex = new SKPaint()
        {
            TextSize = 15,
            Color    = SKColors.Yellow
        };
        
        public SKPaint hex2 = new SKPaint()
        {
            TextSize = 15,
            Color    = SKColors.Cyan
        };

        
        public SKPaint h1 = new SKPaint()
        {
            TextSize = 25,
            Color    = SKColors.Yellow
        };

        

        private SKPaint borderGray = new SKPaint()
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1,
            Color = SKColor.Parse("#444")
        };
        
        private SKPaint borderReg = new SKPaint()
        {
            Style       = SKPaintStyle.Stroke,
            StrokeWidth = 1,
            Color       = SKColors.DarkCyan
        };
        
        public SKPaint GetPaint(IElement e, string id)
        {
            if (e.Model is Register r)
            {
                if (id == "Id") return t1;
                if (id == "Name") return t1a;
                if (id == "Value") return t2;
                
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

            if (e.Parent is ElementRegister)
            {
                switch (id)
                {
                    case "border": return borderReg;
                    case "hex": return hex2;
                    
                }
            }
            
            switch (id)
            {
                case "h1": return h1;
                case "hex": return hex;
                case "text": return text;
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