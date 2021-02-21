using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Animated.CPU.Animation;
using Animated.CPU.Model;
using SkiaSharp;

namespace Animated.CPU
{



    public class Drawing
    {
        private SKCanvas canvas;
        
        public Drawing(SKCanvas canvas)
        {
            this.canvas = canvas;
        }
        
        public void DrawText(string txt, SKPaint t1, SKPoint c)
        {
            SKRect bounds = new SKRect();
            t1.MeasureText(txt, ref bounds);
            
            canvas.DrawText(txt, c.X, c.Y + bounds.Height, t1);
        }
       
        
        public void DrawTextCenter(string txt, SKPaint t1, SKPoint c)
        {
            SKRect bounds = new SKRect();
            t1.MeasureText(txt, ref bounds);
            
            canvas.DrawText(txt, 
                c.X - bounds.Width /2, 
                c.Y - bounds.Height /2, 
                t1);
        }
        
        public void DrawTextRight(string txt, SKPaint t1, SKPoint c)
        {
            SKRect bounds = new SKRect();
            t1.MeasureText(txt, ref bounds);
            
            canvas.DrawText(txt, 
                c.X - bounds.Width, 
                c.Y + bounds.Height, 
                t1);
        }
        
        
        public void DrawRect(SKPaint p1, IRect rect) => canvas.DrawRect(rect.X, rect.Y, rect.W, rect.H, p1);

        public void DrawRect(DBlock b, SKPaint p)
        {
            canvas.DrawRect(b.X + b.Margin.Left, 
                    b.Y + b.Margin.Top, 
                    b.W - b.Margin.Left - b.Margin.Right - b.Border.Left - b.Border.Right , 
                    b.H - b.Margin.Top - b.Margin.Bottom - b.Border.Left - b.Border.Right,
                    p);
        }



        public static float Scale(float normalised, float a, float b) => a + (b - a) * normalised;
        
    }

   
    
}