using System;
using System.Collections.Generic;
using System.Linq;
using Animated.CPU.Animation;
using SkiaSharp;

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

        public object Status { get; set; }
        public Exception? Error  { get; set; }

        protected override void Init()
        {
            this.text = this.Add(new TextBlockElement(this, Block, Scene.Styles.FixedFontDarkGray));
        }

        private List<SKRect> decorate = new List<SKRect>(); 

        protected override void Step(TimeSpan step)
        {
            text.Clear();

            if (Status != null)
            {
                if (Status is FormattableString fs)
                {
                    text.WriteLineFormatted(fs);
                }
                else
                {
                    text.WriteLine(Status.ToString());
                }
            }

            if (Error != null)
            {
                text.WriteLine(Error.Message);
                text.WriteLine(Error.GetType().Name);
                text.WriteLine(Error.StackTrace);
            }
            
            
            text.WriteLineFormatted($"Step: {Scene.Cpu?.Story?.CurrentIndex}, Active: {Scene.ElementALU.StateMachine.Current}");
            text.WriteLineFormatted($"Frames: {Scene.FrameCount}, elapsed {Scene.Elapsed:c} = {Scene.FPS:0.0} fps");
            text.WriteLineFormatted($"Mouse: {Scene.DebugPointAt} | {Scene.DebugText}; KeyPress: {Scene.LastKeyPress}");
            if (Scene.DebugPointAt != SKPoint.Empty)
            {
                decorate.Clear();
                foreach (var element in Scene.ChildrenRecursive())
                {
                    var hit = element.GetSelectionAtPoint(Scene.DebugPointAt);
                    if (hit != null)
                    {
                        if (hit.Selection is TextBlockElement.Span span && span.Url != null)
                        {
                            text.WriteFormatted($"MOUSE ===> {element} URL: {span.Url}");    
                        }
                        else
                        {
                            text.WriteFormatted($"MOUSE ===> {element} SEL: {hit.Selection}");
                        }
                        text.WriteLine();

                    }
                }
                 
            }
         
            
        }

        protected override void Decorate(DrawContext surface)
        {
            foreach (var dec in decorate)
            {
                surface.Canvas.DrawRect(dec, Scene.Styles.debug);
            }
        }
    }
}