using System;
using System.Collections.Generic;
using System.IO.Compression;
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

        protected override void Init()
        {
            var stack = Add(new StackElement(this, Block, DOrient.Vert));
            
            foreach (var reg in Model.WithIndex())
            {
                if (reg.val.IsExtendedReg) continue;
                var r = stack.Add(new ElementRegister(stack, reg.val,
                    new DBlock(0,0,0,50).Set(2,1,2)));
                r.Alpha.Value     = 0;
                r.Alpha.BaseValue = 255;
                
                // Add Animation
                r.Animator = new AnimatorPipeline(TimeSpan.FromSeconds(10));
                r.Animator.Add(new AnimationDelay(TimeSpan.FromSeconds(reg.index/32f)));
                r.Animator.Add(new AnimationProp(r.Alpha, 0, r.Alpha.BaseValue, TimeSpan.FromSeconds(1/2f)));
                r.Animator.Start();
            }
        }
    }
    

    public class ElementRegister : Element<Scene, Register>
    {
        private TextBlockElement text;

        public ElementRegister(IElement parent, Register model, DBlock block) : base(parent, model, block)
        {
        }

        protected override void Init()
        {
            this.text = Add(new TextBlockElement(this, Block , Scene.Styles.FixedFont)
            {
                Grow = true
            });
            
            // var bytes = new byte[8];
            // this.Bytes = Add(new ByteArrayElement(this, new ByteArrayModel(bytes, "", ""))
            // {
            //     Block = Block.Inset(30, 25)
            // });
        }
        
        //public ByteArrayElement Bytes { get; set; }

        protected override void Step(TimeSpan step)
        {
            //text.IsHidden = Animator.IsActive;
            text.Clear();
            text.Write($"{Model.Name} ");
            text.Write($"{Model.Id}", Scene.Styles.FixedFontWhite);
            text.WriteLine();
            text.Write($"{Model.ValueHex}");
            text.WriteLine();
            if (Model.IsChanged)
            {
                text.WriteLine($"{Model.Value}", Scene.Styles.FixedFontCyan);
            }
            
            // var bytes = BitConverter.GetBytes(Model.Value);
            // Array.Reverse(bytes);   // hack
            // Bytes.Model.Bytes       = bytes;
            //
            // if (Model.Value < 10_000) // assume address otherwise
            // {
            //     Bytes.Model.ParsedValue = Model.Value.ToString("#,##0");    
            // }

            IsHighlighted = Model.IsChanged;

            Block = text.Block;


        }

        public PropFloat Alpha { get; } = new PropFloat();
        
        public bool IsHighlighted { get; set; }
        
        protected override void Draw(DrawContext surface)
        {
            var canvas = surface.Canvas;
            var draw   = new Drawing(canvas);

            //
            //
            // var sName = Scene.Styles.GetPaint(this, "Name");
            // var sVal  = Scene.Styles.GetPaint(this, "Value");
            // var sId   = Scene.Styles.GetPaint(this, "Id");
            // var sBg   = Scene.Styles.GetPaint(this, "bg");
            // var i     = (byte)60;
            // sBg = new SKPaint()
            // {
            //     Color = new SKColor(i,i,i, (byte)Alpha.Value)
            // };
            //     
            //
            // sName.Color = new SKColor(sName.Color.Red, sName.Color.Green, sName.Color.Blue, (byte)Alpha.Value);
            // sVal.Color  = new SKColor(sVal.Color.Red, sVal.Color.Green, sVal.Color.Blue, (byte)Alpha.Value);
            // sId.Color   = new SKColor(sId.Color.Red, sId.Color.Green, sId.Color.Blue, (byte)Alpha.Value);
            //
            // draw.DrawRect(Block, sBg);
            //
            // //draw.DrawText($"#{Alpha.Value}", new SKPaint() { Color = SKColors.Black, TextSize = 10}, Block.Inner.BL + new SKPoint(0, -15));
            //
            // draw.DrawText($"[{Model.Id}]", sId, Block, BlockAnchor.TL);
            // draw.DrawText(Model.Name ?? "", sName, Block, BlockAnchor.TR);
            // //draw.DrawText(Model.Value.ToString("X"), sVal, Block, BlockAnchor.BR);
            //
            //
            
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
                draw.Canvas.DrawRect(Block.BorderRect.ToSkRect(), Scene.Styles.Selected);
            }
            
        }
    }
    
    



}