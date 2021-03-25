using System;
using System.Runtime.Intrinsics.X86;
using SkiaSharp;

namespace GoingLower.Core.Primitives
{
    
    public interface IDRect
    {
        public float X { get; set; }
        public float Y { get; set; }
        
        public float W { get; set; }
        public float H { get; set; }
       
        // Calculated
        public float   X2 => X + W;
        public float   Y2 => Y + H;
        public float   XC => X + W/2;
        public float   YC => Y + H/2;
        public SKPoint TL => new SKPoint(X, Y);
        public SKPoint TM => new SKPoint(XC, Y);
        public SKPoint TR => new SKPoint(X2, Y);
        public SKPoint ML => new SKPoint(X, YC);
        public SKPoint MM => new SKPoint(XC, YC);
        public SKPoint MR => new SKPoint(X2, YC);
        public SKPoint BL => new SKPoint(X, Y2);
        public SKPoint BM => new SKPoint(XC, Y2);
        public SKPoint BR => new SKPoint(X2, Y2);
    }
    
    public struct DRect : IDRect
    {
        public DRect(float x, float y, float w, float h)
        {
            X = x;
            Y = y;
            W = w;
            H = h;
        }

        public DRect(SKPoint a, SKPoint b)
        {
            if (a.X < b.X)
            {
                X = a.X;
                W = b.X - a.X;
            }
            else
            {
                X = b.X;
                W = a.X - b.X;
            }
            
            if (a.Y < b.Y)
            {
                Y = a.Y;
                H = b.Y - a.Y;
            }
            else
            {
                Y = b.Y;
                H = a.Y - b.Y;
            }
        }

        public float X { get; set; }
        public float Y { get; set; }
        public float W { get; set; }
        public float H { get; set; }
        
        public float   X2   => X + W;
        public float   Y2   => Y + H;
        public float   XC   => X + W/2;
        public float   YC   => Y + H/2;
        public SKPoint TL   => new SKPoint(X, Y);
        public SKPoint TM   => new SKPoint(XC, Y);
        public SKPoint TR   => new SKPoint(X2, Y);
        public SKPoint ML   => new SKPoint(X, YC);
        public SKPoint MM   => new SKPoint(XC, YC);
        public SKPoint MR   => new SKPoint(X2, YC);
        public SKPoint BL   => new SKPoint(X, Y2);
        public SKPoint BM   => new SKPoint(XC, Y2);
        public SKPoint BR   => new SKPoint(X2, Y2);
        public SKSize  Size => new SKSize(W, H);

        public SKRect ToSkRect() => new SKRect(X, Y, X2, Y2);
        public SKRectI ToSkRectI() => new SKRectI((int)X, (int)Y, (int)X2, (int)Y2);

        public SKPoint this[BlockAnchor anchor]
            => anchor switch
            {
                BlockAnchor.TL => TL,
                BlockAnchor.TM => TM,
                BlockAnchor.TR => TR,
                BlockAnchor.ML => ML,
                BlockAnchor.MM => MM,
                BlockAnchor.MR => MR,
                BlockAnchor.BL => BL,
                BlockAnchor.BM => BM,
                BlockAnchor.BR => BR,
                _ => MM
            };


        public DRect Inset(float x, float y)
        {
            return new DRect(X + x, Y + y, W - x - x, H - y - y);
        }
        
        public DRect Outset(float x, float y)
        {
            return new DRect(X - x, Y - y, W + x + x, H + y + y);
        }
    }
        
    
}