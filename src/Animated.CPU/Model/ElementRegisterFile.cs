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
            
            foreach (var reg in stack.Layout(Model))
            {
                var r = Add(new ElementRegister(this, reg.model, reg.block));
                r.Block.Set(2, 2, 4, SKColor.Empty);
                r.Alpha.Value     = 0;
                r.Alpha.BaseValue = 180;
                
                
                // Add Animation
                r.Animator = new AnimatorPipeline(TimeSpan.FromSeconds(10));
                r.Animator.Add(new AnimationDelay(TimeSpan.FromSeconds(reg.index/8f)));
                r.Animator.Add(new AnimationProp(r.Alpha, 0, r.Alpha.BaseValue, TimeSpan.FromSeconds(1/2f)));
                r.Animator.Start();
            }

            

        }

        public override void Step(TimeSpan step)
        {
            var x = 1;
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
        
        }

        public PropFloat Alpha { get; } = new PropFloat();
        
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
            
            //draw.DrawText($"#{Alpha.Value}", new SKPaint() { Color = SKColors.Black, TextSize = 10}, Block.Inner.BL + new SKPoint(0, -15));
            
            
            
            draw.DrawText($"[{Model.Id}]", sId, Block, BlockAnchor.TL);
            draw.DrawText(Model.Name ?? "", sName, Block, BlockAnchor.TR);
            draw.DrawText(Model.Value.ToString("X"), sVal, Block, BlockAnchor.BR);
            
        }
    }



}