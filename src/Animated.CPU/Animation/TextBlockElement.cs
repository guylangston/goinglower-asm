using System;
using System.Collections.Generic;
using System.Linq;
using SkiaSharp;

namespace Animated.CPU.Animation
{
    public class TextBlockElement : ElementBase
    {
        private readonly List<Line> lines;
        
        public TextBlockElement(IElement? parent, DBlock b, SKPaint defaultStyle) : base(parent.Scene, parent, b)
        {
            DefaultStyle = defaultStyle;
            lines        = new List<Line>();
            Clear();
            
            UpdateLineHeight();
        }

        private void UpdateLineHeight()
        {
            var m = new SKRect();
            DefaultStyle.MeasureText("X|", ref m);
            this.LineHeight = m.Height;
        }

        public float    LineHeight   { get; set; }
        public SKPaint  DefaultStyle { get; set; }
        public SKPaint? Background   { get; set; }

        public class Line
        {
            public List<Span> Spans   { get; }      = new List<Span>();
            public float      HeaderY { get; set; } = 1;
            public float      FooterY { get; set; } = 1;
        }

        public class Span
        {
            public Type    ModelType { get; set; }
            public object? Model     { get; set; }
            public string  Text      { get; set; }
            public SKPaint Style     { get; set; }
            public SKRect  Region    { get; set; }
            public float   Width     { get; set; }
            public SKPoint LastDraw  { get; set; }
        }

        string GetText(Type t, object val) => val?.ToString() ?? string.Empty;

        public Span Write<T>(T txt, SKPaint style = null /* Use Default */)
        {
            var span = new Span()
            {
                Model = txt,
                ModelType = typeof(T),
                Text = GetText(typeof(T), txt),
                Style = style ?? DefaultStyle
            };

            var r = new SKRect();
            span.Width = span.Style.MeasureText(span.Text, ref r);
            span.Region = r;
            
            lines[^1].Spans.Add(span);

            return span;
        }
        
        public void WriteLine()
        {
            lines.Add(new Line());
        }
        
        public Span WriteLine<T>(T txt, SKPaint style = null /* Use Default */)
        {
            var s = Write(txt, style);
            WriteLine();
            return s;
        }
        
        public void Clear()
        {
            lines.Clear();
            lines.Add(new Line());
        }

        protected override void Step(TimeSpan step)
        {
            
        }

        protected override void Draw(DrawContext surface)
        {
            surface.Canvas.Save();
            surface.Canvas.ClipRegion(new SKRegion(Block.Inner.ToSkRectI()));

            if (Background != null)
            {
                surface.Canvas.DrawRect(Block.Outer.ToSkRect(), Background);
            }

            UpdateLineHeight();
            
            float y = Block.Inner.Y + LineHeight;
            foreach (var line in lines)
            {
                float x = Block.Inner.X ; 
                float h = LineHeight;

                if (line.Spans.Any())
                {
                    y += line.HeaderY;
                    foreach (var span  in line.Spans)
                    {
                        span.LastDraw = new SKPoint(x, y);
                        surface.Canvas.DrawText(span.Text.ToString(), x, y, span.Style);
                        x += span.Width; // + span.Region.Width / span.Text.Length;
                        if (span.Region.Height > h) h = span.Region.Height;
                    }
                    y += h + line.FooterY;
                }
                else
                {
                    y += line.HeaderY;
                    
                    var b = new SKRect();
                    DefaultStyle.MeasureText("X", ref b);
                    
                    y += b.Height + line.HeaderY;
                }
            }
            
            surface.Canvas.Restore();
        }
    }
}