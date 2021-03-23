using System;
using System.Collections.Generic;
using SkiaSharp;

namespace GoingLower.Core.Primitives
{
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

        public DBlock(SKRect rect)
        {
            X = rect.Left;
            Y = rect.Top;
            W = rect.Width;
            H = rect.Height;
        }

        public DBlock(DBlockProps copy) : base(copy)
        {
        }

        public static DBlock FromTwoPoints(SKPoint a, SKPoint b)
        {
            var x1 = Math.Min(a.X, b.X);
            var x2 = Math.Max(a.X, b.X);
            var y1 = Math.Min(a.Y, b.Y);
            var y2 = Math.Max(a.Y, b.Y);

            return new DBlock(x1, y1, x2 - x1, y2 - y1);
        }

        public bool IsDefaultValue => X == 0 && X == 0 && W == 0 && H == 0;
        
        public float X  { get; set; }
        public float Y  { get; set; }
        public float Z  { get; set; }
        public float W  { get; set; }
        public float H  { get; set; }
        public float X2 => X + W;
        public float Y2 => Y + H;
        
        
        public bool Contains(SKPoint p) => X <= p.X && p.X <= X2 && 
                                           Y <= p.Y && p.Y <= Y2;
        public bool Contains(float pX, float pY) => X <= pX && pX <= X2 && 
                                           Y <= pY && pY <= Y2;

        public DRect Outer => new DRect(X, Y, W, H);

        public DRect Inner => new DRect(
            X + Padding.Left + Border.Left + Margin.Left,
            Y + Padding.Top + Border.Top + Margin.Top,
            W - (Padding.Left + Border.Left + Margin.Left) - (Padding.Right + Border.Right + Margin.Right),
            H - (Padding.Top + Border.Top + Margin.Top) - (Padding.Bottom + Border.Bottom + Margin.Bottom)
        );
        
        public DRect BorderDRect => new DRect(
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

        public SKPoint this[BlockAnchor anchor, bool inner]
        {
            get
            {
                var b = inner ? Inner : Outer;
                return b[anchor];
            }
        }


        public DBlock CreateRelative(BlockAnchor anchor, bool inner, SKPoint rel, SKPoint size)
        {
            var b = this[anchor, inner];
            return new DBlock(b.X + rel.X, b.Y + rel.Y, size.X, size.Y);
        }

        public void CenterAt(DBlock centerAt)
        {
            var c = centerAt.Inner.MM;
            X = c.X - W/2;
            Y = c.Y - H/2;
        }

        public void CenterAt(SKPoint centerAt)
        {
            X = centerAt.X - W/2;
            Y = centerAt.Y - H/2;
        }
    }
}
