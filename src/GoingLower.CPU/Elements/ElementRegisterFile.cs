using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using GoingLower.Core;
using GoingLower.Core.Animation;
using GoingLower.Core.Drawing;
using GoingLower.Core.Elements;
using GoingLower.Core.Elements.Sections;
using GoingLower.Core.Helpers;
using GoingLower.Core.Primitives;
using GoingLower.CPU.Animation;
using GoingLower.CPU.Model;
using GoingLower.CPU.Scenes;
using SkiaSharp;

namespace GoingLower.CPU.Elements
{

    
    public class ElementRegisterFile : Section<SceneExecute, List<Register>>
    {
        public ElementRegisterFile(IElement parent, List<Register> model, DBlock block) : base(parent, model, block)
        {
            Title       = "Registers";
            TitleAction = GetType().Name;
        }

        protected override void Init()
        {
            var stack = Add(new StackElement(this, Block, DOrient.Vert)
            {
                SkipHidden = false
            });
            
            
            foreach (var reg in Model.WithIndex())
            {
                if (reg.val.IsExtendedReg) continue;
                var r = stack.Add(new ElementRegister(stack, reg.val,
                    new DBlock(0,0,0,50).Set(2,1,2)));
                r.Alpha.Value     = 0;
                r.Alpha.BaseValue = 255;
                
                // Add Animation
                r.Animator = new AnimatorPipeline(TimeSpan.FromSeconds(10));
                r.Animator.Add(new AnimationBaseDelay(TimeSpan.FromSeconds(reg.index/32f)));
                r.Animator.Add(new AnimationBaseProp(r.Alpha, 0, r.Alpha.BaseValue, TimeSpan.FromSeconds(1/2f)));
                r.Animator.Start();
            }
        }
    }

    public class ElementRegister : Element<SceneExecute, Register>
    {
        private TextBlockElement text;

        public ElementRegister(IElement parent, Register model, DBlock block) : base(parent, model, block)
        {
            IsHidden = true;
        }
        
     
        public PropFloat Alpha         { get; } = new PropFloat(0,0);
        public bool      IsHighlighted { get; set; }

        protected override void Init()
        {
            this.text = Add(new TextBlockElement(this, Block , Scene.Styles.FixedFont)
            {
                Grow = true
            });
            
            // var bytes = new byte[8];
            // this.Bytes = Add(new ByteArrayElement(this, new ByteArrayM]odel(bytes, "", ""))
            // {
            //     Block = Block.Inset(30, 25)
            // });
        }
        
        //public ByteArrayElement Bytes { get; set; }

        

        protected override void Step(TimeSpan step)
        {
            if (Model == Scene.Cpu.RIP)
            {
                // Always show
                IsHidden        = false;
                Model.IsChanged = false;
                IsHighlighted   = false;
            }
            if (Model.IsChanged)
            {
                IsHidden = false;
            }
            IsHighlighted = Model.IsChanged;
            
            text.Clear();
            text.Write($"{Model.Id}", Scene.Styles.FixedFontYellow);
            text.Write($" {Model.Name}");
            text.WriteLine();
            
            if (Model is FlagsRegister flags)
            {
                var xx = 0;
                foreach (var (name, val) in flags.GetFlags().Reverse())
                {
                    if (xx > 0) text.Write(" ");
                    var span = text.Write(name[1..3], val ? Scene.Styles.FixedFontArg :  Scene.Styles.FixedFontDarkGray);
                    span.Model = name;
                    xx++;
                }
                text.WriteLine();
                text.WriteLine(Convert.ToString((long)flags.Value, 2).PadLeft(12, '0').Replace("0", " 0 ").Replace("1", " 1 "));
                
                foreach (var (name, _) in flags.GetFlags().Where(x=>x.val))
                {
                    if (name.StartsWith("(IF)")) continue;
                    
                    var span = text.WriteLine(name, Scene.Styles.FixedFontBlue);
                }
                
                text.WriteUrl("https://en.wikipedia.org/wiki/FLAGS_register", "WIKI");
                text.WriteLine();
            }
            else
            {
                
                text.Write($"HEX:", Scene.Styles.FixedFontDarkGray);
                text.WriteHexWords(Model.Value, Model.LastUsedAsSize);
                if (Model.LastUsedAs != null)
                {
                    text.Write(" [", Scene.Styles.FixedFontDarkGray);
                    text.Write(Model.LastUsedAs, Scene.Styles.FixedFontYellow);
                    text.Write("]", Scene.Styles.FixedFontDarkGray);
                }
                
                text.WriteLine();

                if (true) //IsHighlighted)
                {
                    if (Model.Value < 10000)
                    {
                        text.Write($"DEC:", Scene.Styles.FixedFontDarkGray);
                        text.Write($"{Model.Value:#,##0}", Scene.Styles.FixedFontCyan);    
                    }
                    
                    
                    if (Model.TagValue != null)
                    {
                        text.Write(" <", Scene.Styles.FixedFontDarkGray);
                        text.Write(Model.TagValue, Scene.Styles.FixedFontSource);
                        text.Write(">", Scene.Styles.FixedFontDarkGray);
                    }
                }
                text.WriteLine();

                // if (IsHighlighted)
                // {
                //     text.Write($"BIN: ", Scene.Styles.FixedFontDarkGray);
                //     text.WriteLine(Convert.ToString((long)Model.Value, 2), Scene.Styles.SmallFont);    
                // }
            }

            text.Resize();
            Block = text.Block;
            
            
        }


        protected override void Draw(DrawContext surface)
        {
            surface.Canvas.DrawRect(Block.Inner.ToSkRect(), Scene.Styles.BackGroundAlt);
            
            if (IsHighlighted)
            {
                surface.Canvas.DrawRect(Block.BorderDRect.ToSkRect(), Scene.Styles.Selected);
            }

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
            
            
        }
    }
    
    



}