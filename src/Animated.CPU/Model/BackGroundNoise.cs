using System;
using System.Linq;
using Animated.CPU.Animation;
using SkiaSharp;

namespace Animated.CPU.Model
{
    public class BackGroundNoise : Element<Scene>
    {
        static Random r = new Random();

        public BackGroundNoise(Scene scene, DBlock b) : base(scene, b)
        {
            Bounds     = b.Inner;
            Speed      = new SKPoint(r.Next(-30, 30) / 30f, r.Next(-30, 30) / 30f);
            Location   = new SKPoint(r.Next(0, (int)b.W), r.Next(0, (int)b.H));

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

        public SKPaint Paint  { get; set; }
        public SKPaint Paint2 { get; set; }
        public IRect   Bounds { get; set; }
        
        public SKPoint Location { get; set; }
        public SKPoint Speed    { get; set; }


        public override void Init(SKSurface surface)
        {
            
        }
        public override void Step(TimeSpan step)
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
        }
        
        public override void Draw(SKSurface surface)
        {
            surface.Canvas.DrawCircle(Location.X, Location.Y, 3, Paint);

            foreach (var e in this.Parent.Children.Where(x=>x is BackGroundNoise).Cast<BackGroundNoise>())
            {
                if (e == this) continue;

                var distance = SKPoint.Distance(this.Location, e.Location);
                if (distance < 150)
                {
                    var i = (byte)Drawing.Scale(distance/150f, 255, 20);
                    Paint2.Color = new SKColor(i,i,i);
                    surface.Canvas.DrawLine(e.Location, this.Location, Paint2);
                }

            }
        }
    }
}