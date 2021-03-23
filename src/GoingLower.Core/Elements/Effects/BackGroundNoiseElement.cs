using System;
using System.Linq;
using GoingLower.Core.Drawing;
using GoingLower.Core.Helpers;
using GoingLower.Core.Primitives;
using SkiaSharp;

namespace GoingLower.Core.Elements.Effects
{
    public class BackGroundNoiseElement : ElementBase
    {
        private DBlock bounds;

        public BackGroundNoiseElement(IElement parent, DBlock bounds) : base(parent, new DBlock()
        {
            W = 3,
            H = 3
        })
        {
            Bounds   = bounds.Inner;
            Speed    = RandomHelper.PointInRange(-1, 1, 30);
            Location = RandomHelper.WithIn(Bounds);

            Paint = new SKPaint()
            {
                Style = SKPaintStyle.Fill,
                Color = SKColor.Parse("#666")
            };
            Paint2 = new SKPaint()
            {
                Style = SKPaintStyle.Fill,
                Color = SKColor.Parse("#444")
            };
        }

        public SKPaint Paint    { get; set; }
        public SKPaint Paint2   { get; set; }
        public IDRect   Bounds   { get; set; }
        public SKPoint Location { get; set; }
        public SKPoint Speed    { get; set; }

        protected override void Step(TimeSpan step)
        {
            Location += Speed;
            if (Location.X < 0)
            {
                Location = new SKPoint(0, Location.Y);
                Speed    = new SKPoint(Math.Abs(Speed.X), Speed.Y);
            }
            if (Location.X > Bounds.X2)
            {
                Location = new SKPoint(Bounds.X2, Location.Y);
                Speed    = new SKPoint(-Math.Abs(Speed.X), Speed.Y);
            }
            
            if (Location.Y < 0)
            {
                Location = new SKPoint(Location.X, 0);
                Speed    = new SKPoint(Speed.X, Math.Abs(Speed.Y));
            }
            if (Location.Y > Bounds.Y2)
            {
                Location = new SKPoint(Location.X, Bounds.Y2);
                Speed    = new SKPoint(Speed.X, -Math.Abs(Speed.Y));
            }

            Block.X = Location.X;
            Block.Y = Location.Y;
            
        }
        
        protected override void Draw(DrawContext surface)
        {
            surface.Canvas.DrawCircle(Location.X, Location.Y, Block.W, Paint);

            foreach (var e in this.Parent.Children.Where(x=>x is BackGroundNoiseElement).Cast<BackGroundNoiseElement>())
            {
                if (e == this) continue;

                var distance = SKPoint.Distance(this.Location, e.Location);
                if (distance < 150)
                {
                    var i = (byte)MathHelper.Scale(distance/150f, 255, 20);
                    Paint2.Color = new SKColor(i,i,i);
                    surface.Canvas.DrawLine(e.Location, this.Location, Paint2);
                }

            }
        }
    }

   
}