using System;
using Animated.CPU.Animation;

namespace Animated.CPU.Model
{
    
    public class Terminal
    {
        
    }
    
    public class TerminalElement : Section<Scene, Terminal>
    {
        private TextBlockElement text;

        public TerminalElement(IElement parent, Terminal model, DBlock block) : base(parent, model, block)
        {
            Title = "Terminal";
        }

        protected override void Init()
        {
            this.text = Add(new TextBlockElement(this, Block, Scene.Styles.FixedFontDarkGray));
        }

        protected override void Step(TimeSpan step)
        {
            text.Clear();
            text.WriteLine($"Step: {Scene.Cpu?.Story?.CurrentIndex}");
            text.WriteLine($"Frames: {Scene.FrameCount}, elapsed {Scene.Elapsed:c} = {Scene.FPS:0.0} fps");
            text.WriteLine($"Mouse: {Scene.Mouse}");
        }
    }
}