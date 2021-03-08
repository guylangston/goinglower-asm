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

    public struct DBorder : IBorder
    {
        public DBorder(float all)
        {
            Top    = all;
            Bottom = all;
            Left   = all;
            Right  = all;
        }

        public DBorder(float top, float right, float bottom, float left)
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

        public bool IsDefaultValue => X == 0 && X == 0 && W == 0 && H == 0;
        
        public float X  { get; set; }
        public float Y  { get; set; }
        public float W  { get; set; }
        public float H  { get; set; }
        public float X2 => X + W;
        public float Y2 => Y + H;
        
        
        public bool Contains(SKPoint p) => X <= p.X && p.X <= X2 && 
                                           Y <= p.Y && p.Y <= Y2;
        public bool Contains(float pX, float pY) => X <= pX && pX <= X2 && 
                                           Y <= pY && pY <= Y2;

        public Rect Outer => new Rect(X, Y, W, H);

        public Rect Inner => new Rect(
            X + Padding.Left + Border.Left + Margin.Left,
            Y + Padding.Top + Border.Top + Margin.Top,
            W - (Padding.Left + Border.Left + Margin.Left) - (Padding.Right + Border.Right + Margin.Right),
            H - (Padding.Top + Border.Top + Margin.Top) - (Padding.Bottom + Border.Bottom + Margin.Bottom)
        );
        
        public Rect BorderRect => new Rect(
            X + Margin.Left,
            Y +  Margin.Top,
            W - (Margin.Left) - ( Margin.Right),
            H - (Margin.Top) - (Margin.Bottom)
        );

        public DBlock Inset(float x, float y) => new DBlock(Inner.X + x, Inner.Y + y, Inner.W - x - x, Inner.H - y - y);
        
        public new DBlock Set(float margin, float border, float padding)
        {
            base.Set(margin, border, padding);
            return this;
        }
        
        public DBlock Set(
            float marginTop,float marginRight, float marginBottom, float marginLeft,   
            float borderTop,float borderRight, float borderBottom, float borderLeft,
            float paddingTop,float paddingRight, float paddingBottom, float paddingLeft
            )
        {
            base.Margin = new DBorder(marginTop, marginRight, marginBottom, marginLeft);
            base.Border = new DBorder(borderTop, borderRight, borderBottom, borderLeft);
            base.Padding = new DBorder(paddingTop, paddingRight, paddingBottom, paddingLeft);
            return this;
        }

        public static DBlock JustWidth(float f) => new DBlock()
        {
            W = f
        };
    }

    public class DynamicDBlock : DBlock
    {
        public DynamicDBlock()
        {
        }

        public DynamicDBlock(float desiredWidth, float desiredHeight) : base(0,0, desiredWidth, desiredHeight)
        {
            DesiredWidth  = desiredWidth;
            DesiredHeight = desiredHeight;
        }

        public DynamicDBlock(float x, float y, float w, float h, float desiredWidth, float desiredHeight) : base(x, y, w, h)
        {
            DesiredWidth  = desiredWidth;
            DesiredHeight = desiredHeight;
        }

        public float DesiredWidth { get; set; }
        public float DesiredHeight { get; set; }
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
