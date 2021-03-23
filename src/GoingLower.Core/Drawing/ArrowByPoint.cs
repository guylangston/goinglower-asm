using GoingLower.Core.Primitives;
using GoingLower.CPU.Model;
using SkiaSharp;

namespace GoingLower.Core.Drawing
{


    public class ArrowByPoint
    {
        public SKPaint Style     { get; set; }
        public SKPoint Start     { get; set; }
        public SKPoint End       { get; set; }
        public SKPoint WayPointA { get; set; } = SKPoint.Empty;
        public SKPoint WayPointB { get; set; } = SKPoint.Empty;
        public bool    ShowHead  { get; set; } = true;

        public SKPaint LabelStyle { get; set; }
        public string  LabelText  { get; set; }

        public ArrowByPoint RelativeWayPoints(SKPoint relStart, SKPoint relEnd)
        {
            WayPointA = Start + relStart;
            WayPointB = End + relEnd;
            return this;
        }

        public void Draw(SKCanvas canvas)
        {
            
            if (!WayPointA.IsEmpty)
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
            //var l = new Line(a, b);
        }

        public static void DrawScaledLine(SKCanvas canvas, SKPoint a, SKPoint b, float scale, SKPaint style)
        {
            var l = new Line(a, b);

            canvas.DrawLine(a, new SKPoint(a.X + l.DX / l.Length * scale, a.Y + l.DY / l.Length * scale), style);
        }
    }
}