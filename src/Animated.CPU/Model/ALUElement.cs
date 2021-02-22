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
            
        }
    }


}