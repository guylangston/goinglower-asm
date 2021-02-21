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


        public DBorder(DBorder copy) 
        {
            this.Top    = copy.Top;
            this.Bottom = copy.Bottom;
            this.Left   = copy.Left;
            this.Right  = copy.Right;
        }



        public float Top    { get; private set; }
        public float Bottom { get; private set; }
        public float Left   { get; private set; }
        public float Right  { get; private set; }

        public void Set(float all)
        {
            Top    = all;
            Bottom = all;
            Left   = all;
            Right  = all;
        }
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
        
        public DBorderStyled(DBorderStyled copy) 
        {
            this.Top    = copy.Top;
            this.Bottom = copy.Bottom;
            this.Left   = copy.Left;
            this.Right  = copy.Right;
            this.Style  = copy.Style;
        }


        public float    Top    { get; private set;}
        public float    Bottom { get; private set;}
        public float    Left   { get; private set;}
        public float    Right  { get; private set;}
        public SKPaint? Style  { get; private set;}

        public void Set(float all, SKPaint? p = null)
        {
            Top    = all;
            Bottom = all;
            Left   = all;
            Right  = all;
            Style  = p;
        }
    }

    public class DDocument  // Root object
    {
        // Styles
    }
    
    
    public class DBlockProps
    {
        DBorderStyled border;
        DBorder padding;
        DBorder margin;
        
        public DBlockProps()
        {

        }

        public DBlockProps(DBlockProps copy)
        {
            margin = new DBorder(copy.margin);
            border = new DBorderStyled(copy.border);
            padding = new DBorder(copy.padding);
        }


        public DBorderStyled Border  => border;
        public DBorder       Padding => padding;
        public DBorder       Margin  => margin;

        public DBlockProps Set(float margin, float border, float padding, SKColor c)
        {
            this.margin.Set(margin);
            this.border.Set(border, new SKPaint()
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = border,
                Color = c,
            });
            this.padding.Set(padding);
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
        
        public float                       X      { get; set; }
        public float                       Y      { get; set; }
        public float                       W      { get; set; }
        public float                       H      { get; set; }
        public object                      Domain { get; set; }
        public IReadOnlyCollection<IDDock> Docks  { get; }
        

        public Rect Outer => new Rect(X, Y, W, H);

        public Rect Inner => new Rect(
            X + Padding.Left + Border.Left + Margin.Left,
            Y + Padding.Right + Border.Right + Margin.Right,
            W - (Padding.Left + Border.Left + Margin.Left) - (Padding.Right + Border.Right + Margin.Right),
            H - (Padding.Top + Border.Top + Margin.Top) - (Padding.Bottom + Border.Bottom + Margin.Bottom)
        );
        
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