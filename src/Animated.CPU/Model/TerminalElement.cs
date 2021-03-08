using System;
using System.Linq;
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
            this.text = this.Add(new TextBlockElement(this, Block, Scene.Styles.FixedFontDarkGray));
        }

        protected override void Step(TimeSpan step)
        {
            text.Clear();
            text.WriteLine($"Step: {Scene.Cpu?.Story?.CurrentIndex}, Active: {Scene.ElementALU.StateMachine.Current}");
            text.WriteLine($"Frames: {Scene.FrameCount}, elapsed {Scene.Elapsed:c} = {Scene.FPS:0.0} fps");
            text.WriteLine($"Mouse: {Scene.Mouse}; KeyPress: {Scene.LastKeyPress}");
            if (Scene.DebugHits.Any())
            {
                foreach (var hit in Scene.DebugHits)
                {
                    text.WriteLine(hit.ToString());
                }
                
            }
            text.WriteLine();
            if (Scene.Cpu?.Story?.ReadMe != null)
            {
                foreach (var line in Scene.Cpu.Story.ReadMe)
                {
                    text.WriteLine(line);
                }
            }

        }
    }
}