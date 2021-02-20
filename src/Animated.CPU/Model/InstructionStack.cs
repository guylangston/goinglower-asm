using System;
using System.Collections.Generic;
using Animated.CPU.Animation;
using SkiaSharp;

namespace Animated.CPU.Model
{
    public class InstructionStack : Element<Scene>
    {

        private List<string> items = new List<string>();
        private int curr = 20;

        private CountDown cd = new CountDown(1.2f);

        private DBlock block;


        public InstructionStack(Scene scene, DBlock block) : base(scene, block)
        {
            this.block = block;
            for (int cc = 0; cc < 100; cc++)
            {
                items.Add($"add rax, {cc}");
            }
        }


        public override void Step(TimeSpan step)
        {
            if (cd.Step(step))
            {
                curr++;
                if (curr >= items.Count) curr = 0;
            }
        }

        public override void Draw(SKSurface surface)
        {
            var d = new Drawing(surface.Canvas);
            d.DrawRect(Scene.p1, block.Inner);

            d.DrawTextCenter(items[curr], Scene.debug, block.Inner.MM);
            
            // Prev
            var cc = curr - 1;
            while (cc > 0)
            {
                d.DrawTextCenter(items[cc], Scene.debug, block.Inner.MM - new SKPoint(0, (curr - cc)*20));
                cc--;
            }
            
            // next
            cc = curr + 1;
            while (cc < items.Count)
            {
                d.DrawTextCenter(items[cc], Scene.debug, block.Inner.MM - new SKPoint(0, (curr - cc)*20));
                cc++;
            }
        }
        
    
    }
}