using System;
using System.Linq;
using Animated.CPU.Animation;
using SkiaSharp;

namespace Animated.CPU.Model
{
    public class ParticleSystemElement : ElementBase
    {
        static Random Random = new Random();

        public ParticleSystemElement(IElement parent, DBlock? b) : base(parent, b)
        {
        }

        protected override void Init()
        {
            base.Init();
            for (int i = 0; i < 50; i++)
            {
                var p = Add(new ParticleElement(this, new SKPaint()
                {
                    Style = SKPaintStyle.Fill,
                    Color = SKColors.YellowGreen
                }));
                p.Location = new SKPoint(Random.Next((int)Block.X, (int)Block.X2), Random.Next((int)Block.Y, (int)Block.Y2));
                p.Size =  p.Mass     = Random.Next(2, 12) * 3f;
                //p.Speed = new SKPoint(Random.Next(-5, 5), Random.Next(-5, 5));
                
            }
            
        }

        private const float G = 300f;

        protected override void Step(TimeSpan step)
        {
            var   all = ChildrenAre<ParticleElement>().ToArray();
            
            foreach (var a in all)
            {
                float fx = 0, fy = 0;
                foreach (var b in all)
                {
                    if (a == b) continue;
                    
                    var dx = b.Location.X - a.Location.X;
                    var dy = b.Location.Y - a.Location.Y;
                    var d  = MathF.Sqrt(dx * dx + dy * dy);

                    if (d < 0.001)
                    {
                        continue;
                    }
                     
                    // F = G.M1.M1/r*r
                    // https://en.wikipedia.org/wiki/Newton%27s_law_of_universal_gravitation
                    var rr = MathHelper.DistanceSq(a.Location, b.Location);
                    
                    var sc = G * a.Mass * b.Mass / rr;
                    
                    

                    fx += sc * dx / d;
                    fy += sc * dy / d;
                }

                var m           = 5f;
                if (fx > m) fx  = m;
                if (fx < -m) fx = -m;
                if (fy > m) fy  = m;
                if (fy < -m) fy = -m;
                
                a.Force = new SKPoint(fx, fy);
            }
        }

       
        protected override void Draw(DrawContext surface)
        {
            
        }

        public void Leave(ParticleElement p)
        {
           p.Location = new SKPoint(Random.Next((int)Block.X, (int)Block.X2), Random.Next((int)Block.Y, (int)Block.Y2));
        }
    }

    public class ParticleElement : ElementFixedParent<ParticleSystemElement>
    {
        public ParticleElement(ParticleSystemElement parent, SKPaint paint) : base(parent, null)
        {
            Paint = paint;
        }
        
        public SKPaint Paint        { get; set; }
        public SKPoint Location     { get; set; }
        public SKPoint Speed        { get; set; }
        public SKPoint Acceleration { get; set; }
        public SKPoint Force        { get; set; }
        public float   Mass         { get; set; } = 5f;
        public float   Size         { get; set; }

        protected override void Step(TimeSpan step)
        {
            Acceleration = new SKPoint(Force.X / Mass, Force.Y / Mass);  // F = MA; A = F/M
            Speed        = new SKPoint(Speed.X + Acceleration.X * (float)step.TotalSeconds, 
                                      Speed.Y + Acceleration.Y * (float)step.TotalSeconds);
            Location     = new SKPoint( Location.X + Speed.X * (float)step.TotalSeconds, 
                                    Location.Y + Speed.Y * (float)step.TotalSeconds);
            Block        = new DBlock(Location.X, Location.Y, Size, Size);
            
            if (!Parent.Block.Contains(Location))
            {
                Parent.Leave(this);
            }

            
        }

        protected override void Draw(DrawContext surface)
        {
            surface.Canvas.DrawCircle(Location.X, Location.Y, Block.W, Paint);    
        }
    }
}