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

        public Exception? Error { get; set; }

        protected override void Init()
        {
            this.text = this.Add(new TextBlockElement(this, Block, Scene.Styles.FixedFontDarkGray));
        }

        private List<SKRect> decorate = new List<SKRect>(); 

        protected override void Step(TimeSpan step)
        {
            text.Clear();

            if (Error != null)
            {
                text.WriteLine(Error.Message);
                text.WriteLine(Error.GetType().Name);
                text.WriteLine(Error.StackTrace);
            }
            
            
            text.WriteLineWithHighlights($"Step: {Scene.Cpu?.Story?.CurrentIndex}, Active: {Scene.ElementALU.StateMachine.Current}");
            text.WriteLineWithHighlights($"Frames: {Scene.FrameCount}, elapsed {Scene.Elapsed:c} = {Scene.FPS:0.0} fps");
            text.WriteLineWithHighlights($"Mouse: {Scene.DebugPointAt} | {Scene.DebugText}; KeyPress: {Scene.LastKeyPress}");
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
                            text.WriteWithHighlights($"MOUSE ===> {element} URL: {span.Url}");    
                        }
                        else
                        {
                            text.WriteWithHighlights($"MOUSE ===> {element} SEL: {hit.Selection}");
                        }
                        text.WriteLine();

                    }
                }
                 
            }
            // if (Scene.DebugHits.Any())
            // {
            //     foreach (var hit in Scene.DebugHits)
            //     {
            //         text.WriteLine(hit.ToString());
            //     }
            //     
            // }
            text.WriteLine();
            if (Scene.Cpu?.Story?.ReadMe != null)
            {
                foreach (var line in Scene.Cpu.Story.ReadMe)
                {
                    text.WriteLine(line);
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