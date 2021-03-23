using System;
using SkiaSharp;

namespace GoingLower.Core.Drawing
{
    public readonly struct Line
    {
        public Line(SKPoint a, SKPoint b)
        {
            A = a;
            B = b;

            DX = a.X - b.X;
            DY = a.Y - b.Y;

            Length = (float)Math.Sqrt(DX * DX + DY * DY);
        }

        public SKPoint A { get; }
        public SKPoint B { get; }

        public float DX     { get; }
        public float DY     { get; }
        public float Length { get; }
    }
}