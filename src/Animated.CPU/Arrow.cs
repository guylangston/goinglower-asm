using System;
using Animated.CPU.Animation;
using Animated.CPU.Model;
using SkiaSharp;

namespace Animated.CPU
{

    public class DockPoint
    {
        public DockPoint(IElement element)
        {
            Element = element;
            
        }

        public DockPoint(IElement element, TextBlockElement.Span? span)
        {
            Element = element;
            Span    = span;
        }

        public IElement               Element { get;  }
        public TextBlockElement.Span? Span    { get; }
        
        public BlockAnchor Anchor      { get; set; }
        public bool        AnchorInner { get; set; }
        public SKPoint     Offset           { get; set; } // in the direction of the dock
    }

    public class ArrowElement
    {
        public ArrowElement(DockPoint start, DockPoint end, SKPaint style)
        {
            Start      = start;
            End        = end;
            Style = style;
        }

        public DockPoint Start    { get; }
        public DockPoint End      { get; }
        public SKPaint   Style    { get; }
        public float     ArrowPop { get; set; } = 10;
        public bool      IsHidden { get; set; }


        public void Step()
        {
            if (Start.Element.Block == null || End.Element.Block == null)
            {
                IsHidden = true;
                return;
            }
            
            var bs = Start.Element.Block!;
            var be = End.Element.Block!;
            
            // Left -> Right
            if (bs.Outer.MM.X < be.Outer.MM.X)
            {
                Start.Anchor = BlockAnchor.MR;
                Start.Offset = new SKPoint(ArrowPop, 0);
                
                End.Anchor   = BlockAnchor.ML;
                End.Offset = new SKPoint(-ArrowPop, 0);
                
            }
            else
            {
                Start.Anchor = BlockAnchor.ML;
                Start.Offset = new SKPoint(-ArrowPop, 0);
                
                End.Anchor = BlockAnchor.MR;
                End.Offset = new SKPoint(ArrowPop, 0);
            }

        }

        public void Draw(SKCanvas canvas)
        {
            if (IsHidden) return;

            var a0 = Start.Element.Block[Start.Anchor, Start.AnchorInner];
            var a1 = a0 + Start.Offset;
            
            var a3 = End.Element.Block[End.Anchor, End.AnchorInner];
            var a2 = a3 + End.Offset;
            
            // TODO: Convert to path
            canvas.DrawLine(a0,a1, Style);
            canvas.DrawLine(a1,a2, Style);
            canvas.DrawLine(a2,a3, Style);


            var dx = 8;
            var dy = 5;
            if (a3.X > a2.X)
            {
                canvas.DrawLine(a3,a3 + new SKPoint(-dx, -dy), Style);
                canvas.DrawLine(a3,a3 + new SKPoint(-dx, +dy), Style);
            }
            else
            {
                canvas.DrawLine(a3,a3 + new SKPoint(+dx, -dy), Style);
                canvas.DrawLine(a3,a3 + new SKPoint(+dx, +dy), Style);
            }
            
            
        }
    }
    
    
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

        public Arrow RelativeWayPoints(SKPoint relStart, SKPoint relEnd)
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

        public SKPoint A { get; }
        public SKPoint B { get; }

        public float DX     { get; }
        public float DY     { get; }
        public float Length { get; }
    }
}