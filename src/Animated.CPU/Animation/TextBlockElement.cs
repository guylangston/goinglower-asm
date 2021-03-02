using System;
using System.Collections.Generic;
using System.Linq;
using SkiaSharp;

namespace Animated.CPU.Animation
{
    public class TextBlockElement : ElementBase
    {
        private readonly List<List<Span>> lines;
        
        
        public TextBlockElement(IScene scene, IElement? parent, DBlock b, SKPaint defaultStyle) : base(scene, parent, b)
        {
            DefaultStyle = defaultStyle;
            lines        = new List<List<Span>>();
            Clear();

        }

        public SKPaint DefaultStyle { get; set; }

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
            
            lines[^1].Add(span);

            return span;
        }
        
        public void WriteLine()
        {
            lines.Add(new List<Span>());
        }
        
        public Span WriteLine(string txt, SKPaint style = null /* Use Default */)
        {
            var s = Write(txt, style);
            WriteLine();
            return s;
        }
        
        public void Clear()
        {
            lines.Clear();
            lines.Add(new List<Span>());
        }

        protected override void Step(TimeSpan step)
        {
            
        }

        protected override void Draw(DrawContext surface)
        {
            float y = Block.Inner.Y;
            foreach (var line in lines)
            {
                float x = Block.Inner.X; 
                float h = 0;
                foreach (var span  in line)
                {
                    span.LastDraw = new SKPoint(x, y);
                    surface.Canvas.DrawText(span.Text.ToString(), x, y, span.Style);
                    x += span.Width; // + span.Region.Width / span.Text.Length;
                    if (span.Region.Height > h) h = span.Region.Height;
                }
                y += h + 2;
            }
        }
    }
}