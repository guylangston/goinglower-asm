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
            H = 120
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

        protected override void Step(TimeSpan step)
        {
            IsEnabled     = !(master.StateMachine.Current == master.StateMachine.Fetch);
            IsHighlighted = (master.Active == this);
            
            text.Clear();
            var decode = Model.Asm;
            if (decode != null)
            {
                text.WriteLine(decode);
                text.WriteLine(decode.FriendlyName, Scene.Styles.FixedFontBlue);
                text.WriteLine(decode.FriendlyMethod, Scene.Styles.FixedFontBlue);
            }
        }
        
        protected override void Decorate(DrawContext surface)
        {
            if (master.Active == this)
            {
                var seg = Scene.Cpu.Instructions.GetByAddress(master.Story.Current.RIP);
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
    }
}