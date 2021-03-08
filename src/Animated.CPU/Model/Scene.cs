using System;
using System.Diagnostics;
using Animated.CPU.Animation;
using Animated.CPU.Model.ModelElements;
using SkiaSharp;

namespace Animated.CPU.Model
{
    public class Scene : SceneBase<Cpu, StyleFactory>
    {
        public Scene(DBlock region) : base(new StyleFactory())
        {
            Block = region;
        }
        
        // Helpers
        public Cpu                    Cpu                 => Model;
        public MemoryViewElement      ElementInstructions { get; set; }
        public CodeSection            ElementCode         { get; set; }
        public ElementRegisterFile    ElementRegisterFile { get; set; }
        public SKPoint?               Mouse               { get; set; }
        public Action<string, object> SendCommand         { get; set; }
        public ALUElement             ElementALU          { get; set; }
        public string                 LastKeyPress        { get; set; }


        protected override void InitScene()
        {
            if (false)
            {
                for (int cc = 0; cc < 100; cc++)
                    Add(new BackGroundNoise(this, Block));
            }

            float w     = Block.Inner.W / 4;
            var   stack = Add(new StackElement(this, Block, DOrient.Horz));
            this.ElementRegisterFile = stack.Add(new ElementRegisterFile(stack, Model.RegisterFile, DBlock.JustWidth(w).Set(20, 1, 10)));
            this.ElementALU = stack.Add(new ALUElement(stack, Model.ALU, DBlock.JustWidth(w).Set(20, 1, 10)));
            this.ElementInstructions = stack.Add(new MemoryViewElement(stack, DBlock.JustWidth(w).Set(20, 1, 10), Model.Instructions)
            {
                Title = "Instructions"
            });
            this.ElementCode = stack.Add(new CodeSection(stack, Model.Story.MainFile, DBlock.JustWidth(w).Set(10, 1, 10)));

            var dBlock = new DBlock(300, 1050, 900, 400);
            dBlock.Set(0, 3, 10);
            var term   = Add(new TerminalElement(this, new Terminal(), dBlock));
        }

        protected override void InitSceneComplete()
        {
            ElementALU.Start();
        }

        public override void StepScene(TimeSpan s)
        {
        }
        
        

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

        

        public override void KeyPress(object platformKeyObject, string key)
        {
            LastKeyPress = key;
            foreach (var register in Model.RegisterFile)
            {
                register.IsChanged = false;
            }
            if (Model?.Story == null) return;

            switch (key)
            {
                case "s":
                    ElementALU.Start();
                    break;
                
                case "d":
                case "n":
                    ElementALU.Next();
                    break;
                
                case "a":
                case "p":
                    ElementALU.Prev();
                    break;
                case "q":
                    SendCommand.Invoke("QUIT", null);
                    break;
            }
        }

       
    }
}