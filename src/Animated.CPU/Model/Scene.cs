using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Animated.CPU.Animation;
using SkiaSharp;

namespace Animated.CPU.Model
{
    

    public class Scene : SceneBase<Cpu, StyleFactory>
    {
        public Scene() : base(new StyleFactory())
        {
        }
        
        // Helpers
        public Cpu Cpu => Model;

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

            float w     = Block.Inner.W / 4;
            var   stack = Add(new StackElement(this, Block, DOrient.Horz));
            stack.Add(new ElementRegisterFile(stack, Model.RegisterFile, DBlock.JustWidth(w).Set(20, 1, 10)));
            stack.Add(new ALUElement(stack, Model.ALU, DBlock.JustWidth(w).Set(20, 1, 10)));
            stack.Add(new MemoryViewElement(stack, DBlock.JustWidth(w).Set(20, 1, 10), Model.Instructions)
            {
                Title = "Instructions"
            });
            stack.Add(new CodeSection(stack,  "Code", DBlock.JustWidth(w).Set(10, 1, 10)));

            var dBlock = new DBlock(300, 1050, 900, 400);
            dBlock.Set(0, 3, 10);
            var term   = Add(new TerminalElement(this, new Terminal(), dBlock));
            
        }
        
        public override void StepScene(TimeSpan s)
        {
            Model.Step();
        }
        
        public SKPoint? Mouse { get; set; }

        protected override void DrawOverlay(DrawContext surface)
        {
            // surface.DrawText($"{FrameCount} frames at {Elapsed.TotalSeconds:0.00} sec. {lastKey} | {Mouse}", 
            //     Styles.GetPaint(this, "debug"),
            //     new SKPoint(0,0));
        }

        protected override void DrawBackGround(DrawContext surface)
        {
            var canvas = surface.Canvas;
            canvas.Clear(Styles.GetColor(this, "bg"));
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
                    if (rd.ValueRaw != null)
                    {
                        Model.SetReg(rd.Register, rd.ValueParsed.Value);
                    }
                }
            }
        }
    }
}