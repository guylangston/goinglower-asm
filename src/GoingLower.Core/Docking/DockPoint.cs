using GoingLower.Core.Elements;
using GoingLower.Core.Primitives;
using SkiaSharp;

namespace GoingLower.Core.Docking
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

        public IElement               Element     { get; }
        public TextBlockElement.Span? Span        { get; }
        public BlockAnchor            Anchor      { get; set; }
        public bool                   AnchorInner { get; set; }
        public SKPoint                Offset      { get; set; }// in the direction of the dock
        public SKPoint                Padding     { get; set; }
        public bool                   SpanToOuter { get; set; }

        public SKPoint GetDock()
        {
            if (Span is null)
            {
                return Element.Block[Anchor, AnchorInner] + Padding;
            }
            else
            {
                var outer = Element.Block[Anchor, AnchorInner];
                
                var tb    = new DBlock(Span.LastDrawRect);
                var inner = tb[Anchor, AnchorInner];

                if (SpanToOuter)
                {
                    return new SKPoint(outer.X, inner.Y) + Padding;      
                }
                return inner + Padding;
                
            }
        }
    }
}