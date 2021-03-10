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
        private SKBitmap bitmap1;

        public Scene(DBlock region) : base(new StyleFactory())
        {
            Block = region;
        }

        public const string Version = "0.2alpha";
        
        // Helpers
        public Cpu                    Cpu                 => Model;
        public MemoryViewElement      ElementInstructions { get; set; }
        public CodeSection            ElementCode         { get; set; }
        public ElementRegisterFile    ElementRegisterFile { get; set; }
        public TerminalElement        Terminal            { get; set; }
        public DialogElement          Dialog              { get; set; }
        public Action<string, object> SendCommand         { get; set; }
        public ALUElement             ElementALU          { get; set; }
        public string                 LastKeyPress        { get; set; }
        public bool                   UseEmbelishments    { get; set; } = true;
        public string                 DebugText           { get; set; }

        protected override void InitScene()
        {
            if (UseEmbelishments)
            {
                for (int cc = 0; cc < 100; cc++)
                    Add(new BackGroundNoise(this, Block));
            }

            var ww = 1920;
            var hh = 1090;

            float w     = Block.Inner.W / 4;

            var cpu = Add(new SimpleSection(this, "CPU", 
                DBlock.FromTwoPoints(new SKPoint(50, 20), new SKPoint(970, 1020))));
            cpu.TitleAction = "CPU";
            
            var ram = Add(new SimpleSection(this, "RAM", 
                DBlock.FromTwoPoints(new SKPoint(990, 20), new SKPoint(1440, 1020))));
            ram.TitleAction = "RAM";
            

            var   stack = Add(new StackElement(this, Block, DOrient.Horz));
            this.ElementRegisterFile = stack.Add(new ElementRegisterFile(stack, Model.RegisterFile, DBlock.JustWidth(w).Set(20, 1, 10)));
            this.ElementALU = stack.Add(new ALUElement(stack, Model.ALU, DBlock.JustWidth(w).Set(20, 1, 10)));
            this.ElementInstructions = stack.Add(new MemoryViewElement(stack, DBlock.JustWidth(w).Set(20, 1, 10), Model.Instructions)
            {
                Title = "Instructions",
                TitleAction = "ASM"
            });
            this.ElementCode = stack.Add(new CodeSection(stack, Model.Story.MainFile, DBlock.JustWidth(w).Set(10, 1, 10)));

            var bTerm = new DBlock(30, hh + 10, 900, 400);
            bTerm.Set(0, 3, 10);
            Terminal   = Add(new TerminalElement(this, new Terminal(), bTerm));

            var d = 400;
            
            var bDialog = new DBlock(600, d, 900, hh-d - 30);
            bDialog.Set(0, 3, 10);
            this.Dialog = Add(new DialogElement(this, new Dialog(), bDialog));

            var rel = bTerm.CreateRelative(BlockAnchor.TR, false, new SKPoint(20, 0), new SKPoint(60, 20));

            foreach (var action in new string[] { "Help", "Quit", "Next", "Previous"})
            {
                var x = Add(new ButtonElement(this, new ActionModel()
                    {
                        Name = action,
                        Arg  = action
                    }, rel
                ));
                rel.Set(0, 2, 20);

                rel = rel.CreateRelative(BlockAnchor.TR, false, new SKPoint(20, 0), new SKPoint(60, 20));
            }
            
            this.bitmap1 = SKBitmap.Decode("/home/guy/repo/cpu.anim/doc/IntelIntro-GeneralArch.png");
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
            foreach (var reg in ElementRegisterFile.ChildrenRecursiveAre<ElementRegister>())
            {
                reg.IsHidden = true;
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
            
            surface.DrawRect(Block.Outer, Styles.Border);
            
            surface.Canvas.DrawText($"0xGoingLower v{Version}", new SKPoint(10,20), Styles.TextLogo);

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
                case "Escape":
                    Dialog.IsHidden = true;
                    return;
                
                case "t":
                    Dialog.Model = new Dialog()
                    {
                        Title = "Dialog Test"
                    };
                    Dialog.Model.Lines.Add("Hello World");

                    Dialog.Image    = bitmap1;
                    Dialog.IsHidden = !Dialog.IsHidden;
                    break;
                
                
                case "F1":
                case "h":
                    PerformAction(new ActionModel("Help"));
                    break;
                
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
                if (hit != null)
                {
                    if (hit.Selection is TextBlockElement.Span span && span.Url != null)
                    {
                        ShowUrl(span.Url);
                        return;    
                    }
                    else if (hit.Element is ButtonElement be)
                    {
                        PerformAction(be.Model);
                        return;
                    }
                    else if (hit.Selection is ActionModel am)
                    {
                        PerformAction(am);
                        return;
                    }

                }
            }
        }

        private void PerformAction(ActionModel act)
        {
            Terminal.Status = null;
            if (act.Name == "Quit")
            {
                SendCommand.Invoke("QUIT", null);
                return;
            }

            if (act.Name == "Help")
            {
                Dialog.Model = new Dialog()
                {
                    Title = "Help"
                };
                if (Cpu?.Story?.ReadMe != null)
                {
                    foreach (var line in Cpu.Story.ReadMe)
                    {
                        Dialog.Model.Lines.Add(line);
                    }
                }
                Dialog.IsHidden = !Dialog.IsHidden;
                return;
            }
            
            if (act.Name == nameof(ElementRegisterFile))
            {
                Dialog.Model = new Dialog()
                {
                    Title = "Registers"
                };
                Dialog.Image    = this.bitmap1;
                Dialog.IsHidden = false;
                
                return;
            }
            
            if (act.Name == "CPU")
            {
                ShowUrl("https://en.wikipedia.org/wiki/X86-64");
                return;
            }
            
            if (act.Name == "ASM")
            {
                ShowUrl("https://sonictk.github.io/asm_tutorial/");
                return;
            }
            
            

            Terminal.Status = (FormattableString)$"Unhandled Action: {act.Name}, {act.Arg}";
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