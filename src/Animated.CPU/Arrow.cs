using System;
using Animated.CPU.Animation;
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
        public bool    ShowHead  { get; set; } = true;
        
        public SKPaint LabelStyle { get; set; }
        public string  LabelText  { get; set; }
        

        public void Draw(SKCanvas canvas)
        {
            if (false && !WayPointA.IsEmpty)
            {
                IRect rect = new Rect(Start, End);
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
                DrawHead(canvas, Start, End);
            }
        }

        public void DrawHead(SKCanvas canvas, SKPoint a, SKPoint b)
        {
            // var l = new Line(a, b);
            //
            // var p = new SKPoint(a.X + (b.X - a.X)/2f, 
            //     a.Y + (b.Y - a.Y)/2f);
            //
            //
            // const double cos = 0.866;
            // const double sin = 0.500;
            // SKPoint end1 = new SKPoint(
            //     (float)(p.X + (l.DX * cos + dy * -sin)),
            //     (float)(p.Y + (dx * sin + dy * cos)));
            // SKPoint end2 = new SKPoint(
            //     (float)(p.X + (dx * cos + dy * sin)),
            //     (float)(p.Y + (dx * -sin + dy * cos)));
            // canvas.DrawLine(b, end1, Style);
            // canvas.DrawLine(b, end2, Style);
        }

        public static void DrawScaledLine(SKCanvas canvas, SKPoint a, SKPoint b, float scale, SKPaint style)
        {
            var l = new Line(a, b);
            
            canvas.DrawLine(a, new SKPoint(a.X + l.DX/l.Length*scale, a.Y + l.DY/l.Length*scale), style);
        }
        
        
            
    }

    public readonly struct Line
    {
        public Line(SKPoint a, SKPoint b)
        {
            A = a;
            B = b;

            DX = a.X - b.X;
            DY = a.Y - b.Y;

            Length = (float)Math.Sqrt(DX * DX + DY * DY);
        }

        public SKPoint A { get;  }
        public SKPoint B { get;  }
        
        public float DX { get;  }
        public float DY { get;  }
        public float Length { get;  }
    }
}