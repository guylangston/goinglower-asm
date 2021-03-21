using System;
using SkiaSharp;

namespace Animated.CPU.Model
{
    public static class MathHelper
    {
        public static float Distance(SKPoint a, SKPoint b)
        {
            var x = a.X - b.X;
            var y = a.Y - b.Y;
            return MathF.Sqrt(x * x + y * y);
        }
        
        public static float DistanceSq(SKPoint a, SKPoint b)
        {
            var x = a.X - b.X;
            var y = a.Y - b.Y;
            return x * x + y * y;
        }
    }
}