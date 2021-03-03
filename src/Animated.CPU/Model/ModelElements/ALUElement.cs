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

        public override void Init(DrawContext surface)
        {
            var stack = Add(new StackElement(this, Block, DOrient.Vert));

            stack.Add(new FetchPhaseElement(stack, Model.Fetch));
            stack.Add(new DecodePhaseElement(stack, Model.Decode));
            stack.Add(new ExecutePhaseElement(stack, Model.Execute));
      
            
            
        }

        protected override void Step(TimeSpan step)
        {
        }

       

      
      
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
        }

        public override void Init(DrawContext surface)
        {
            text = Add(new TextBlockElement( this, Block, Scene.Styles.FixedFont));
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
        }

        protected override void Draw(DrawContext surface)
        {
            base.Draw(surface);

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
            Title = "Decode";
        }

        public override void Init(DrawContext surface)
        {
            text = Add(new TextBlockElement( this, Block, Scene.Styles.FixedFont));
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
            Block.Set(4, 1, 10);
            Title = "Execute";
        }

        public override void Init(DrawContext surface)
        {
            text = Add(new TextBlockElement(this, Block, Scene.Styles.FixedFont));
        }

        protected override void Step(TimeSpan step)
        {
            text.Clear();
            
            var decode = Model. Asm;
            if (decode != null && decode.Args.Any())
            {
                foreach (var arg in decode.Args.Where(x=>(x.InOut & InOut.In) > 0))
                {
                    var val = Model.Alu.GetInput(decode, arg);
                    
                    text.Write($" IN: ");
                    text.Write(arg.Text.PadRight(10), Scene.Styles.FixedFontCyan);
                    text.Write(": ");
                    text.WriteLine(val, Scene.Styles.FixedFontYellow);
                }
                
                text.WriteLine();
                text.Write(">>> ", Scene.Styles.FixedFontDarkGray);
                text.Write(decode.OpCode, Scene.Styles.FixedFontYellow);
                text.Write($" ({decode.FriendlyName})", Scene.Styles.FixedFontGray);
                text.Write(" <<<", Scene.Styles.FixedFontDarkGray);
                text.WriteLine();
                
                
                // foreach (var arg in decode.Args.Where(x=>(x.InOut & InOut.Out) > 0))
                // {
                //     text.WriteLine($"OUT {arg.Text}");
                // }
                
                text.WriteLine();
                foreach (var arg in decode.Args.Where(x=>(x.InOut & InOut.Out) > 0))
                {
                    var val = Model.Alu.GetOutput(decode, arg);
                    
                    text.Write($"OUT: ");
                    text.Write(arg.Text.PadRight(10), Scene.Styles.FixedFontCyan);
                    text.Write(": ");
                    text.WriteLine(val, Scene.Styles.FixedFontYellow);
                }
                
                
                text.WriteLine();
                text.WriteLine($"Next: {Scene.Cpu.RIP.ValueHex}...");
                

            }
            
            // var i = Model.Inputs.ToArray();
            // if (i.Any())
            // {
            //     text.WriteLine();
            //     text.WriteLine("Input:");
            //
            //     foreach (var reg in i)
            //     {
            //         text.Write(reg.Id.PadRight(10));
            //         text.Write(": ");
            //         text.WriteLine(reg.ValueHex, Scene.Styles.FixedFontBlue);
            //     }    
            // }

           
        }

        
    }

    // public class ALUPhaseFetchElement : Element<Scene, object>
    // {
    //     public ALUPhaseFetchElement(IElement parent, object model) : base(parent, model, new DBlock())
    //     {
    //         Block.H = 150;
    //         Block.Set(5, 2, 5);
    //         
    //     }
    //
    //     protected override void Step(TimeSpan step)
    //     {
    //         
    //     }
    //
    //     protected override void Draw(DrawContext surface)
    //     {
    //         var drawing = new Drawing(surface.Canvas);
    //         drawing.DrawRect(Block, Scene.StyleFactory.GetPaint(this, "border"));
    //         
    //         drawing.DrawText(Model?.ToString(), Scene.StyleFactory.GetPaint(this, "h1"), Block, BlockAnchor.TM);
    //
    //         var mType    = Model.GetType();
    //         var txtStyle = Scene.StyleFactory.GetPaint(this, "text");
    //         var off      = new SKPoint(0,0);
    //         foreach (var prop in mType.GetProperties())
    //         {
    //             var pp  = prop.GetValue(Model);
    //             if (pp is {})
    //             {
    //                 if (pp is byte[] ba) pp = DisplayHelper.ToHex(ba);
    //
    //                 if (pp is IEnumerable en && pp is not string)
    //                 {
    //                     var ss   = drawing.DrawText(prop.Name, txtStyle, Block, BlockAnchor.MM, off);
    //                     off += new SKPoint(0, ss.Height + 2);
    //                     foreach (var  inner in en)
    //                     {
    //                         var txt = $"{inner}";
    //                         var s   = drawing.DrawText(txt, txtStyle, Block, BlockAnchor.MM, off);
    //                         off += new SKPoint(0, s.Height + 2);
    //                     }
    //                 }
    //                 else
    //                 {
    //                     var txt = $"{prop.Name}: {pp}";
    //                     var s   = drawing.DrawText(txt, txtStyle, Block, BlockAnchor.MM, off);
    //                     off += new SKPoint(0, s.Height + 2);    
    //                 }
    //                 
    //                 
    //                     
    //             }
    //         }
    //     }
    //}
}