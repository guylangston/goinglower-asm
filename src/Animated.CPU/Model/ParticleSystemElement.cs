using System;
using System.Linq;
using Animated.CPU.Animation;
using SkiaSharp;

namespace Animated.CPU.Model
{
    public class ParticleSystemElement : ElementBase
    {
        private readonly int particleCount;
        static Random Random = new Random();
        public DBlock Bountry { get; set; }
        private const float G = 3000f;

        public ParticleSystemElement(IElement parent, DBlock? b, int particleCount) : base(parent, b)
        {
            this.particleCount = particleCount;
        }

        protected override void Init()
        {
            base.Init();
            Bountry = Block.Inset(-1000, -1000);
            for (int i = 0; i < particleCount; i++)
            {
                var p = Add(new ParticleElement(this, new SKPaint()
                {
                    Style       = SKPaintStyle.Fill,
                    StrokeWidth = 3,
                    Color       = new SKColor(90, 100, 150, 150)
                }));
                p.Location = new SKPoint(Random.Next((int)Block.X, (int)Block.X2), Random.Next((int)Block.Y, (int)Block.Y2));
                p.Mass     = Random.Next(2, 500) * 3f;
                p.Size     = p.Mass / 50 + 5;
                //p.Speed    = new SKPoint(Random.Next(-5, 5), Random.Next(-5, 5));
            }
            
        }

        public static void ScalarMax(ref float v, float absMax)
        {
            if (v > absMax) v = absMax;
            if (v < -absMax) v = -absMax;
        }
        
        public static void ScalarMax(ref SKPoint v, float absMax)
        {
            if (v.X > absMax) v.X  = absMax;
            if (v.X < -absMax) v.X = -absMax;
            if (v.Y > absMax) v.Y  = absMax;
            if (v.Y < -absMax) v.Y = -absMax;
        }



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

                    if (d < a.Size + b.Size)
                    {
                        d = a.Size + b.Size;
                    }
                     
                    // F = G.M1.M1/r*r
                    // https://en.wikipedia.org/wiki/Newton%27s_law_of_universal_gravitation
                    var rr = MathHelper.DistanceSq(a.Location, b.Location);
                    
                    var sc = G * a.Mass * b.Mass / rr;
                    
                    fx += sc * dx / d;
                    fy += sc * dy / d;
                }

                ScalarMax(ref fx, 5000f);
                ScalarMax(ref fy, 5000f);
                a.Force = new SKPoint(fx, fy);
                
            }
        }

       
        protected override void Draw(DrawContext surface)
        {
            
        }

        public void Leave(ParticleElement p)
        {
           p.Location = new SKPoint(Random.Next((int)Block.X, (int)Block.X2), Random.Next((int)Block.Y, (int)Block.Y2));
           p.Speed    = new SKPoint(Random.Next(-5, 5), Random.Next(-5, 5));
        }
    }

    public class ParticleElement : ElementFixedParent<ParticleSystemElement>
    {
        public ParticleElement(ParticleSystemElement parent, SKPaint paint) : base(parent, null)
        {
            Paint = paint;
        }
        
        public SKPaint Paint        ;
        public SKPoint Location     ;
        public SKPoint Speed        ;
        public SKPoint Acceleration ;
        public SKPoint Force        ;
        public float   Mass         ;
        public float   Size         ;

        protected override void Step(TimeSpan step)
        {
            Acceleration = new SKPoint(Force.X / Mass, Force.Y / Mass);  // F = MA; A = F/M
            ParticleSystemElement.ScalarMax(ref Acceleration, 50f);
            
            Speed        = new SKPoint(Speed.X + Acceleration.X * (float)step.TotalSeconds, 
                                      Speed.Y + Acceleration.Y * (float)step.TotalSeconds);
            ParticleSystemElement.ScalarMax(ref Speed, 50f);

            Location     = new SKPoint( Location.X + Speed.X * (float)step.TotalSeconds, 
                                    Location.Y + Speed.Y * (float)step.TotalSeconds);
            Block        = new DBlock(Location.X, Location.Y, Size, Size);
            
            if (!Parent.Bountry.Contains(Location))
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