using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SkiaSharp;

namespace Animated.CPU.Animation
{
    public class TextBlockElement : ElementBase
    {
        private readonly List<Line> lines;
        
        public TextBlockElement(IElement? parent, DBlock b, SKPaint defaultStyle) : base(parent, b)
        {
            DefaultStyle = defaultStyle;
            lines        = new List<Line>();
            Clear();
            
            UpdateLineHeight();
        }

        public float    LastDrawHeight { get; set; } = -1;
        public float    LineHeight     { get; set; }
        public SKPaint  DefaultStyle   { get; set; }
        public SKPaint? Background     { get; set; }
        public bool     ClipEnabled    { get; set; } = true;
        public bool     Grow           { get; set; } = false;

        public class Line
        {
            public List<Span> Spans   { get; }      = new List<Span>();
            public float      HeaderY { get; set; } = 1;
            public float      FooterY { get; set; } = 1;
            public float      Y       { get; set; }
            public float      X       { get; set; }
            public float      H       { get; set; }
        }

        public class Span
        {
            public Type             ModelType  { get; set; }
            public object?          Model      { get; set; }
            public string           Text       { get; set; }
            public SKPaint          Style      { get; set; }
            public SKRect           TextSize     { get; set; }
            public float            Width      { get; set; }
            public SKPoint          LastDraw   { get; set; }
            public object?          Tag        { get; set; }
            public Action<Drawing>? CustomDraw { get; set; }
            public Line?            Line       { get; set; }
            public string           Url        { get; set; }

            public SKRect LastDrawRect 
                => Line == null ? SKRect.Empty 
                    : new SKRect(
                        LastDraw.X, 
                        LastDraw.Y - Line.H, 
                        LastDraw.X + TextSize.Width, 
                        LastDraw.Y + Line.FooterY );
            
            public Span SetTag(object tag)
            {
                Tag = tag;
                return this;
            }
            
            public Span SetModel(object model)
            {
                Model = model;
                return this;
            }

            public override string ToString() => $"Span(L:{3})";
        }
        
        void UpdateLineHeight()
        {
            var m = new SKRect();
            DefaultStyle.MeasureText("X|", ref m);
            this.LineHeight = m.Height;
        }

        string GetText(Type t, object val) => val?.ToString() ?? string.Empty;

        public Span Write<T>(T txt, SKPaint style = null /* Use Default */)
        {
            var t = GetText(typeof(T), txt);
            if (t == null) return null;
            if (t.Contains("\n"))       // Multi-Line text?
            {
                Span? last = null;
                foreach (var ll in StringHelper.ToLines(t))
                {
                    var s = WriteLine<string>(ll, style);
                    s.Model = (txt, ll);
                    last    = s;
                }
                return last;
            }
            
            var span = new Span()
            {
                Model = txt,
                ModelType = typeof(T),
                Text = GetText(typeof(T), txt),
                Style = style ?? DefaultStyle
            };

            var r = new SKRect();
            span.Width = span.Style.MeasureText(span.Text, ref r);
            span.TextSize = r;
            
            lines[^1].Spans.Add(span);
            
            // Did we clip LEFT?
            var draw = span.LastDrawRect;
            if (draw.Right > Block.Inner.X2)
            {
                // Left overflow, move to next line
                lines.Add(new Line());

            }

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
            UpdateLineHeight();
            float y     = Block.Inner.Y + LineHeight;
            float start = y;
            foreach (var line in lines)
            {
                line.Y = y;
                float x = line.X = Block.Inner.X ;
                
                float h =  LineHeight;

                if (line.Spans.Any())
                {
                    y += line.HeaderY;
                    foreach (var span  in line.Spans)
                    {
                        span.LastDraw = new SKPoint(x, y);
                        span.Line     = line;
                        
                        //surface.Canvas.DrawText(span.Text.ToString(), x, y, span.Style);
                        
                        x += span.Width; // + span.Region.Width / span.Text.Length;
                        if (span.TextSize.Height > h) h = span.TextSize.Height;
                    }
                    line.H =  h + line.FooterY; 
                    y      += h + line.FooterY;
                }
                else
                {
                    y += line.HeaderY;
                    
                    var b = new SKRect();
                    DefaultStyle.MeasureText("X", ref b);
                    
                    y      += b.Height + line.FooterY;
                    line.H =   b.Height + line.HeaderY + line.FooterY;
                }
            }
            LastDrawHeight = y - start;

            if (Grow)
            {
                Block.H = LastDrawHeight;
            }
        }

        public override PointSelectionResult? GetSelectionAtPoint(SKPoint p)
        {
            var s = base.GetSelectionAtPoint(p);
            if (s != null)
            {
                foreach (var line in lines)
                {
                    foreach (var span in line.Spans)
                    {
                        if (span.LastDrawRect.Contains(p))
                        {
                            s.Selection = span;
                            return s;
                        }
                    }
                }
            }
            return s;
        }

        public bool TryGetSpanFromModel<T>(T model, out Span s)
        {
            foreach (var line in lines)
            {
                foreach (var span in line.Spans)
                {
                    if (span.Model is T mt &&  mt.Equals(model))
                    {
                        s = span;
                        return true;
                    }
                    
                }
            }
            s = default;
            return false;
        }

       

        protected override void Draw(DrawContext surface)
        {
            if (Block == null) throw new NullReferenceException("Block");

            if (ClipEnabled)
            {
                surface.Canvas.Save();
                surface.Canvas.ClipRegion(new SKRegion(Block.Inner.ToSkRectI()));    
            }
            
            if (Background != null)
            {
                surface.Canvas.DrawRect(Block.Outer.ToSkRect(), Background);
            }

            foreach (var line in lines)
            {
                if (line.Spans.Any())
                {
                    foreach (var span  in line.Spans)
                    {
                        surface.Canvas.DrawText(span.Text.ToString(), span.LastDraw.X, span.LastDraw.Y, span.Style);
                    }
                }
            }

            if (ClipEnabled)
            {
                surface.Canvas.Restore();    
            }
            
        }

        protected override void Decorate(DrawContext surface)
        {
            foreach (var line in lines)
            {
                foreach (var lineSpan in line.Spans)
                {
                    if (lineSpan.CustomDraw != null)
                    {
                        lineSpan.CustomDraw(surface);
                    }
                }
            }
        }

        public float CalcHeight() => lines.Count * LineHeight;

        public void Resize()
        {
            Block.H = CalcHeight();
        }
    }
}