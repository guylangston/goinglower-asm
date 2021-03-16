using System;
using System.Linq;
using Animated.CPU.Animation;
using SkiaSharp;

namespace Animated.CPU.Model
{
    public class ExecutePhaseElementINP : Section<SceneExecute, PhaseExecute>
    {
        private TextBlockElement text;
        LogicUnitElement master;

        public ExecutePhaseElementINP(IElement parent, PhaseExecute model, LogicUnitElement master) : base(parent, model, new DBlock()
        {
            H = 200
        })
        {
            this.master = master;
            Title       = "Execute: READ";
            Block.Set(4, 1, 10);
            Block.Margin = new DBorder(30, 4, 4, 4);
        }
        

        protected override void Init()
        {
            text = Add(new TextBlockElement(this, Block, Scene.Styles.FixedFont));
        }

        protected override void Step(TimeSpan step)
        {
            IsEnabled     = master.StateMachine.GetSeq(this) <= master.StateMachine.GetSeq(master.StateMachine.Current);
            IsHighlighted = (master.Active == this);

            if (!IsHighlighted)
            {
                return;
            }


            text.Block = this.Block;
            text.Clear();

            var decode = Model.DecodeResult;
            if (decode?.Args != null && decode.Args.Any())
            {
                foreach (var arg in decode.Args.Where(x=>(x.InOut & InOut.In) > 0).Distinct(DecodedArg.Compare))
                {
                    if (arg.Register != null)
                    {
                        arg.Register.LastUsedAs = arg.Value;    
                        if (Scene.TryGetElementFromModelRecurse(arg.Register, out var eReg) && eReg is ElementBase eb)
                        {
                            eb.IsHidden = false;
                        } 
                    }
                    
                    var val = Model.Alu.GetInput(decode, arg);
                    var loc = text.Write($" IN A{arg.Index}: ");
                    text.Write(arg.Value, Scene.Styles.FixedFontCyan);
                    text.Write(" ");
                    if (val != null)
                    {
                        text.Write(val.ValueRaw, Scene.Styles.FixedFontYellow);
                        text.Write("=");
                        text.Write(val.ValueParsed, Scene.Styles.FixedFontCyan);
                        
                    }
                    else
                    {
                        text.Write("?", Scene.Styles.FixedFontDarkGray);
                    }
                    text.WriteLine();
                    if (arg.Description != null)
                    {
                        text.WriteLine($"    {arg.Description}");
                    }
                    
                    if (IsHighlighted)
                    {
                        if (arg.Register != null && Scene.TryGetElementFromModelRecurse(arg.Register, out var eReg))
                        {
                            loc.CustomDraw = (c) => {
                                
                                var arr = new DockedArrow(
                                    new DockPoint(eReg),
                                    new DockPoint(this),
                                    
                                    IsHighlighted ? Scene.Styles.Arrow : Scene.Styles.ArrowGray
                                );
                                arr.Step();
                                arr.Draw(c.Canvas);
                            };

                        }
                    }
                }

            }
        }

        public void StateChangeOnEnter(State<IElement> change)
        {
            foreach (var reg in Model.Alu.Cpu.RegisterFile)
            {
                reg.IsChanged = false;
            }

            if (master.Story.CurrentIndex > 0)
            {
                foreach (var dt in master.Story.Steps[master.Story.CurrentIndex-1].Delta)
                {
                    if (dt.ValueParsed != null)
                    {
                        var reg = Model.Alu.Cpu.SetReg(dt.Register, dt.ValueParsed.Value);
                        if (reg == Scene.Cpu.RIP)
                        {
                            reg.IsChanged = false;  // RIP always changes - so hide it
                        }
                    }
                        
                }    
            }
        }

        protected override void Decorate(DrawContext surface)
        {
            if (IsHighlighted)
            {
                var seg = Scene.Cpu.Instructions.GetByAddress(master.Story.Current.RIP);
                if (seg != null && Scene.TryGetElementFromModelRecurse(seg, out var eRip))
                {
                    var arr = new DockedArrow(
                        new DockPoint(this),
                        new DockPoint(eRip),
                        Scene.Styles.Arrow
                    );
                    arr.Step();
                    arr.Draw(surface.Canvas);
                }
            }
            
            
        }
    }
}