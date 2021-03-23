using System;
using GoingLower.Core.Primitives;
using SkiaSharp;

namespace GoingLower.Core.Helpers
{
    public static class RandomHelper
    {
        public static readonly Random Random = new Random();

        public static SKPoint WithIn(IDRect block) 
            => new SKPoint(Random.Next(0, (int)block.W), Random.Next(0, (int)block.H));
        
        public static float NewFloat(int a, int b, int scale) 
            => (float)Random.Next(a * scale, b * scale) / (float)scale;

        public static SKPoint PointInRange(int a, int b, int scale)
            => new SKPoint(NewFloat(a, b, scale), NewFloat(a, b, scale));
    }
}