using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Animated.CPU.Animation;
using Animated.CPU.Model;
using SkiaSharp;

namespace Animated.CPU
{

   

    public class Arrow
    {
        public SKPaint Style     { get; set; }
        public SKPoint Start     { get; set; }
        public SKPoint End       { get; set; }
        public SKPoint WayPointA { get; set; } = SKPoint.Empty;
        public SKPoint WayPointB { get; set; } = SKPoint.Empty;
        public bool    ShowHead  { get; set; }
        
        public SKPaint LabelStyle    { get; set; }
        public string  LabelText    { get; set; }
        

        public void Draw(SKCanvas canvas)
        {
            
            
            
            if (!WayPointA.IsEmpty)
            {
                IRect rect = new Rect(Start, End);
                 WayPointA = rect.TM;
                 WayPointB = rect.BM;
                canvas.DrawLine(Start, WayPointA, Style);
                canvas.DrawLine(WayPointA, WayPointB, Style);
                canvas.DrawLine(WayPointB, End, Style);    
            }
            else
            {
                canvas.DrawLine(Start, End, Style);
            }
            
            
            if (LabelText is not null)
            {
                // Draw Text    
            }
            
            // Draw End
            if (ShowHead)
            {
                
            }
        }
    } 


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

        public void DrawRect(DBlock b)
        {
            if (b.BorderStyle != null)
                canvas.DrawRect(b.X + b.MarginLeft, 
                    b.Y + b.MarginTop, 
                    b.W - b.MarginLeft - b.MarginRight - b.BorderLeft - b.BorderRight , 
                    b.H - b.MarginTop - b.MarginBottom - b.BorderLeft - b.BorderRight,
                    b.BorderStyle);
        } 
            
        
        
    }

   
    
}