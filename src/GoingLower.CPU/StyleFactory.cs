using System;
using System.Collections.Generic;
using System.Reflection;
using GoingLower.Core;
using GoingLower.Core.Helpers;
using GoingLower.CPU.Animation;
using GoingLower.CPU.Elements;
using SkiaSharp;

namespace GoingLower.CPU
{
    public static class Theme 
    {
        public const string MonoSpace = "Jetbrains Mono";
        public const string Small = "Noto Sans";
        public const float TextSizeDefault = 16;//20;
        public const string Sans = "Ubuntu";

    }
    
    public class StyleFactory : IStyleFactory
    {
        private readonly Dictionary<string, SKColor> name = new();
        private readonly PropertyInfo[] props;
        
        public SKPaint Annotate2         { get; }
        public SKPaint ButtonText        { get; }
        public SKPaint ButtonBg          { get; }
        public SKPaint FixedFontSource   { get; }
        public SKPaint FixedFontArg      { get; }
        public SKPaint FixedFontURL      { get; }
        public SKPaint BackGround        { get; }
        public SKPaint BackGroundAlt     { get; }
        public SKPaint Text              { get; }
        public SKPaint FixedFont         { get; }
        public SKPaint FixedFontYellow   { get; }
        public SKPaint FixedFontCyan     { get; }
        public SKPaint FixedFontWhite    { get; }
        public SKPaint FixedFontBlue     { get; }
        public SKPaint FixedFontGray     { get; }
        public SKPaint FixedFontDarkGray { get; }
        public SKPaint Border            { get; }
        public SKPaint Arrow             { get; }
        public SKPaint ArrowGray         { get; }
        public SKPaint ArrowAlt          { get; }
        public SKPaint TextLogo          { get; }
        public SKPaint TextH1            { get; }
        public SKPaint TextH1BG          { get; }
        public SKPaint Selected          { get; }
        public SKPaint Highlighted       { get; }
        public SKPaint SmallFont         { get; }
        public SKPaint FixedFontHuge     { get; }
        public SKPaint Annotate          { get; }

        
        public StyleFactory()
        {
            this.props = GetType().GetProperties();
            FixedFont = new SKPaint()
            {
                TextSize = Theme.TextSizeDefault,
                Color    = SKColor.Parse("#ccc"),
                Typeface = SKTypeface.FromFamilyName(
                    Theme.MonoSpace, 
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
            FixedFontSource   = Clone(FixedFont, p => p.Color = SKColors.LightCoral);
            FixedFontHuge     = Clone(FixedFont, p => {
                p.Color    = SKColors.LightCoral;
                p.TextSize = 36;
            });
            
            SmallFont = new SKPaint()
            {
                TextSize = Theme.TextSizeDefault - 5,
                Color    = SKColor.Parse("#ccc"),
                Typeface = SKTypeface.FromFamilyName(
                    Theme.MonoSpace, 
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
            
            gray = 0x28;
            ButtonBg = new SKPaint()
            {
                Style       = SKPaintStyle.StrokeAndFill,
                StrokeWidth = 3,
                Color       = SKColor.Parse("#333"),
                Shader      = SKShader.CreateColor(new SKColor(gray, gray, gray))
            };
            ButtonText = Clone(FixedFont, p => {
                p.TextSize = 12;
                p.Color    = SKColors.Wheat;
            });
            
            Text = new SKPaint()
            {
                TextSize = Theme.TextSizeDefault,
                Color    = SKColor.Parse("#eee")
            };
            TextH1 = new SKPaint()
            {
                TextSize = Theme.TextSizeDefault + 5,
                Color    = SKColors.Goldenrod,
                Typeface = SKTypeface.FromFamilyName(
                    Theme.MonoSpace, 
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
            
            Annotate = new SKPaint()
            {
                Style       = SKPaintStyle.Stroke,
                StrokeWidth = 4f,
                Color       = SKColors.PeachPuff,
                IsAntialias = true
            };
            Annotate2 = new SKPaint()
            {
                Style       = SKPaintStyle.Stroke,
                StrokeWidth = 6f,
                Color       = new SKColor(150, 150, 150, 50),
                IsAntialias = false
            };
            
            Arrow = new SKPaint()
            {
                Style       = SKPaintStyle.Stroke,
                StrokeWidth = 1.5f,
                Color       = SKColors.Lime,
                IsAntialias = true
            };
            ArrowGray = Arrow.CloneAndUpdate(x => x.Color = SKColors.Gray);
            ArrowAlt = Arrow.CloneAndUpdate(x => x.Color = SKColors.BlueViolet);

            Highlighted = Selected = Arrow;


            MakeNamedColours();
        }

        

        private void MakeNamedColours()
        {
            name.Clear();
            foreach (var prop in typeof(SKColors).GetFields())
            {
                name[prop.Name.ToLowerInvariant()] = (SKColor)prop.GetValue(null);
            }
        }

        
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
            
            foreach (var prop in props)
            {
                if (prop.PropertyType == typeof(SKPaint))
                {
                    if (string.Equals(id, prop.Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return (SKPaint)prop.GetValue(this);
                    }    
                }
            }

            if (id.StartsWith("font-"))
            {
                float size = Theme.TextSizeDefault;
                if (e is ITextStyleContainer ts)
                {
                    size = ts.Normal.TextSize;
                }
                var rem = id.Remove(0, "font-".Length).ToLowerInvariant();
                if (SKColor.TryParse(rem, out var clr))
                {
                    return Clone(FixedFont,
                        delegate(SKPaint x) {
                            x.TextSize = size;
                            x.Color    = clr;
                        });
                }
                if (name.TryGetValue(rem, out var clr2))
                {
                    return Clone(FixedFont,
                        delegate(SKPaint x) {
                            x.TextSize = size;
                            x.Color    = clr2;
                        });
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
        
            
        internal SKColor bg = SKColor.Parse("#333");
        internal SKPaint debug =  new SKPaint { TextSize = 10, Color = SKColor.Parse("#ffffff")};

       
        public SKPaint hex = new SKPaint()
        {
            TextSize = 15,
            Color    = SKColors.Yellow
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
