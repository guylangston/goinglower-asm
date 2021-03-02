using System;
using System.Collections;
using System.Collections.Generic;
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
                
                text.Write("RIP".PadRight(12));
                text.Write(": ");
                text.WriteLine(DisplayHelper.ToHex(Model.RIP), Scene.StyleFactory.FixedFontCyan);
                
                text.Write("Binary Code".PadRight(12));
                text.Write(": ");
                text.WriteLine(DisplayHelper.ToHex(Model.Memory), Scene.StyleFactory.FixedFontBlue);    
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
            Block.Set(4, 1, 40);
            Title = "Decode";
        }

        public override void Init(DrawContext surface)
        {
            text = Add(new TextBlockElement(Scene, this, Block, Scene.StyleFactory.FixedFont));
        }

        protected override void Step(TimeSpan step)
        {
            text.Clear();
            var decode = Model.Asm;
            if (decode != null)
            {
                text.WriteLine(decode);
                text.WriteLine();
                text.WriteLine(decode.FriendlyName, Scene.StyleFactory.FixedFontBlue);
                text.WriteLine(decode.FriendlyMethod, Scene.StyleFactory.FixedFontBlue);
            }
            
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
            Block.Set(4, 1, 40);
            Title = "Execute";
        }

        public override void Init(DrawContext surface)
        {
            text = Add(new TextBlockElement(Scene, this, Block, Scene.StyleFactory.FixedFont));
        }

        protected override void Step(TimeSpan step)
        {
            text.Clear();
            
            var i = Model.Inputs.ToArray();
            if (i.Any())
            {
                text.WriteLine();
                text.WriteLine("Input:");
            
                foreach (var reg in i)
                {
                    text.Write(reg.Id.PadRight(10));
                    text.Write(": ");
                    text.WriteLine(reg.ValueHex, Scene.StyleFactory.FixedFontBlue);
                }    
            }

            var c = Model.Changes.ToArray();
            if (c.Any())
            {
                text.WriteLine();
                text.WriteLine("Output:");
                foreach (var reg in c)
                {
                    text.Write(reg.Id.PadRight(10));
                    text.Write(": ");
                    text.WriteLine(reg.ValueHex, Scene.StyleFactory.FixedFontCyan);
                }    
            }
            
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