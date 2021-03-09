using System;
using System.Diagnostics;
using Animated.CPU.Animation;
using Animated.CPU.Model.ModelElements;
using SkiaSharp;

namespace Animated.CPU.Model
{
    public class SimpleSection : Section<Scene, string>
    {
        public SimpleSection(IElement parent, string model, DBlock block) : base(parent, model, block)
        {
            Title = model;
        }
    }
    
    public class Scene : SceneBase<Cpu, StyleFactory>
    {
        public Scene(DBlock region) : base(new StyleFactory())
        {
            Block = region;
        }
        
        // Helpers
        public Cpu                 Cpu                 => Model;
        public MemoryViewElement   ElementInstructions { get; set; }
        public CodeSection         ElementCode         { get; set; }
        public ElementRegisterFile ElementRegisterFile { get; set; }
        public TerminalElement     Terminal            { get; set; }
        
        public Action<string, object> SendCommand      { get; set; }
        public ALUElement             ElementALU       { get; set; }
        public string                 LastKeyPress     { get; set; }
        public bool                   UseEmbelishments { get; set; } = true;
        public string                 DebugText        { get; set; }

        protected override void InitScene()
        {
            if (UseEmbelishments)
            {
                for (int cc = 0; cc < 100; cc++)
                    Add(new BackGroundNoise(this, Block));
            }

            float w     = Block.Inner.W / 4;

            var cpu = Add(new SimpleSection(this, "CPU", 
                DBlock.FromTwoPoints(new SKPoint(50, 20), new SKPoint(970, 1020))));
            
            var ram = Add(new SimpleSection(this, "RAM", 
                DBlock.FromTwoPoints(new SKPoint(990, 20), new SKPoint(1440, 1020))));
            

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
            Terminal   = Add(new TerminalElement(this, new Terminal(), dBlock));
        }
        
        protected override void InitSceneComplete()
        {
            ElementALU.Start();
            int cc = 0;
            while (cc < 50 && ElementALU.Story.Current.Asm == null)
            {
                ElementALU.StateMachine.ExecNext();
                cc++;
            }
        }

        public override void StepScene(TimeSpan s)
        {
        }
        
        

        protected override void DrawOverlay(DrawContext surface)
        {
            // surface.DrawText($"{FrameCount} frames at {Elapsed.TotalSeconds:0.00} sec. {lastKey} | {Mouse}", 
            //     Styles.GetPaint(this, "debug"),
            //     new SKPoint(0,0));

            if (DebugPointAt != SKPoint.Empty)
            {
                surface.Canvas.DrawCircle(DebugPointAt, 4, Styles.Highlighted);
            }
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

        public override void ButtonPress(uint eventButton, double eventX, double eventY, object interop)
        {
            DebugPointAt = new SKPoint((float)eventX, (float)eventY);
            DebugButton  = eventButton;
            
            foreach (var element in Scene.ChildrenRecursive())
            {
                var hit = element.GetSelectionAtPoint(Scene.DebugPointAt);
                if (hit != null && hit.Selection is TextBlockElement.Span span && span.Url != null)
                {
                    ShowUrl(span.Url);
                    return;
                }
            }
        }

        private void ShowUrl(string spanUrl)
        {
            try
            {
                Process.Start("xdg-open", spanUrl);
                // TODO: Show animation of browser loading
            }
            catch (Exception e)
            {
                Terminal.Error = e;
            }
        }
    }
}