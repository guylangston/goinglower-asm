using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GoingLower.Core;
using GoingLower.Core.Actions;
using GoingLower.Core.Drawing;
using GoingLower.Core.Elements;
using GoingLower.Core.Elements.Effects;
using GoingLower.Core.Helpers;
using GoingLower.Core.Primitives;
using GoingLower.CPU.Elements;
using GoingLower.CPU.Model;
using SkiaSharp;

namespace GoingLower.CPU.Scenes
{
    public class SceneExecute : SceneBase<Cpu, StyleFactory>
    {
        private readonly ISceneMaster master;
        private CodeElement ElementCodeIL;
        private CodeElement ElementCodeASM;

        public SceneExecute(ISceneMaster master, Cpu model, DBlock region) : base("Execute", new StyleFactory(), region)
        {
            this.master = master;
            Model       = model;
        }
        
        
        // Helpers
        public Cpu                    Cpu                 => Model;
        public MemoryViewElement      ElementInstructions { get; set; }
        public CodeElement            ElementCode         { get; set; }
        public ElementRegisterFile    ElementRegisterFile { get; set; }
        public TerminalElement        Terminal            { get; set; }
        public DialogElement          Dialog              { get; set; }
        
        public LogicUnitElement       ElementLogicUnit    { get; set; }
        public string                 LastKeyPress        { get; set; }
        public bool                   UseEmbelishments    { get; set; } = true;
        public string                 DebugText           { get; set; }

        protected override void InitScene()
        {
            if (UseEmbelishments)
            {
                for (int cc = 0; cc < 100; cc++)
                    Add(new BackGroundNoiseElement(this, Block));
            }

            var ww = 1920;
            var hh = 1090;

            float w     = Block.Inner.W / 4;

            // var cpu = Add(new SimpleSection(this, "CPU", 
            //     DBlock.FromTwoPoints(new SKPoint(50, 20), new SKPoint(970, 1020))));
            // cpu.TitleAction = "CPU";
            //
            // var ram = Add(new SimpleSection(this, "RAM", 
            //     DBlock.FromTwoPoints(new SKPoint(990, 20), new SKPoint(1440, 1020))));
            // ram.TitleAction = "RAM";
            //

            var   stack = Add(new StackElement(this, Block, DOrient.Horz));
            
            this.ElementCode   = stack.Add(new CodeElement(stack, Model.Story.MainFile, 
                DBlock.JustWidth(w)));

            // if (Cpu.Story.IL != null)
            // {
            //     this.ElementCodeIL = stack.Add(new CodeElement(stack, Cpu.Story.IL, 
            //         DBlock.JustWidth(w)));
            //     this.ElementCodeIL.IsHidden = true;
            // }
            //
            // if (Cpu.Story.Asm != null)
            // {
            //     this.ElementCodeASM = stack.Add(new CodeElement(stack, Cpu.Story.Asm, 
            //         DBlock.JustWidth(w)));
            //     this.ElementCodeASM.IsHidden = true;
            // }
            
            this.ElementInstructions = stack.Add(new MemoryViewElement(stack,  
                DBlock.JustWidth(w),
                Model.Instructions)
            {
                Title = "Executable Memory"
            });
            
            this.ElementLogicUnit          = stack.Add(new LogicUnitElement(stack, Model.ALU, 
                DBlock.JustWidth(w)));
            
            this.ElementRegisterFile = stack.Add(new ElementRegisterFile(stack, Model.RegisterFile, 
                DBlock.JustWidth(w)));

            foreach (var ss in stack.Children)
            {
                ss.Block?.Set(5, 1, 10);
            }

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
                var x = Add(new ButtonElement(this, new Command()
                    {
                        Name = action,
                        Arg  = action
                    }, rel
                ));
                rel.Set(0, 2, 20);

                rel = rel.CreateRelative(BlockAnchor.TR, false, new SKPoint(20, 0), new SKPoint(60, 20));
            }
            
            
        }

        protected override void InitSceneComplete()
        {
            ElementLogicUnit.Start();
            int cc = 0;
            while (cc < 50 && ElementLogicUnit.Story.Current.Asm == null)
            {
                ElementLogicUnit.StateMachine.ExecNext();
                cc++;
            }
            foreach (var reg in ElementRegisterFile.ChildrenRecursiveAre<ElementRegister>())
            {
                reg.IsHidden        = true;
                reg.Model.IsChanged = false;
            }

            // if (Model.Story?.ReadMe != null && Model.Story.ReadMe.Any())
            // {
            //     ShowDialog("README", Model.Story.ReadMe);
            // }
        }

        private void ShowDialog(string? slideTitle, string slideText)
            => ShowDialog(slideTitle, StringHelper.ToLines(slideText).ToList());
        
        private void ShowDialog(string? slideTitle, IReadOnlyList<string> lines)
        {
            Dialog.Model = new Dialog()
            {
                Title = slideTitle,
                Lines = lines,
            };
            Dialog.Image    = null;
            Dialog.IsHidden = false;
        }
        
        private void ShowDialog(string? slideTitle, SKBitmap image)
        {
            Dialog.Model = new Dialog()
            {
                Title = slideTitle,
            };
            Dialog.Image    = image;
            Dialog.IsHidden = false;
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

        public override void ProcessEvent(string name, object args, object platform)
        {
            
        }

        public override void KeyPress(string key, object platformKeyObject)
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
                
                // case "t":
                //     ShowDialog("Test", bitmap1);
                //     break;
                // case "F1":
                // case "h":
                //     PerformAction(new ActionModel("Help"));
                //     break;
                // case "F2":
                // case "?":
                //     PerformAction(new ActionModel("CurrentSlide"));
                //     break;
                
                case "r":
                    this.InitSceneComplete();
                    break;
                
                case "Prior":
                    ElementLogicUnit.PrevInstruction();
                    break;
                
                case "Next":
                    ElementLogicUnit.NextInstruction();
                    break;
                
                case "d":
                case "n":
                case "period":
                case "Down":
                    ElementLogicUnit.Next();
                    break;
                
                case "a":
                case "p":
                case "Up":
                case "comma":
                    ElementLogicUnit.Prev();
                    break;
                
                case "Key_1":
                    master.MoveToScene(new SceneSeqArg(SceneSeq.Prior));
                    break;
                
                case "q":
                    SendHostCommand?.Invoke("QUIT", null);
                    break;
            }
        }

        public override void MousePress(uint eventButton, double eventX, double eventY, object interop)
        {
            if (eventButton == 3)
            {
                DebugPointAt = new SKPoint((float)eventX, (float)eventY);
            }
            else
            {
                DebugPointAt = new SKPoint();
            }
            
            DebugButton = eventButton;  
            
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
                    else if (hit.Selection is Command am)
                    {
                        PerformAction(am);
                        return;
                    }

                }
            }
        }

        private void PerformAction(Command act)
        {
            Terminal.Status = null;
            if (act.Name == "Quit")
            {
                SendHostCommand.Invoke("QUIT", null);
                return;
            }

            if (act.Name == "Help")
            {
                if (Cpu?.Story?.ReadMe != null)
                {
                    ShowDialog("ReadMe", Cpu.Story.ReadMe.Lines);
                }
                
                return;
            }
            
            // if (act.Name == nameof(ElementRegisterFile))
            // {
            //     Dialog.Model = new Dialog()
            //     {
            //         Title = "Registers"
            //     };
            //     Dialog.Image    = this.bitmap1;
            //     Dialog.IsHidden = false;
            //     
            //     return;
            // }
            
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
            
            if (act.Name == "CurrentSlide")
            {
                var s = Model.Story.CurrentSlide;
                if (s is not null)
                {
                    ShowDialog(s.Title, s.Text);
                }
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