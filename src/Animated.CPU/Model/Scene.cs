using Animated.CPU.Animation;
using SkiaSharp;

namespace Animated.CPU.Model
{
    public class Scene : SceneBase
    {
        public Scene()
        {
        }
        
        internal Cpu cpu = new Cpu();
        internal SKPaint p1 = new SKPaint()
        {
            Style       = SKPaintStyle.Stroke,
            Color       = new SKColor(255,0,0),
            StrokeWidth = 2
                    
        };
            
        internal SKColor bg = SKColor.Parse("#333");
        internal SKPaint b1 = new SKPaint()
        {
            Style       = SKPaintStyle.Stroke,
            Color       = new SKColor(200,200,200),
            StrokeWidth = 2
                    
        };
        internal SKPaint t1    =  new SKPaint { TextSize = 15, Color = SKColor.Parse("#00d0fa")};
        internal SKPaint t1a   =  new SKPaint { TextSize = 15, Color = SKColor.Parse("#00fa00")};
        internal SKPaint t2    =  new SKPaint { TextSize = 20, Color = SKColor.Parse("#00ff00")};
        internal SKPaint debug =  new SKPaint { TextSize = 10, Color = SKColor.Parse("#ffffff")};
        private DStack stack;

        public override void Init(SKSurface surface)
        {
            var size = surface.Canvas.LocalClipBounds;
            var main = new DBlock()
            {
                X = 0,
                Y = 0,
                W = size.Width,
                H = size.Height
            };

            main.Set(10, 1, 4, new SKColor(100, 0, 100));

            stack = new DStack(main, DOrient.Vert, new DBlockProps().Set(1, 1, 1, SKColor.Parse("#444")));
            stack.Divide(4);
            
            for (int cc = 0; cc < 100; cc++)
                Add(new BackGroundNoise(this, main));

            Add(new RegisterElement(this, stack.Children[0]));
            // 1 - CPU/FPU
            Add(new InstructionStack(this, stack.Children[2]));
            
            
            Add(new MemoryViewElement(this, stack.Children[3], ExampleCPU.Build_Print_Rax()));

            
            
        }
        
        protected override void DrawOverlay(SKSurface surface)
        {
            var canvas = surface.Canvas;
            canvas.DrawText($"{Steps} frames at {Elapsed.TotalSeconds:0.00} sec", 10, 10, debug);
        }
        
        protected override void DrawBackGround(SKSurface surface)
        {
            var canvas = surface.Canvas;
            canvas.Clear(bg);

            Drawing d = new Drawing(surface.Canvas);
            
            foreach (var item in stack.Children)
            {
                d.DrawRect(item);
            }

        }
    }


}