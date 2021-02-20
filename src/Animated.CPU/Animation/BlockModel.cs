using System.Collections.Generic;
using SkiaSharp;

namespace Animated.CPU.Animation
{

    public interface IBorder
    {
        float Top    { get; }
        float Bottom { get; }
        float Left   { get; }
        float Right  { get; }
    }

    public interface IBorderStyled : IBorder
    {
        public SKPaint Style { get; }

    }

    public struct DBorder : IBorder
    {
        public DBorder(float all)
        {
            Top    = all;
            Bottom = all;
            Left   = all;
            Right  = all;
        }

        public DBorder(float top, float right, float left, float bottom)
        {
            Top    = top;
            Bottom = bottom;
            Left   = left;
            Right  = right;
        }

        public float Top    { get; }
        public float Bottom { get; }
        public float Left   { get; }
        public float Right  { get; }
    }

    public struct DBorderStyled : IBorderStyled
    {
        public DBorderStyled(float all, SKPaint style)
        {
            Top    = all;
            Bottom = all;
            Left   = all;
            Right  = all;
            Style  = style;
        }

        public DBorderStyled(float top, float right, float left, float bottom, SKPaint style)
        {
            Top    = top;
            Bottom = bottom;
            Left   = left;
            Right  = right;
            Style  = style;
        }

        public float Top    { get; }
        public float Bottom { get; }
        public float Left   { get; }
        public float Right  { get; }

        public SKPaint Style { get; }
    }

    public class DDocument  // Root object
    {
        // Styles
    }
    
    
    public class DBlockProps
    {
        public DBlockProps()
        {

        }

        public DBlockProps(DBlockProps copy)
        {
            this.PaddingTop        = copy.PaddingTop;
            this.PaddingBottom     = copy.PaddingBottom;
            this.PaddingLeft       = copy.PaddingLeft;
            this.PaddingRight      = copy.PaddingRight;
        
            this.MarginTop         = copy.MarginTop;
            this.MarginBottom      = copy.MarginBottom;
            this.MarginLeft        = copy.MarginLeft;
            this.MarginRight       = copy.MarginRight;
            
            this.BorderTop         = copy.BorderTop;
            this.BorderBottom      = copy.BorderBottom;
            this.BorderLeft        = copy.BorderLeft;
            this.BorderRight       = copy.BorderRight;
        

            this.BorderStyle = copy.BorderStyle;
        }
        
        // DisplayProps

        // Convert to DBorder
        public float PaddingTop    { get; set; }
        public float PaddingBottom { get; set; }
        public float PaddingLeft   { get; set; }
        public float PaddingRight  { get; set; }

        // Convert to DBorder
        public float MarginTop    { get; set; }
        public float MarginBottom { get; set; }
        public float MarginLeft   { get; set; }
        public float MarginRight  { get; set; }

        // Convert to DBorder
        public float   BorderTop    { get; set; }
        public float   BorderBottom { get; set; }
        public float   BorderLeft   { get; set; }
        public float   BorderRight  { get; set; }
        public SKPaint BorderStyle  { get; set; }
        
        public DBlockProps Set(float margin, float border, float padding, SKColor c)
        {
            SetMargin(margin);
            SetBorder(border);
            SetPadding(padding);
            if (border > 0)
            {
                BorderStyle = new SKPaint()
                {
                    Style       = SKPaintStyle.Stroke,
                    StrokeWidth = border,
                    Color       = c
                };
            }

            return this;
        }

        public DBlockProps SetPadding(float all)
        {
            PaddingTop    = all;
            PaddingBottom = all;
            PaddingLeft   = all;
            PaddingRight  = all;
            return this;
        }

        public DBlockProps SetBorder(float all)
        {
            BorderTop    = all;
            BorderBottom = all;
            BorderLeft   = all;
            BorderRight  = all;
            return this;
        }

        public DBlockProps SetMargin(float all)
        {
            MarginTop    = all;
            MarginBottom = all;
            MarginLeft   = all;
            MarginRight  = all;
            return this;
        }
    }


    public class DBlock : DBlockProps
    {
        public DBlock()
        {
        }

        public DBlock(DBlockProps copy) : base(copy)
        {
        }


        public float X { get; set; }
        public float Y { get; set; }

        public float W { get; set; }
        public float H { get; set; }

        public Rect Outer => new Rect(X, Y, W, H);

        public Rect Inner => new Rect(
            X + PaddingLeft + BorderLeft + MarginLeft,
            Y + PaddingRight + BorderRight + MarginRight,
            W - (PaddingLeft + BorderLeft + MarginLeft) - (PaddingRight + BorderRight + MarginRight),
            H - (PaddingTop + BorderTop + MarginTop) - (PaddingBottom + BorderBottom + MarginBottom)
        );


        // State
        public object Domain { get; set; }
        
        public IReadOnlyCollection<IDDock> Docks { get; }

    }

    public interface IDDock
    {
        string Id    { get; set; }
        SKRect GetAnchor(DBlock owner);
    }

    public class DText
    {
        public DText(string text, SKPaint? style = null)
        {
            Text  = text;
            Style = style;
        }

        public string  Text  { get; set; }
        public SKPaint? Style { get; set; }
    }

    public class DPanel : DBlock
    {
        public DText   Title       { get; set; }
        public DText   Footer      { get; set; }
        
    }

    public enum DOrient
    {
        Vert,
        Horz
    }


}