using System;
using System.Linq;
using Animated.CPU.Animation;
using SkiaSharp;

namespace Animated.CPU.Model
{
    public class ALUElement : Element<Scene, ArithmeticLogicUnit>
    {

        public ALUElement(Scene scene, ArithmeticLogicUnit alu, DBlock b) : base(scene, alu, b)
        {
        }
        public override void Init(SKSurface surface)
        {
            var stack = new DStack(this.Block, DOrient.Horz);

            foreach (var item in stack.Layout<object>(Model.Phases().ToList()))
            {
                item.block.Set(4, 2, 4, null);
                var e = Add(new BlockElement(this, item.block)
                {
                    Model = item.model,
                    Anchor = BlockAnchor.TM
                });
            }
        }
        public override void Step(TimeSpan step)
        {
            
        }
        public override void Draw(SKSurface surface)
        {
            var drawing = new Drawing(surface.Canvas);
            var sText   = Scene.StyleFactory.GetPaint(this, "text");
            
            
            var ss = Scene.Model.Story.Current;
            if (ss != null)
            {
                Scene.TryGetElementFromModel(Scene.Model.RIP, out var eRIP);
                drawing.DrawText($"Fetch(RIP)={DisplayHelper.ToHex64(ss.RIP)}", sText, GetChild<BlockElement>(0).Block, BlockAnchor.MM);
                new Arrow()
                {
                    Start = eRIP.Block.Outer.MR,
                    End   = GetChild<BlockElement>(0).Block.Inner.MM,
                    Style = Scene.StyleFactory.GetPaint(this, "arrow")
                }.Draw(surface.Canvas);

                Scene.TryGetElementFromModel(Scene.Model.Instructions.GetByAddress(ss.RIP), out var eSeg);
                new Arrow()
                {
                    Start = eSeg.Block.Outer.ML,
                    End   = GetChild<BlockElement>(0).Block.Inner.MM,
                    Style = Scene.StyleFactory.GetPaint(this, "arrow")
                }.Draw(surface.Canvas);
                
                
                drawing.DrawText(ss.Asm, sText, GetChild<BlockElement>(1).Block, BlockAnchor.MM);
                
                var o = new SKPoint();
                foreach (var reg in Scene.Model.RegisterFile.Where(x=>x.IsChanged))
                {
                    if (reg == Scene.Model.RIP) continue;
                    
                    
                    var s = drawing.DrawText(reg.ToString(), sText, GetChild<BlockElement>(2).Block, BlockAnchor.ML, o);
                    o += new SKPoint(0, s.Height+5);
                }
                
            }
            
            
            
        }
    }


}