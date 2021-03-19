using Animated.CPU.Animation;
using SkiaSharp;

namespace Animated.CPU.Model
{
    public class IntroScene : SimpleSceneBase
    {
        private SKPaint font;
        private SKPaint font2;

        public IntroScene(string name, IStyleFactory styleFactory, DBlock b) : base(name, styleFactory, b)
        {
            this.font = new SKPaint()
            {
                TextSize = 20,
                Color    = SKColors.LightSeaGreen,
                Typeface = SKTypeface.FromFamilyName(
                    "Ubuntu", 
                    SKFontStyleWeight.Normal, 
                    SKFontStyleWidth.Normal, 
                    SKFontStyleSlant.Upright)
            };
            this.font2 = new SKPaint()
            {
                TextSize = 20,
                Color    = new SKColor(SKColors.LightSeaGreen.Red, SKColors.LightSeaGreen.Green, SKColors.LightSeaGreen.Blue, 100),
                Typeface = SKTypeface.FromFamilyName(
                    "Ubuntu", 
                    SKFontStyleWeight.Normal, 
                    SKFontStyleWidth.Normal, 
                    SKFontStyleSlant.Upright)
            };

        }
        
        const string logoTxt   = "ØxGoingLower";
        const string strapLine = "Build your intuition for what the compiler does for us";

        protected override void DrawOverlay(DrawContext drawing)
        {
            var inner = Block.Inset(100, 100);
            drawing.DrawRect(inner, StyleFactory.GetPaint(this, "bg"));
            
            var           x     = 130f;
            var           y     = 450f;
            const float   sizeS = 350f;
            const float   sizeE = 100f;
            for (int i = 0; i < logoTxt.Length; i++)
            {
                var c = logoTxt[i].ToString();

                var norm = 1f / logoTxt.Length;
                font2.TextSize = font.TextSize = Drawing.Scale(norm * i, sizeS, sizeE);


                var bounds = new SKRect();
                font.MeasureText(c, ref bounds);
                
                drawing.Canvas.DrawText(c, x+10, y+10, font2);
                drawing.Canvas.DrawText(c, x, y, font);

                x += bounds.Width + bounds.Left ;
            }

            
            drawing.DrawText(strapLine, StyleFactory.GetPaint(this, "h1"), inner, BlockAnchor.BM);
            
            
            drawing.DrawText(PresentationSceneMaster.Version, StyleFactory.GetPaint(this, "FixedFontBlue") , Block, BlockAnchor.BR);
            
        }
        
        protected override void InitScene()
        {
          
            for (int cc = 0; cc < 60; cc++)
                Add(new BackGroundNoiseElement(this, Block));
        }


    }
}