using System;
using System.Collections.Generic;
using Animated.CPU.Model;
using Animated.CPU.Parsers;
using SkiaSharp;

namespace Animated.CPU.Animation
{
    public abstract class TextSection<TScene, TModel> : Section<TScene, TModel> where TScene:IScene
    {
        protected TextBlockElement text;
        protected SKPaint normal;
        protected SKPaint prefix;

        public TextSection(IElement parent, TModel model, DBlock block) : base(parent, model, block)
        {
            Title = model?.ToString();
        }
        
        public SourceParser? Parser { get; set; }  

        protected abstract IReadOnlyList<string> GetLines(TModel model);
        
        protected override void Init()
        {
            normal = Scene.StyleFactory.GetPaint(this, "FixedFontGray");
            prefix = Scene.StyleFactory.GetPaint(this, "FixedFontDarkGray");

            text = Add(new TextBlockElement(this, this.Block, normal));

            IsSourceChanged = true;
        }
        
        public bool IsSourceChanged { get; set; }

        protected override void Step(TimeSpan step)
        {
            base.Step(step);

            if (IsSourceChanged)
            {
                text.Clear();

                if (Parser != null)
                {
                    var  map = Parser.Parse(GetLines(Model));
                    foreach (var line in map.Walk())
                    {
                        text.Write(line.txt, line.ident == null 
                            ? normal 
                            : Scene.StyleFactory.GetPaint(this, $"font-{line.ident.Colour ?? line.ident.Name}"));
                    }

                    IsSourceChanged = false;
                }
                else
                {
                    uint cc = 1;
                    foreach (var line in GetLines(Model))
                    {
                        text.Write($"{cc.ToString().PadLeft(3)}: ", prefix)
                            .SetModel(cc);
                        text.WriteLine(line);
                    
                        cc++;
                    }

                    IsSourceChanged = false;    
                }
                
                

            }
            
            
        }

        public TextBlockElement.Span? GetLine(uint line)
        {
            if (text.TryGetSpanFromModel(line, out var s))
            {
                return s;
            }
            return null;
        }
        
    }
}