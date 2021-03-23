using SkiaSharp;

namespace GoingLower.Core.Primitives
{
    public class DText
    {
        public DText(string text, SKPaint? style = null)
        {
            Text  = text;
            Style = style;
        }

        public string      Text   { get; set; }
        public SKPaint?    Style  { get; set; }
        public BlockAnchor Anchor { get; set; }
    }
}