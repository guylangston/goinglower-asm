using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using Animated.CPU.Animation;
using SkiaSharp;

namespace Animated.CPU.Model
{
    public class ALUElement : Section<Scene, ArithmeticLogicUnit>
    {
        public ALUElement(IElement scene, ArithmeticLogicUnit alu, DBlock b) : base(scene, alu, b)
        {
            Title = "ALU";
        }

        public override void Init()
        {
            var stack = Add(new StackElement(this, Block, DOrient.Vert));

            this.Fetch = stack.Add(new FetchPhaseElement(stack, Model.Fetch));
            this.Decode = stack.Add(new DecodePhaseElement(stack, Model.Decode));
            this.Execute = stack.Add(new ExecutePhaseElement(stack, Model.Execute));
        }

        public ExecutePhaseElement Execute { get; set; }
        public DecodePhaseElement  Decode  { get; set; }
        public FetchPhaseElement   Fetch   { get; set; }


    }

    public class FetchPhaseElement : Section<Scene, PhaseFetch>
    {
        private TextBlockElement text;

        public FetchPhaseElement(IElement parent, PhaseFetch model) : base(parent, model, new DBlock()
        {
            H = 100
        })
        {
            Title = "Fetch";
            Block.Set(4, 1, 10);
            Block.Margin = new DBorder(30, 4, 4, 4);
        }

        public override void Init()
        {
            text = Add(new TextBlockElement(this, Block, Scene.Styles.FixedFont));
        }

        protected override void Step(TimeSpan step)
        {
            if (Model.Memory != null)
            {
                text.Clear();

                text.Write("RIP".PadRight(12));
                text.Write(": ");
                text.WriteLine(DisplayHelper.ToHex(Model.RIP), Scene.Styles.FixedFontCyan);

                text.Write("Binary Code".PadRight(12));
                text.Write(": ");
                text.WriteLine(DisplayHelper.ToHex(Model.Memory), Scene.Styles.FixedFontBlue);
            }
            //Block.H = Math.Max(60, text.LastDrawHeight);
        }

      

        protected override void Decorate(DrawContext surface)
        {
            var seg = Scene.Cpu.Instructions.GetByAddress(Model.RIP);
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

    public class DecodePhaseElement : Section<Scene, PhaseDecode>
    {
        private TextBlockElement text;

        public DecodePhaseElement(IElement parent, PhaseDecode model) : base(parent, model, new DBlock()
        {
            H = 200
        })
        {
            Block.Set(4, 1, 10);
            Block.Margin = new DBorder(30, 4, 4, 4);
            Title        = "Decode";
        }

        public override void Init()
        {
            text = Add(new TextBlockElement(this, Block, Scene.Styles.FixedFont));
        }

        protected override void Step(TimeSpan step)
        {
            text.Clear();
            var decode = Model.Asm;
            if (decode != null)
            {
                text.WriteLine(decode);
                text.WriteLine();
                text.WriteLine(decode.FriendlyName, Scene.Styles.FixedFontBlue);
                text.WriteLine(decode.FriendlyMethod, Scene.Styles.FixedFontBlue);
            }
        }

        protected override void Draw(DrawContext surface)
        {
            base.Draw(surface);
        }
    }

    public class ExecutePhaseElement : Section<Scene, PhaseExecute>
    {
        private TextBlockElement text;

        public ExecutePhaseElement(IElement parent, PhaseExecute model) : base(parent, model, new DBlock()
        {
            H = 300
        })
        {
            Title = "Execute";
            Block.Set(4, 1, 10);
            Block.Margin = new DBorder(30, 4, 4, 4);
        }

        public override void Init()
        {
            text = Add(new TextBlockElement(this, Block, Scene.Styles.FixedFont));
        }

        protected override void Step(TimeSpan step)
        {
            text.Block = this.Block;
            text.Clear();

            var decode = Model.Asm;
            if (decode?.Args != null && decode.Args.Any())
            {
                foreach (var arg in decode.Args.Distinct(DecodedArg.Compare))
                {
                    var val = Model.Alu.GetInput(decode, arg);
                    text.Write($" IN A{arg.Index}: ");
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
                }

                text.WriteLine();
                text.Write("---[ ", Scene.Styles.FixedFontDarkGray);
                text.Write(decode.OpCode.ToUpperInvariant(), Scene.Styles.FixedFontYellow);
                text.Write(" ]---", Scene.Styles.FixedFontDarkGray);
                text.WriteLine();
                text.WriteLine(Model.AsmText, Scene.Styles.FixedFontGray);
                text.WriteLine();
                
                foreach (var arg in decode.Args.Distinct(DecodedArg.Compare))
                {
                    var val = Model.Alu.GetOutput(decode, arg);

                    text.Write($"OUT A{arg.Index}: ");
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
                }
            }
        }
    }
}