using System;
using SkiaSharp;

namespace GoingLower.Core.Helpers
{
    public static class StyleHelper
    {
        public static SKPaint CloneAndUpdate(this SKPaint paint, Action<SKPaint> update)
        {
            var c = paint.Clone();
            update(c);
            return c;
        }
    }
}