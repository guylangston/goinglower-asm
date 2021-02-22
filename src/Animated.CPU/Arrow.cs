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
        public bool    ShowHead  { get; set; }
        
        public SKPaint LabelStyle { get; set; }
        public string  LabelText  { get; set; }
        

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
                
            }
        }
    }
}