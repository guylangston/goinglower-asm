using System;
using Animated.CPU.Animation;
using SkiaSharp;

namespace Animated.CPU.Model
{
    public class DecodePhaseElement : Section<Scene, PhaseDecode>
    {
        private TextBlockElement text;
        ALUElement master;

        public DecodePhaseElement(IElement parent, PhaseDecode model, ALUElement master) : base(parent, model, new DBlock()
        {
            H = 200
        })
        {
            this.master = master;
            Block.Set(4, 1, 10);
            Block.Margin = new DBorder(30, 4, 4, 4);
            Title        = "Decode";
        }
        

        protected override void Init()
        {
            text = Add(new TextBlockElement(this, Block, Scene.Styles.FixedFont));
        }
        
        TextBlockElement.Span urlOpCode;

        protected override void Step(TimeSpan step)
        {
            IsEnabled     = !(master.StateMachine.Current == master.StateMachine.Fetch);
            IsHighlighted = (master.Active == this);
            
            text.Clear();
            var decode = Model.DecodeResult;
            if (decode != null)
            {
                text.Write("   ASM: ", Scene.Styles.FixedFontDarkGray);
                text.WriteLine(decode, Scene.Styles.FixedFontYellow);
                text.Write("  Name: \"", Scene.Styles.FixedFontDarkGray);
                text.Write(decode.FriendlyName, Scene.Styles.FixedFontWhite);
                text.WriteLine("\"", Scene.Styles.FixedFontDarkGray);
                text.Write("Pseudo: ", Scene.Styles.FixedFontDarkGray);
                text.WriteLine(decode.FriendlyMethod, Scene.Styles.FixedFontWhite);
                if (decode.Description != null)
                {
                    text.WriteLine(decode.Description, Scene.Styles.FixedFontGray);
                }
                
                urlOpCode = text.WriteUrl($"https://www.felixcloutier.com/x86/{decode.OpCode}#description", "WIKI");
            }
            
            
        }
        
        protected override void Decorate(DrawContext surface)
        {
            if (master.Active == this)
            {
                var seg = Scene.Cpu.Instructions.GetByAddress(master.Story.Current.RIP);
                if (seg != null && Scene.TryGetElementFromModelRecurse(seg, out var eRip))
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
    }
}