using System;
using System.Collections.Generic;
using System.Linq;
using Animated.CPU.Animation;
using SkiaSharp;

namespace Animated.CPU.Model
{

    
    public class ElementRegisterFile : Section<Scene, List<Register>>
    {
        public ElementRegisterFile(IElement parent, List<Register> model, DBlock block) : base(parent, model, block)
        {
            Title = "Register File";
        }

        public override void Init()
        {
            var stack = new DStack(this.Block, DOrient.Horz);
            
            foreach (var reg in stack.Layout(Model))
            {
                var r = Add(new ElementRegister(this, reg.model, reg.block));
                r.Block.Set(4, 2, 4);
                r.Alpha.Value     = 0;
                r.Alpha.BaseValue = 180;
                
                
                // Add Animation
                r.Animator = new AnimatorPipeline(TimeSpan.FromSeconds(10));
                r.Animator.Add(new AnimationDelay(TimeSpan.FromSeconds(reg.index/8f)));
                r.Animator.Add(new AnimationProp(r.Alpha, 0, r.Alpha.BaseValue, TimeSpan.FromSeconds(1/2f)));
                r.Animator.Start();
            }

            

        }

        protected override void Step(TimeSpan step)
        {
            var x = 1;
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
        public ElementRegister(IElement parent, Register model, DBlock block) : base(parent, model, block)
        {
        }

        public override void Init()
        {
            var bytes = new byte[8];
            this.Bytes = Add(new ByteArrayElement(this, new ByteArrayModel(bytes, "", ""))
            {
                Block = Block.Inset(30, 25)
            });
        }
        
        public ByteArrayElement Bytes { get; set; }

        protected override void Step(TimeSpan step)
        {
            Bytes.IsHidden = Animator.IsActive;
            
            var bytes = BitConverter.GetBytes(Model.Value);
            Array.Reverse(bytes);   // hack
            Bytes.Model.Bytes       = bytes;
            Bytes.Model.ParsedValue = Model.Value.ToString("#,##0");

        }

        public PropFloat Alpha { get; } = new PropFloat();
        
        public bool IsHighlighted { get; set; }
        
        protected override void Draw(DrawContext surface)
        {
            var canvas = surface.Canvas;
            var draw   = new Drawing(canvas);
            
            var sName = Scene.Styles.GetPaint(this, "Name");
            var sVal  = Scene.Styles.GetPaint(this, "Value");
            var sId   = Scene.Styles.GetPaint(this, "Id");
            var sBg   = Scene.Styles.GetPaint(this, "bg");
            var i     = (byte)60;
            sBg = new SKPaint()
            {
                Color = new SKColor(i,i,i, (byte)Alpha.Value)
            };
                
            
            sName.Color = new SKColor(sName.Color.Red, sName.Color.Green, sName.Color.Blue, (byte)Alpha.Value);
            sVal.Color  = new SKColor(sVal.Color.Red, sVal.Color.Green, sVal.Color.Blue, (byte)Alpha.Value);
            sId.Color   = new SKColor(sId.Color.Red, sId.Color.Green, sId.Color.Blue, (byte)Alpha.Value);
            
            draw.DrawRect(Block, sBg);
            
            //draw.DrawText($"#{Alpha.Value}", new SKPaint() { Color = SKColors.Black, TextSize = 10}, Block.Inner.BL + new SKPoint(0, -15));
            
            draw.DrawText($"[{Model.Id}]", sId, Block, BlockAnchor.TL);
            draw.DrawText(Model.Name ?? "", sName, Block, BlockAnchor.TR);
            //draw.DrawText(Model.Value.ToString("X"), sVal, Block, BlockAnchor.BR);

            if (IsHighlighted)
            {
                // var high = new SKPaint()
                // {
                //     Style = SKPaintStyle.Fill,
                //     Shader = SKShader.CreateLinearGradient( 
                //         Block.Outer.TL + new SKPoint(-4, -4),
                //         Block.Outer.BR + new SKPoint(4, 40),
                //         new[]
                //         {
                //             SKColors.Orange,
                //             SKColors.Yellow,
                //             SKColors.Red
                //         },
                //         SKShaderTileMode.Repeat)
                // };
                draw.DrawHighlight(Block.Outer.ToSkRect(), Scene.Styles.Selected, 4);
            }
            
        }
    }
    
    



}