using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Text;
using Animated.CPU.Animation;
using SkiaSharp;

namespace Animated.CPU.Model
{

    public class ElementRegister : Element<Scene, Register>
    {

        public ElementRegister(Scene scene, Register model) : base(scene, model)
        {
        }
        public ElementRegister(IElement parent, Register model) : base(parent, model)
        {
        }
        public ElementRegister(Scene scene, Register model, DBlock block) : base(scene, model, block)
        {
        }
        public ElementRegister(IElement parent, Register model, DBlock block) : base(parent, model, block)
        {
        }
        public override void Step(TimeSpan step)
        {
            
        }
        public override void Draw(SKSurface surface)
        {
            var canvas = surface.Canvas;
            var draw   = new Drawing(canvas);
            
            // map to scheme
            int cc = 0;


            // var itemPaint = new DBlockProps().Set(2, 2, 2, new SKPaint()
            // {
            //     Style = SKPaintStyle.Stroke,
            //     Color = SKColors.SkyBlue,
            // });
            
            
            draw.DrawRect(Block);
            
            draw.DrawText($"[{Model.Id}]", Scene.StyleFactory.GetPaint(this, "Id"), Block.Inner.TL);
            draw.DrawTextRight(Model.Name ?? "", Scene.StyleFactory.GetPaint(this, "Name"), Block.Inner.TR + new SKPoint(-5, 0));
            draw.DrawTextRight(Model.Value.ToString("X"), Scene.StyleFactory.GetPaint(this, "Value"), Block.Inner.BR + new SKPoint(-5, -15));
          
            
        }
    }


    public class ElementRegisterFile : Element<Scene, List<Register>>
    {
        public ElementRegisterFile(Scene scene, List<Register> model, DBlock block) : base(scene, model, block)
        {
            
        }



        public override void Init(SKSurface surface)
        {
            var stack = new DStack(this.Block, DOrient.Horz);
            ;
            
            foreach (var reg in stack.Divide(Model))
            {
                Add(new ElementRegister(this, reg.model, reg.block));
            }
        }

        public override void Step(TimeSpan step)
        {
            
        }
        
        public override void Draw(SKSurface surface)
        {
            
        }
    }

    

}