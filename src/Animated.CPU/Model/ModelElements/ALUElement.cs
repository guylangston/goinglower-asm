using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Animated.CPU.Animation;
using SkiaSharp;

namespace Animated.CPU.Model
{
    public class ALUElement : Element<Scene, ArithmeticLogicUnit>
    {
        public ALUElement(Scene scene, ArithmeticLogicUnit alu, DBlock b) : base(scene, alu, b)
        {
        }

        public override void Init(DrawContext surface)
        {
            var stack = Add(new StackElement(Scene, this, Block, DOrient.Vert));

            stack.Add(new FetchPhaseElement(stack, Model.Fetch));
            stack.Add(new DecodePhaseElement(stack, Model.Decode));
            stack.Add(new ExecutePhaseElement(stack, Model.Execute));
            stack.Add(new ALUPhaseFetchElement(stack, Model.Step));
            
            
        }

        protected override void Step(TimeSpan step)
        {
        }

        protected override void Draw(DrawContext surface)
        {

            if (Scene.Model.Story.Current == null) return;
            
            new Arrow()
            {
                Start = RecurseByModel(Model.Fetch).Block.Inner.MM + new SKPoint(0, 20),
                End   = Scene.RecurseElementFromModelSafe(Scene.Model.Instructions.GetByAddress(Model.Fetch.RIP.Value)).Block.Outer.ML,
                Style = Scene.StyleFactory.GetPaint(this, "arrow")
            }
            .RelativeWayPoints(new SKPoint(0, +20), new SKPoint(-50, 0))
            .Draw(surface.Canvas);
            
            // foreach (var reg in LoosyMathRegs(Scene.Model, Model.Cpu.Story.Current.Asm))
            // {
            //     new Arrow()
            //         {
            //             Start = RecurseByModel(Model.Decode).Block.Inner.MM + new SKPoint(0, 20),
            //             End   = Scene.RecurseElementFromModelSafe(reg).Block.Outer.MR,
            //             Style = Scene.StyleFactory.GetPaint(this, "arrow")
            //         }
            //         .RelativeWayPoints(new SKPoint(0, +20), new SKPoint(+50, 0))
            //         .Draw(surface.Canvas);
            // }

            // foreach (var register in UsedRegisters().Where(x=>x.Id != "RIP"))
            // {
            //     new Arrow()
            //         {
            //             Start = RecurseByModel(Model.Execute).Block.Inner.MM + new SKPoint(0, 20),
            //             End   = Scene.RecurseElementFromModelSafe(register).Block.Outer.MR,
            //             Style = Scene.StyleFactory.GetPaint(this, "arrow")
            //         }
            //         .RelativeWayPoints(new SKPoint(0, +20), new SKPoint(+50, 0))
            //         .Draw(surface.Canvas);    
            // }
            
            
          
        }

      
      
    }
    
    public class FetchPhaseElement : Element<Scene, PhaseFetch>
    {
        private TextBlockElement text;

        public FetchPhaseElement(IElement parent, PhaseFetch model) : base(parent, model, new DBlock()
        {
            H = 80
        })
        {
            Block.Set(4, 1, 40);
        }

        public override void Init(DrawContext surface)
        {
            text = Add(new TextBlockElement(Scene, this, Block, Scene.StyleFactory.FixedFont));
        }

        protected override void Step(TimeSpan step)
        {
            if (Model.Memory != null)
            {
                text.Clear();
                text.WriteLine("--- FETCH ---", Scene.StyleFactory.h1);
                text.Write("RIP".PadRight(10));
                text.Write(": ");
                text.WriteLine(Model.RIP.ValueHex, Scene.StyleFactory.FixedFontCyan);
                text.WriteLine(DisplayHelper.ToHex(Model.Memory), Scene.StyleFactory.FixedFontBlue);    
            }
            
        }

        protected override void Draw(DrawContext surface)
        {
            var drawing = new Drawing(surface.Canvas);
            drawing.DrawRect(Block, Scene.StyleFactory.GetPaint(this, "border"));
        }
    }

    public class DecodePhaseElement : Element<Scene, PhaseDecode>
    {
        private TextBlockElement text;

        public DecodePhaseElement(IElement parent, PhaseDecode model) : base(parent, model, new DBlock()
        {
            H = 100
        })
        {
            Block.Set(4, 1, 40);
        }

        public override void Init(DrawContext surface)
        {
            text = Add(new TextBlockElement(Scene, this, Block, Scene.StyleFactory.FixedFont));
        }

        protected override void Step(TimeSpan step)
        {
            text.Clear();
            text.WriteLine("--- DECODE ---", Scene.StyleFactory.h1);
            text.WriteLine(Model.Asm);
            text.WriteLine(Model.Easy, Scene.StyleFactory.FixedFontBlue);
        }

        protected override void Draw(DrawContext surface)
        {
            var drawing = new Drawing(surface.Canvas);
            drawing.DrawRect(Block, Scene.StyleFactory.GetPaint(this, "border"));
        }
    }

    public class ExecutePhaseElement : Element<Scene, PhaseExecute>
    {
        private TextBlockElement text;

        public ExecutePhaseElement(IElement parent, PhaseExecute model) : base(parent, model, new DBlock()
        {
            H = 300
        })
        {
            Block.Set(4, 1, 40);
        }

        public override void Init(DrawContext surface)
        {
            text = Add(new TextBlockElement(Scene, this, Block, Scene.StyleFactory.FixedFont));
        }

        protected override void Step(TimeSpan step)
        {
            text.Clear();
            text.WriteLine("--- EXECUTE ---", Scene.StyleFactory.h1);

            if (Model.Inputs != null)
            {
                text.WriteLine("|");
                text.WriteLine("Input:");
            
                foreach (var reg in Model.Inputs)
                {
                    text.Write(reg.Id.PadRight(10));
                    text.Write(": ");
                    text.WriteLine(reg.ValueHex, Scene.StyleFactory.FixedFontBlue);
                }    
            }
            
            


            if (Model.Changes != null)
            {
                text.WriteLine("|");
                text.WriteLine("Output:");
                foreach (var reg in Model.Changes)
                {
                    text.Write(reg.Id.PadRight(10));
                    text.Write(": ");
                    text.WriteLine(reg.ValueHex, Scene.StyleFactory.FixedFontCyan);
                }    
            }
            
        }

        protected override void Draw(DrawContext surface)
        {
            var drawing = new Drawing(surface.Canvas);
            drawing.DrawRect(Block, Scene.StyleFactory.GetPaint(this, "border"));
        }
    }

    public class ALUPhaseFetchElement : Element<Scene, object>
    {
        public ALUPhaseFetchElement(IElement parent, object model) : base(parent, model, new DBlock())
        {
            Block.H = 150;
            Block.Set(5, 2, 5);
            
        }

        protected override void Step(TimeSpan step)
        {
            
        }

        protected override void Draw(DrawContext surface)
        {
            var drawing = new Drawing(surface.Canvas);
            drawing.DrawRect(Block, Scene.StyleFactory.GetPaint(this, "border"));
            
            drawing.DrawText(Model?.ToString(), Scene.StyleFactory.GetPaint(this, "h1"), Block, BlockAnchor.TM);

            var mType    = Model.GetType();
            var txtStyle = Scene.StyleFactory.GetPaint(this, "text");
            var off      = new SKPoint(0,0);
            foreach (var prop in mType.GetProperties())
            {
                var pp  = prop.GetValue(Model);
                if (pp is {})
                {
                    if (pp is byte[] ba) pp = DisplayHelper.ToHex(ba);

                    if (pp is IEnumerable en && pp is not string)
                    {
                        var ss   = drawing.DrawText(prop.Name, txtStyle, Block, BlockAnchor.MM, off);
                        off += new SKPoint(0, ss.Height + 2);
                        foreach (var  inner in en)
                        {
                            var txt = $"{inner}";
                            var s   = drawing.DrawText(txt, txtStyle, Block, BlockAnchor.MM, off);
                            off += new SKPoint(0, s.Height + 2);
                        }
                    }
                    else
                    {
                        var txt = $"{prop.Name}: {pp}";
                        var s   = drawing.DrawText(txt, txtStyle, Block, BlockAnchor.MM, off);
                        off += new SKPoint(0, s.Height + 2);    
                    }
                    
                    
                        
                }
            }
        }
    }
}