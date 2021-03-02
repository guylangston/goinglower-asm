using System;
using System.Collections.Generic;
using SkiaSharp;

namespace Animated.CPU.Animation
{
    
    
    public enum DOrient
    {
        Horz,
        Vert
    }
    
    
    public enum BlockAnchor
    {
        TL, TM, TR,
        ML, MM, MR,
        BL, BM, BR
    }
    

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



        public float Top    { get; }
        public float Bottom { get; }
        public float Left   { get; }
        public float Right  { get; }

        public float All => Math.Max(Top, Math.Max(Bottom, Math.Max(Left, Right)));
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

        public float    Top    { get; }
        public float    Bottom { get; }
        public float    Left   { get; }
        public float    Right  { get; }
        public SKPaint? Style  { get; }

    }

    
    public class DBlockProps
    {
        public DBorder Border  { get; set; }
        public DBorder Padding { get; set; }
        public DBorder Margin  { get; set; } 
        
        public DBlockProps()
        {

        }

        public DBlockProps(DBlockProps copy)
        {
            Margin  = new DBorder(copy.Margin);
            Border  = new DBorder(copy.Border);
            Padding = new DBorder(copy.Padding);
        }

        public DBlockProps Set(float margin, float border, float padding)
        {
            this.Margin  = new DBorder(margin);
            this.Border  = new DBorder(border);
            this.Padding = new DBorder(padding);
            return this;
        }
        
    }




    public class DBlock : DBlockProps
    {
        public DBlock()
        {
        }

        public DBlock(float x, float y, float w, float h)
        {
            X = x;
            Y = y;
            W = w;
            H = h;
        }

        public DBlock(DBlockProps copy) : base(copy)
        {
        }
        
        public float                       X      { get; set; }
        public float                       Y      { get; set; }
        public float                       W      { get; set; }
        public float                       H      { get; set; }
        
        public Rect Outer => new Rect(X, Y, W, H);

        public Rect Inner => new Rect(
            X + Padding.Left + Border.Left + Margin.Left,
            Y + Padding.Right + Border.Right + Margin.Right,
            W - (Padding.Left + Border.Left + Margin.Left) - (Padding.Right + Border.Right + Margin.Right),
            H - (Padding.Top + Border.Top + Margin.Top) - (Padding.Bottom + Border.Bottom + Margin.Bottom)
        );

        public DBlock Inset(float x, float y) => new DBlock(Inner.X + x, Inner.Y + y, Inner.W - x - x, Inner.H - y - y);


    }
    

    public class DText
    {
        public DText(string text, SKPaint? style = null)
        {
            Text  = text;
            Style = style;
        }

        public string    Text  { get; set; }
        public SKPaint?  Style { get; set; }
        public BlockAnchor Anchor  { get; set; }
    }

    

    


}
