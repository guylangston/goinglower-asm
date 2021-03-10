using System;
using System.Text.Encodings.Web;
using Animated.CPU.Animation;
using SkiaSharp;

namespace Animated.CPU.Model
{
    public class FetchPhaseElement : Section<Scene, PhaseFetch>
    {
        private TextBlockElement text;
        ALUElement master;

        public FetchPhaseElement(IElement parent, PhaseFetch model, ALUElement master) : base(parent, model, new DBlock()
        {
            H = 100
        })
        {
            this.master = master;
            Title       = "Fetch";
            Block.Set(4, 1, 10);
            Block.Margin = new DBorder(10, 4, 4, 4);
        }
        

        
        protected override void Init()
        {
            text = Add(new TextBlockElement(this, Block, Scene.Styles.FixedFont));
        }

        protected override void Step(TimeSpan step)
        {
            IsHighlighted = (master.Active == this);
            if (Model.Memory != null)
            {
                text.Clear();

                text.Write("Instruction Ptr".PadRight(12) + ": ", Scene.Styles.FixedFontDarkGray);
                text.WriteLine(DisplayHelper.ToHex(Model.RIP), Scene.Styles.FixedFontWhite);

                text.Write("Machine Code".PadRight(12) + ": ", Scene.Styles.FixedFontDarkGray);
                text.WriteLine(DisplayHelper.ToHex(Model.Memory), Scene.Styles.FixedFontYellow);
            }
            //Block.H = Math.Max(60, text.LastDrawHeight);
        }

      

        protected override void Decorate(DrawContext surface)
        {
            if (master.Active == this)
            {

                var seg = Scene.Cpu.Instructions.GetByAddress(Model.RIP);
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