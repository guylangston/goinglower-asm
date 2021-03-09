using System;
using System.Linq;
using Animated.CPU.Animation;
using SkiaSharp;

namespace Animated.CPU.Model
{
    public class ExecutePhaseElementINP : Section<Scene, PhaseExecute>
    {
        private TextBlockElement text;
        ALUElement master;

        public ExecutePhaseElementINP(IElement parent, PhaseExecute model, ALUElement master) : base(parent, model, new DBlock()
        {
            H = 150
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
                        if (arg.Register != null && Scene.TryRecurseElementFromModel(arg.Register, out var eReg))
                        {
                            loc.CustomDraw = (c) => {
                                if (IsHighlighted)
                                {
                                    var a = loc.LastDraw;
                                    var b = eReg.Block.Inner.MR;
                                    new Arrow()
                                    {
                                        Start     = a,
                                        WayPointA = a + new SKPoint(-20, 0),
                                        WayPointB = b + new SKPoint(+20, 0),
                                        End       = b,
                                        Style     = Scene.Styles.Arrow
                                    }.Draw(c.Canvas);
                                }
                                else
                                {
                                    var a = loc.LastDraw;
                                    var b = eReg.Block.Inner.MR;
                                    new Arrow()
                                    {
                                        Start     = a,
                                        WayPointA = a + new SKPoint(-20, 0),
                                        WayPointB = b + new SKPoint(+20, 0),
                                        End       = b,
                                        Style     = Scene.Styles.FixedFontGray
                                    }.Draw(c.Canvas);
                                }
                                
                                
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
                if (seg != null && Scene.TryRecurseElementFromModel(seg, out var eRip))
                {
                    var a = Block.Outer.MR;
                    var b = eRip.Block.Inner.ML;
                    new Arrow()
                    {
                        Start     = a,
                        WayPointA = a + new SKPoint(20, 0),
                        WayPointB = b + new SKPoint(-20, 0),
                        End       = b,
                        Style     = Scene.Styles.Arrow
                    }.Draw(surface.Canvas);
                }
            }
            
            
        }
    }
}