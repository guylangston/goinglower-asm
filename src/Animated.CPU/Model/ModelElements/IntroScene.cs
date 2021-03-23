using System;
using System.Collections.Generic;
using Animated.CPU.Animation;
using SkiaSharp;

namespace Animated.CPU.Model
{
    public class IntroScene : SimpleSceneBase
    {
        private SKPaint font;
        private SKPaint fontShadow;
        private SKPaint fontTitle;
    
        public IntroScene(string name, IStyleFactory styleFactory, DBlock b) : base(name, styleFactory, b)
        {
            this.font = new SKPaint()
            {
                TextSize = 20,
                Color    = SKColors.LightSeaGreen,
                Typeface = SKTypeface.FromFamilyName(
                    Theme.Sans, 
                    SKFontStyleWeight.Normal, 
                    SKFontStyleWidth.Normal, 
                    SKFontStyleSlant.Upright)
            };
            this.fontShadow = new SKPaint()
            {
                TextSize = 20,
                Color    = new SKColor(SKColors.LightSeaGreen.Red, SKColors.LightSeaGreen.Green, SKColors.LightSeaGreen.Blue, 100),
                Typeface = SKTypeface.FromFamilyName(
                    Theme.Sans,  
                    SKFontStyleWeight.Normal, 
                    SKFontStyleWidth.Normal, 
                    SKFontStyleSlant.Upright)
            };

            this.fontTitle = StyleFactory.GetPaint(this, "h1")
                                         .CloneAndUpdate(x => x.TextSize *= 2);

        }

        public StackElement LogoChars { get; set; }
        const string logoTxt   =  "ØxGoingLower";
        const string strapLine = "Build a deeper intuition for what our compiler/platform does for us";
        public string Title { get; set; } = "[0x01] C# to Binary Code: The Basics";

        protected override void DrawOverlay(DrawContext drawing)
        {
             var inner = Block.Inset(100, 100);
           
            
            drawing.DrawText(strapLine, StyleFactory.GetPaint(this, "h1"), inner, BlockAnchor.BM);
            
            drawing.DrawText(Title, fontTitle, inner, BlockAnchor.BM, new SKPoint(0,-200));
            
            drawing.DrawText(PresentationSceneMaster.Version, StyleFactory.GetPaint(this, "FixedFontBlue") , Block, BlockAnchor.BR);
            
        }
        
        
        
        protected override void InitScene()
        {
            for (int cc = 0; cc < 60; cc++)
                Add(new BackGroundNoiseElement(this, Block));

            
            var inner = Block.Inset(100, 500);
            this.LogoChars = Add(new StackElement(this, inner, DOrient.Horz, StackMode.JustLayout));
            
            
            const float   sizeS = 300f;
            const float   sizeE = 100f;
            for (int i = 0; i < logoTxt.Length; i++)
            {
                var c = logoTxt[i].ToString();
            
                var norm = 1f / logoTxt.Length;

                var s = Drawing.Scale(norm * i, sizeS, sizeE);
                var so = Drawing.Scale(norm * i, 10, 5);
                 
                var el = LogoChars.Add(new StringElement(LogoChars, new DBlock())
                {
                    Text          = c,
                    Anchor        = LineAnchor.Middle,
                    Style         = font.CloneAndUpdate(x=>x.TextSize  = s),
                    Shaddow       = fontShadow.CloneAndUpdate(x=>x.TextSize = s),
                    ShaddowOffset = new SKPoint(so, so),
                });
                el.Size = new PropFloat(el.Style.TextSize);

                el.Animator = new AnimatorPipeline(TimeSpan.FromHours(50))
                {
                    Loop = true
                };
                var timeScale = 0.2;
                el.Animator.Add(new AnimationDelay(TimeSpan.FromSeconds(i*timeScale)));
                el.Animator.Add(new AnimationProp(el.Size, el.Size.BaseValue, el.Size.BaseValue+20, TimeSpan.FromSeconds(timeScale)));
                el.Animator.Add(new AnimationDelay(TimeSpan.FromSeconds(((float)logoTxt.Length-i)*timeScale)));
                el.Animator.Add(new AnimationProp(el.Size, el.Size.BaseValue+20, el.Size.BaseValue, TimeSpan.FromSeconds(timeScale)));
                el.Animator.Start();

            }
        }

        public override void StepScene(TimeSpan s)
        {
            base.StepScene(s);
        }
    }
}