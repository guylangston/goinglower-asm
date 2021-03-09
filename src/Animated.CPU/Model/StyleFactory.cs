using System;
using System.Reflection;
using Animated.CPU.Animation;
using SkiaSharp;

namespace Animated.CPU.Model
{
    public class StyleFactory : IStyleFactory
    {
        public const string MonoSpace = "Jetbrains Mono";
        public const string Small = "Noto Sans";
        
        public StyleFactory()
        {
            this.Props = GetType().GetProperties();
            FixedFont = new SKPaint()
            {
                TextSize = 15,
                Color    = SKColor.Parse("#ccc"),
                Typeface = SKTypeface.FromFamilyName(
                    MonoSpace, 
                    SKFontStyleWeight.Normal, 
                    SKFontStyleWidth.Normal, 
                    SKFontStyleSlant.Upright)
            };
            FixedFontCyan     = Clone(FixedFont, p => p.Color = SKColors.Cyan);
            FixedFontYellow   = Clone(FixedFont, p => p.Color = SKColors.Yellow);
            FixedFontBlue     = Clone(FixedFont, p => p.Color = SKColors.LightBlue);
            FixedFontWhite    = Clone(FixedFont, p => p.Color = SKColors.White);
            FixedFontGray     = Clone(FixedFont, p => p.Color = SKColors.LightGray);
            FixedFontDarkGray = Clone(FixedFont, p => p.Color = SKColors.Gray);
            FixedFontURL      = Clone(FixedFont, p => p.Color = SKColors.LimeGreen);
            FixedFontArg      = Clone(FixedFont, p => p.Color = SKColors.LightSalmon);
            
            
            SmallFont = new SKPaint()
            {
                TextSize = 12,
                Color    = SKColor.Parse("#ccc"),
                Typeface = SKTypeface.FromFamilyName(
                    MonoSpace, 
                    SKFontStyleWeight.Normal, 
                    SKFontStyleWidth.Normal, 
                    SKFontStyleSlant.Upright)
            };

            Border = borderGray1;

            borderStrong = new SKPaint()
            {
                Style       = SKPaintStyle.Stroke,
                StrokeWidth = 3,
                Color       = SKColors.LightCoral
            };

            byte gray = 0x38;
            BackGround = new SKPaint()
            {
                Style       = SKPaintStyle.StrokeAndFill,
                StrokeWidth = 3,
                Color       = SKColor.Parse("#333"),
                Shader      = SKShader.CreateColor(new SKColor(gray, gray, gray, 220))
            };
            gray = 0x48;
            BackGroundAlt = new SKPaint()
            {
                Style       = SKPaintStyle.StrokeAndFill,
                StrokeWidth = 3,
                Color       = SKColor.Parse("#444"),
                Shader      = SKShader.CreateColor(new SKColor(gray, gray, gray, 220)),
                
            };
            
            Text = new SKPaint()
            {
                TextSize = 15,
                Color    = SKColor.Parse("#eee")
            };
            TextH1 = new SKPaint()
            {
                TextSize = 25,
                Color    = SKColors.Goldenrod,
                Typeface = SKTypeface.FromFamilyName(
                    MonoSpace, 
                    SKFontStyleWeight.Normal, 
                    SKFontStyleWidth.Normal, 
                    SKFontStyleSlant.Upright)
            };
            TextH1BG = new SKPaint()
            {
                Style = SKPaintStyle.Fill,
                Color = SKColor.Parse("#555")
            };

            TextLogo       = TextH1.Clone();
            TextLogo.Color = SKColors.LightSkyBlue;
            
            Arrow = new SKPaint()
            {
                Style       = SKPaintStyle.Stroke,
                StrokeWidth = 1.5f,
                Color       = SKColors.Lime,
                IsAntialias = true
            };

            Highlighted = Selected = Arrow;
        }

        public SKPaint FixedFontArg { get; set; }

        public SKPaint        FixedFontURL      { get; }
        public SKPaint        BackGround        { get; }
        public SKPaint        BackGroundAlt     { get; }
        public SKPaint        Text              { get; }
        public SKPaint        FixedFont         { get; }
        public SKPaint        FixedFontYellow   { get; }
        public SKPaint        FixedFontCyan     { get; }
        public SKPaint        FixedFontWhite    { get; }
        public SKPaint        FixedFontBlue     { get; }
        public SKPaint        FixedFontGray     { get; }
        public SKPaint        FixedFontDarkGray { get; }
        public SKPaint        Border            { get; }
        public SKPaint        Arrow             { get; }
        public SKPaint        TextLogo          { get; }
        public SKPaint        TextH1            { get; }
        public SKPaint        TextH1BG          { get; }
        public SKPaint        Selected          { get; }
        public SKPaint        Highlighted       { get; }
        public SKPaint        SmallFont         { get;  }
        public PropertyInfo[] Props             { get;  }

        public static SKPaint Clone(SKPaint cpy, Action<SKPaint> then)
        {
            var c = cpy.Clone();
            then(c);
            return c;
        }
        
        
        
        public SKPaint GetPaint(IElement e, string id)
        {
            id = id.ToLowerInvariant();


            if (e is DialogElement && id == "border") return borderStrong;
            
            switch (id)
            {
                case "border": 
                    if (e.Block?.Border.All == 1f) return borderGray1;
                    if (e.Block?.Border.All == 2f) return borderGray2;
                    if (e.Block?.Border.All == 3f) return borderGray3;
                    
                    return borderGray1;
            }
            
            foreach (var prop in Props)
            {
                if (prop.PropertyType == typeof(SKPaint))
                {
                    if (string.Equals(id, prop.Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return (SKPaint)prop.GetValue(this);
                    }    
                }
            }
            
            switch (id)
            {
                case "url": return FixedFontURL;
                case "h1": return TextH1;
                case "h1bg": return TextH1BG;
                case "hex": return hex;
                case "text": return Text;
                case "debug": return debug;
                case "arrow": return arrow;
                
                case "arg": return FixedFontArg;
                case "var": return FixedFontCyan;
                
                case "bg": return BackGround;
              
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

        
        

        private SKPaint arrow = new SKPaint()
        {
            Style       = SKPaintStyle.Stroke,
            StrokeWidth = 3,
            Color       = SKColors.Fuchsia
        };


        private SKPaint borderGray1 = new SKPaint()
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1,
            Color = SKColor.Parse("#444")
        };
        private SKPaint borderGray2 = new SKPaint()
        {
            Style       = SKPaintStyle.Stroke,
            StrokeWidth = 2,
            Color       = SKColor.Parse("#444")
        };
        private SKPaint borderGray3 = new SKPaint()
        {
            Style       = SKPaintStyle.Stroke,
            StrokeWidth = 3,
            Color       = SKColor.Parse("#444")
        };
        
        private SKPaint borderReg = new SKPaint()
        {
            Style       = SKPaintStyle.Stroke,
            StrokeWidth = 1,
            Color       = SKColors.DarkCyan
        };

        private SKPaint borderStrong;
    }
}
