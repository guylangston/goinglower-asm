using GoingLower.Core;
using SkiaSharp;

namespace GoingLower.Core.Docking
{
    public class DockedArrow
    {
        public DockedArrow(DockPoint start, DockPoint end, SKPaint style)
        {
            Start = start;
            End   = end;
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
                
                End.Anchor = BlockAnchor.ML;
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

            var a0 = Start.GetDock();
            var a1 = a0 + Start.Offset;

            var a3 = End.GetDock();
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
}