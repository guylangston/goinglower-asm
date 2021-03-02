using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Animated.CPU.Animation;
using SkiaSharp;

namespace Animated.CPU.Model
{
    public class TestSection : Section<Scene, string>
    {
        public TestSection(IElement parent, string model, DBlock block) : base(parent, model, block)
        {
            Title = model;
        }

        
    }
        
    public class Scene : SceneBase<Cpu, StyleFactory>
    {
        public Scene() : base(new StyleFactory())
        {
        }

        protected override void InitScene(DrawContext surface)
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


            if (true)
            {
                for (int cc = 0; cc < 100; cc++)
                    Add(new BackGroundNoise(this, main));
            }


            var stack = Add(new StackElement(this, Block, DOrient.Horz));

            float w = Block.Inner.W / 4;
            stack.Add(new ElementRegisterFile(stack, Model.RegisterFile, DBlock.JustWidth(w).Set(20, 1, 10)));
            stack.Add(new ALUElement(stack, Model.ALU, DBlock.JustWidth(w).Set(20, 1, 10)));
            stack.Add(new MemoryViewElement(stack, DBlock.JustWidth(w).Set(20, 1, 10), Model.Instructions)
            {
                Title = "Instructions"
            });
            stack.Add(new TestSection(stack,  "Test", DBlock.JustWidth(w).Set(10, 1, 10)));
            //stack.Add(new MemoryViewElement(stack, DBlock.JustWidth(w).Set(10, 1, 10), Model.Stack));
            
            //
            // test = new TextBlockElement(this, this, new DBlock(1000, 1000, 500, 500), StyleFactory.FixedFont);
            // test.WriteLine("Hello World");
            // test.Write("Sample");
            // test.Write("Yellow", StyleFactory.FixedFontYellow);
            // test.Write("AndBackAgain(no spaces)");
            // test.WriteLine("]", StyleFactory.FixedFontCyan);
            // test.WriteLine("Hello World");
            // test.WriteLine("iiiiiiiiii");
            // test.WriteLine("XXXXXXXXXX");
            // test.WriteLine("[        ]");
            // test.WriteLine("         ]");
            // test.Write("    ");
            // test.WriteLine("     ]");
            // test.WriteLine("[         ");
            //
            // Add(test);



        }

        private TextBlockElement test;

        public override void StepScene(TimeSpan s)
        {
            Model.Step();
        }
        
        public SKPoint? Mouse { get; set; }

        protected override void DrawOverlay(DrawContext surface)
        {
            surface.DrawText($"{Steps} frames at {Elapsed.TotalSeconds:0.00} sec. {lastKey} | {Mouse}", 
                StyleFactory.GetPaint(this, "debug"),
                new SKPoint(0,0));
        }

        protected override void DrawBackGround(DrawContext surface)
        {
            var canvas = surface.Canvas;
            canvas.Clear(StyleFactory.GetColor(this, "bg"));
        }

        private string lastKey;

        public override void KeyPress(object platformKeyObject, string key)
        {
            lastKey = key;
            foreach (var register in Model.RegisterFile)
            {
                register.IsChanged = false;
            }
            if (Model?.Story == null) return;

            if (key == "n")
            {
                if (Model.Story.CurrentIndex < Model.Story.Steps.Count - 1)
                {
                    Model.Story.CurrentIndex++;
                }
            }
            if (key == "p")
            {
                if (Model.Story.CurrentIndex> 0)
                {
                    Model.Story.CurrentIndex--;
                }
            }
            var next = Model.Story.Steps[Model.Story.CurrentIndex];
            if (next != null)
            {
                foreach (var rd in next.Delta)
                {
                    if (rd.ValueString != null)
                    {
                        Model.SetReg(rd.Register, rd.ValueParsed.Value);
                    }
                }
            }
        }
    }
}