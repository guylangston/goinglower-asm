using System;
using System.Linq;
using Animated.CPU.Animation;
using SkiaSharp;

namespace Animated.CPU.Model
{
    public class ExecutePhaseElement : Section<Scene, PhaseExecute>
    {
        private TextBlockElement text;
        ALUElement master;

        public ExecutePhaseElement(IElement parent, PhaseExecute model, ALUElement master) : base(parent, model, new DBlock()
        {
            H = 300
        })
        {
            this.master = master;
            Title       = "Execute";
            Block.Set(4, 1, 10);
            Block.Margin = new DBorder(30, 4, 4, 4);
        }
        

        protected override void Init()
        {
            text = Add(new TextBlockElement(this, Block, Scene.Styles.FixedFont));
        }

        protected override void Step(TimeSpan step)
        {
            IsEnabled     = !(master.StateMachine.Current == master.StateMachine.Fetch || master.StateMachine.Current == master.StateMachine.Decode);
            IsHighlighted = (master.Active == this);

            if (!IsHighlighted)
            {
                return;
            }


            text.Block = this.Block;
            text.Clear();

            var decode = Model.Asm;
            if (decode?.Args != null && decode.Args.Any())
            {
                foreach (var arg in decode.Args.Where(x=>(x.InOut & InOut.In) > 0).Distinct(DecodedArg.Compare))
                {
                    var val = Model.Alu.GetInput(decode, arg);
                    var loc = text.Write($" IN A{arg.Index}: ");
                    text.Write(arg.Value, Scene.Styles.FixedFontCyan);
                    text.Write(" ");
                    if (val != null)
                    {
                        text.Write(val.ValueRaw, Scene.Styles.FixedFontYellow);
                        text.Write("=");
                        text.WriteLine(val.ValueParsed, Scene.Styles.FixedFontCyan);
                    }
                    else
                    {
                        text.WriteLine("?", Scene.Styles.FixedFontDarkGray);
                    }
                    
                    if (master.StateMachine.Current == master.StateMachine.ExecuteInp)
                    {
                        if (arg.Register != null && Scene.TryRecurseElementFromModel(arg.Register, out var eReg))
                        {
                            loc.CustomDraw = (c) => {
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
                            };

                        }
                    }
                }

                text.WriteLine();
                text.Write("---[ ", Scene.Styles.FixedFontDarkGray);
                text.Write(decode.OpCode.ToUpperInvariant(), Scene.Styles.FixedFontYellow);
                text.Write(" ]---", Scene.Styles.FixedFontDarkGray);
                text.WriteLine();
                text.WriteLine(Model.AsmText, Scene.Styles.FixedFontGray);
                text.WriteLine();
                
                foreach (var arg in decode.Args.Where(x=>(x.InOut & InOut.Out) > 0).Distinct(DecodedArg.Compare))
                {
                    var val = Model.Alu.GetOutput(decode, arg);

                    var loc = text.Write($"OUT A{arg.Index}: ");
                    text.Write(arg.Value.PadRight(10), Scene.Styles.FixedFontCyan);
                    text.Write(" ");
                    if (val != null)
                    {
                        text.Write(val.ValueRaw, Scene.Styles.FixedFontYellow);
                        text.Write("=");
                        text.WriteLine(val.ValueParsed, Scene.Styles.FixedFontCyan);
                    }
                    else
                    {
                        text.WriteLine("?", Scene.Styles.FixedFontDarkGray);
                    }
                    
                    if (master.StateMachine.Current == master.StateMachine.ExecuteOut)
                    {
                        if (arg.Register != null && Scene.TryRecurseElementFromModel(arg.Register, out var eReg))
                        {
                            loc.CustomDraw = (c) => {
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

            if (change == master.StateMachine.ExecuteInp)
            {
                
                if (master.Story.CurrentIndex > 0)
                {
                    foreach (var dt in master.Story.Steps[master.Story.CurrentIndex-1].Delta)
                    {
                        if (dt.ValueParsed != null)
                        {
                            Model.Alu.Cpu.SetReg(dt.Register, dt.ValueParsed.Value);    
                        }
                        
                    }    
                }
                
            }
            else if (change == master.StateMachine.ExecuteOut)
            {
                foreach (var dt in master.Story.Current.Delta)
                {
                    if (dt.ValueParsed != null)
                    {
                        Model.Alu.Cpu.SetReg(dt.Register, dt.ValueParsed.Value);    
                    }
                        
                }
                
            }
        }

        protected override void Decorate(DrawContext surface)
        {
            if (master.Active == this)
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