using System;
using System.Collections.Generic;
using Animated.CPU.Animation;
using SkiaSharp;

namespace Animated.CPU.Model
{


    public class ElementRegisterFile : Element<Scene, List<Register>>
    {
        public ElementRegisterFile(Scene scene, List<Register> model, DBlock block) : base(scene, model, block)
        {
            
        }

        public override void Init(SKSurface surface)
        {
            var stack = new DStack(this.Block, DOrient.Horz);

            foreach (var reg in stack.Divide(Model))
            {
                var r = Add(new ElementRegister(this, reg.model, reg.block));
                r.Block.Set(2, 2, 4, SKColor.Empty);
                r.Alpha.Value     = 0;
                r.Alpha.BaseValue = 255;
                r.IsHidden        = true;
                
                // Add Animation
                // Delay x
            }
        }

        public override void Step(TimeSpan step)
        {
            
        }
        
        public override void Draw(SKSurface surface)
        {
            
        }
    }

    public class PropFloat
    {
        public float Value { get; set; }
        public float BaseValue { get; set; }
    }
    
    public class PropInt
    {
        public int Value     { get; set; }
        public int BaseValue { get; set; }
    }
    

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
            Alpha.Value = (Alpha.Value+1) % 255;
        }

        public PropInt Alpha { get; } = new PropInt();
        
        public override void Draw(SKSurface surface)
        {
            var canvas = surface.Canvas;
            var draw   = new Drawing(canvas);
            
            var sName = Scene.StyleFactory.GetPaint(this, "Name");
            var sVal  = Scene.StyleFactory.GetPaint(this, "Value");
            var sId   = Scene.StyleFactory.GetPaint(this, "Id");
            var sBg   = Scene.StyleFactory.GetPaint(this, "bg");
            sBg = new SKPaint()
            {
                Color = new SKColor(100,100,100, (byte)Alpha.Value)
            };
                
            
            sName.Color = new SKColor(sName.Color.Red, sName.Color.Green, sName.Color.Blue, (byte)Alpha.Value);
            sVal.Color  = new SKColor(sVal.Color.Red, sVal.Color.Green, sVal.Color.Blue, (byte)Alpha.Value);
            sId.Color   = new SKColor(sId.Color.Red, sId.Color.Green, sId.Color.Blue, (byte)Alpha.Value);

            
            draw.DrawRect(Block, sBg);
            
            draw.DrawText($"[{Model.Id}]", sId, Block.Inner.TL);
            draw.DrawTextRight(Model.Name ?? "", sName, Block.Inner.TR + new SKPoint(-5, 0));
            draw.DrawTextRight(Model.Value.ToString("X"), sVal, Block.Inner.BR + new SKPoint(-5, -15));
        }
    }



}