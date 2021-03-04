using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Reflection.Metadata;
using Animated.CPU.Animation;
using Animated.CPU.Model;
using SkiaSharp;

namespace Animated.CPU
{
    public class Drawing
    {
        private SKCanvas canvas;

        public Drawing(SKCanvas canvas)
        {
            this.canvas = canvas;
        }

        public SKCanvas Canvas => canvas;

        public void DrawText(string txt, SKPaint t1, SKPoint c)
        {
            SKRect bounds = new SKRect();
            t1.MeasureText(txt, ref bounds);

            canvas.DrawText(txt, c.X, c.Y + bounds.Height, t1);
        }
        public SKRect DrawText(string txt, SKPaint t1, DBlock b, BlockAnchor anchor)
            => DrawText(txt, t1, b, anchor, SKPoint.Empty);

        // https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/graphics/skiasharp/basics/text
        public SKRect DrawText(string txt, SKPaint t1, DBlock b, BlockAnchor anchor, SKPoint offset)
        {
            
            
            SKRect bounds = new SKRect();
            if (b == null || string.IsNullOrWhiteSpace(txt)) return bounds;

            
            t1.MeasureText(txt, ref bounds);
            SKPoint c;
            switch (anchor)
            {
                case BlockAnchor.TL:
                    c = b.Inner.TL + offset;
                    canvas.DrawText(txt, c.X, c.Y + bounds.Height, t1);
                    break;

                case BlockAnchor.ML:
                    c = b.Inner.ML + offset;
                    canvas.DrawText(txt, c.X, c.Y, t1);
                    break;

                case BlockAnchor.BL:
                    c = b.Inner.BL + offset;
                    canvas.DrawText(txt, c.X, c.Y - t1.TextSize * 0.2f, t1);
                    break;


                case BlockAnchor.TR:
                    c = b.Inner.TR + offset;
                    canvas.DrawText(txt, c.X - bounds.Width - t1.TextSize * 0.2f, c.Y + bounds.Height, t1);
                    break;

                case BlockAnchor.MR:
                    c = b.Inner.MR + offset;
                    canvas.DrawText(txt, c.X - bounds.Width, c.Y, t1);
                    break;
                
                case BlockAnchor.MM:
                    c = b.Inner.MM + offset;
                    canvas.DrawText(txt, c.X  - bounds.Width/ 2, c.Y - bounds.Top + bounds.Bottom, t1);
                    break;

                case BlockAnchor.BR:
                    c = b.Inner.BR + offset;
                    canvas.DrawText(txt, c.X - bounds.Width - t1.TextSize * 0.2f, c.Y - t1.TextSize * 0.2f, t1);
                    break;


                case BlockAnchor.TM:
                    c = b.Outer.TM + offset;
                    canvas.DrawText(txt, c.X - bounds.MidX / 2, c.Y - bounds.Top + bounds.Bottom, t1);
                    break;

                default:
                    c = b.Outer.MM + offset;
                    canvas.DrawText(txt, c.X, c.Y + bounds.Height, t1);
                    break;
            }
            return bounds;

        }


        public void DrawTextCenter(string txt, SKPaint t1, SKPoint c)
        {
            SKRect bounds = new SKRect();
            t1.MeasureText(txt, ref bounds);

            canvas.DrawText(txt,
                c.X - bounds.Width / 2,
                c.Y - bounds.Height / 2,
                t1);
        }

        public void DrawTextRight(string txt, SKPaint t1, SKPoint c)
        {
            SKRect bounds = new SKRect();
            t1.MeasureText(txt, ref bounds);

            canvas.DrawText(txt,
                c.X - bounds.Width,
                c.Y + bounds.Height,
                t1);
        }


        public void DrawRect(SKPaint p1, IRect rect) => canvas.DrawRect(rect.X, rect.Y, rect.W, rect.H, p1);

        public void DrawRect(DBlock b, SKPaint p)
        {
            if (b == null) throw new ArgumentNullException(nameof(b));
            if (p == null) throw new ArgumentNullException(nameof(p));
            
            canvas.DrawRect(b.X + b.Margin.Left,
                    b.Y + b.Margin.Top,
                    b.W - b.Margin.Left - b.Margin.Right - b.Border.Left - b.Border.Right,
                    b.H - b.Margin.Top - b.Margin.Bottom - b.Border.Left - b.Border.Right,
                    p);
        }

        public static void MeasureText(string txt, float x, float y, SKPaint p, float borderSize, out SKRect textRegion, out SKRect border)
        {
            var bounds = new SKRect();
            p.MeasureText(txt, ref bounds);

            var tx = x;
            var ty = y - bounds.Top;

            textRegion = new SKRect(x, y, x + bounds.Width, y + bounds.Height);


            border = new SKRect(textRegion.Left - borderSize, textRegion.Top - borderSize, textRegion.Right + borderSize + borderSize, textRegion.Bottom + borderSize + borderSize);
        }

        public static float Scale(float normalised, float a, float b) => a + (b - a) * normalised;
        
        public void DrawHighlight(SKRect r, SKPaint f, float s)
        {
            var p = new SKPath();
            p.FillType = SKPathFillType.EvenOdd;
            p.MoveTo(r.Left - s, r.Top - s);
            p.LineTo(r.Right + s, r.Top - s);
            p.LineTo(r.Right + s, r.Bottom + s);
            p.LineTo(r.Left - s, r.Bottom + s);
            p.LineTo(r.Left - s, r.Top - s);

            p.MoveTo(r.Left, r.Top);
            p.LineTo(r.Right, r.Top);
            p.LineTo(r.Right, r.Bottom);
            p.LineTo(r.Left, r.Bottom);
            p.LineTo(r.Left, r.Top);

            canvas.DrawPath(p, f);
        }


        public SKRect DrawTextAndBGAtTopLeft(string txt, SKPoint topLeft, SKPaint font, SKPaint bg, SKPoint padding)
        {
            if (string.IsNullOrWhiteSpace(txt)) return new SKRect();
            
            var txtSize = new SKRect();
            font.MeasureText(txt, ref txtSize);
            
            // BG
            var xx   = padding.X;
            var yy   = padding.Y;

            var background = new SKRect(
                topLeft.X-xx, 
                topLeft.Y-yy, 
                topLeft.X+xx + txtSize.Width,
                topLeft.Y+yy + txtSize.Height);
            canvas.DrawRect(background, bg);
            
            // Text
            canvas.DrawText(txt, topLeft + new SKPoint(0, txtSize.Height), font);

            return background;

        }
        
        public SKRect DrawTextAndBGAtTopMiddle(string txt, SKPoint topMiddle, SKPaint font, SKPaint bg, SKPoint padding)
        {
            if (string.IsNullOrWhiteSpace(txt)) return new SKRect();
            
            var txtSize = new SKRect();
            font.MeasureText(txt, ref txtSize);
            
            // BG
            var xx = padding.X;
            var yy = padding.Y;

            var background = new SKRect(
                topMiddle.X-xx - txtSize.Width/2, 
                topMiddle.Y-yy, 
                topMiddle.X+xx + txtSize.Width - txtSize.Width/2,
                topMiddle.Y+yy + txtSize.Height);
            canvas.DrawRect(background, bg);
            
            // Text
            canvas.DrawText(txt, topMiddle + new SKPoint(-txtSize.Width/2, txtSize.Height), font);

            return background;

        }
    }
}

