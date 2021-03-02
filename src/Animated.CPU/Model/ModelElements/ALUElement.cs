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

        public override void Init(SKSurface surface)
        {
            var stack = Add(new StackElement(Scene, this, Block, DOrient.Vert));

            foreach (var phase in Model.Phases())
            {
                stack.Add(new ALUPhaseFetchElement(stack, phase));
            }
            
        }

        public override void Step(TimeSpan step)
        {
        }

        public override void Draw(SKSurface surface)
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

            foreach (var register in UsedRegisters().Where(x=>x.Id != "RIP"))
            {
                new Arrow()
                    {
                        Start = RecurseByModel(Model.Execute).Block.Inner.MM + new SKPoint(0, 20),
                        End   = Scene.RecurseElementFromModelSafe(register).Block.Outer.MR,
                        Style = Scene.StyleFactory.GetPaint(this, "arrow")
                    }
                    .RelativeWayPoints(new SKPoint(0, +20), new SKPoint(+50, 0))
                    .Draw(surface.Canvas);    
            }
            
            
            foreach (var reg in LoosyMathRegs(Scene.Model, Model.Cpu.Story.Current.Asm))
            {
                new Arrow()
                    {
                        Start = RecurseByModel(Model.Decode).Block.Inner.MM + new SKPoint(0, 20),
                        End   = Scene.RecurseElementFromModelSafe(reg).Block.Outer.MR,
                        Style = Scene.StyleFactory.GetPaint(this, "arrow")
                    }
                    .RelativeWayPoints(new SKPoint(0, +20), new SKPoint(+50, 0))
                    .Draw(surface.Canvas);
            }
        }

        private IEnumerable<Register> UsedRegisters()
        {
            foreach (var delta in Scene.Model.Story.Current.Delta)
            {
                var r = Scene.Model.RegisterFile.FirstOrDefault(x => x.IsChanged && x.Match(delta.Register));
                if (r != null) yield return r;
            }

        }

        private IEnumerable<Register> LoosyMathRegs(Cpu cpu, string asm)
        {
            foreach (var register in cpu.RegisterFile)
            {
                if (asm.Contains(register.Id, StringComparison.InvariantCultureIgnoreCase))
                {
                    yield return register;
                }
            }
        }
    }

    public class ALUPhaseFetchElement : Element<Scene, object>
    {
        public ALUPhaseFetchElement(IElement parent, object model) : base(parent, model, new DBlock())
        {
            Block.H = 150;
            Block.Set(5, 2, 5);
            
        }

        public override void Step(TimeSpan step)
        {
            
        }

        public override void Draw(SKSurface surface)
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