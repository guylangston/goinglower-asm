using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Animated.CPU.Animation;
using SkiaSharp;

namespace Animated.CPU.Model
{


    public class Scene : SceneBase<Cpu>
    {
        public Scene() : base(new StyleFactory())
        {
            Model = new Cpu();
        }
        

        protected override void InitScene(SKSurface surface)
        {
            Debug.WriteLine("Init");
            Console.WriteLine("Init2");
            
            var size = surface.Canvas.LocalClipBounds;
            var main = new DBlock()
            {
                X = 0,
                Y = 0,
                W = size.Width,
                H = size.Height
            };
            
            for (int cc = 0; cc < 100; cc++)
                Add(new BackGroundNoise(this, main));
            
            
            main.Set(10, 1, 4, new SKColor(100, 0, 100));
            
            var stack = new DStack(main, DOrient.Vert);
            var items = stack.Divide(new IElement[]
            {
                new ElementRegisterFile(this, Model.RegisterFile, null),
                new ALUElement(this, null),
                new MemoryViewElement(this, null, ExampleCPU.Build_Print_Rax()),
                new MemoryViewElement(this, null, new MemoryView(new []
                {
                    new MemoryView.Segment()
                    {
                        Source = "Hello World",
                        Raw    = ExampleCPU.RandomBytes(10)
                    }
                }))
            });

            foreach (var kid in items)
            {
                kid.model.Block = kid.block;
                Add(kid.model);
            }


        }
        
        protected override void DrawOverlay(SKSurface surface)
        {
            var canvas = surface.Canvas;
            canvas.DrawText($"{Steps} frames at {Elapsed.TotalSeconds:0.00} sec", 10, 10, StyleFactory.GetPaint(this, "debug"));
        }
        
        protected override void DrawBackGround(SKSurface surface)
        {
            var canvas = surface.Canvas;
            canvas.Clear(StyleFactory.GetColor(this, "bg"));

            Drawing d = new Drawing(surface.Canvas);
            //
            // foreach (var item in stack.Children)
            // {
            //     d.DrawRect(item);
            // }
            //
            
            // Highlight RIP
            // Arrow RIP -> Instruction
            // Highlight Instruction
            // Arrow Instration -> Decoder

        }

        
    }


}