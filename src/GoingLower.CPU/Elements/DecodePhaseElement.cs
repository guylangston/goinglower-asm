using System;
using GoingLower.Core;
using GoingLower.Core.Docking;
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
    public class DecodePhaseElement : Section<SceneExecute, PhaseDecode>
    {
        private TextBlockElement text;
        LogicUnitElement master;

        public DecodePhaseElement(IElement parent, PhaseDecode model, LogicUnitElement master) : base(parent, model, new DBlock()
        {
            H = 250
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
                text.Write(" ASM: ", Scene.Styles.FixedFontDarkGray);
                text.WriteLine(decode, Scene.Styles.FixedFontYellow);
                
                text.Write("Name: ", Scene.Styles.FixedFontDarkGray);
                text.Write(decode.FriendlyName, Scene.Styles.FixedFontWhite);
                text.WriteLine();
                
                text.WriteLine(decode.FriendlyMethod, Scene.Styles.SmallFont);
                if (decode.Description != null)
                {
                    text.WriteLine(decode.Description, Scene.Styles.SmallFont);
                }

                text.WriteLine();
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
                    var arr = new DockedArrow(
                        new DockPoint(this),
                        new DockPoint(eRip),
                        Scene.Styles.Arrow
                        );
                    arr.Step();
                    arr.Draw(surface.Canvas);
                   
                }    
            }
        }
    }
}