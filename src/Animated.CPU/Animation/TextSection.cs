using System;
using System.Collections.Generic;
using Animated.CPU.Model;
using Animated.CPU.Parsers;
using SkiaSharp;

namespace Animated.CPU.Animation
{

    public interface ITextStyleContainer
    {
        SKPaint Normal { get; }
    }
    
    public abstract class TextSection<TScene, TModel> : Section<TScene, TModel>, ITextStyleContainer where TScene:IScene
    {
        protected TextBlockElement text;
        protected SKPaint normal;
        protected SKPaint prefix;

        public TextSection(IElement parent, TModel model, DBlock block) : base(parent, model, block)
        {
            Title           = model?.ToString();
            ShowLineNumbers = true;
            IsSourceChanged = true;
            OnModelChanged = o => {
                IsSourceChanged = true;
            };
        }
        
        public SourceParser? Parser          { get; set; }
        public bool          IsSourceChanged { get; set; }
        public bool          ShowLineNumbers { get; set; }
        public SKPaint Normal => normal;

        protected abstract IReadOnlyList<string> GetLines(TModel model);
        
        protected override void Init()
        {
            normal = Scene.StyleFactory.GetPaint(this, "FixedFontGray");
            prefix = Scene.StyleFactory.GetPaint(this, "FixedFontDarkGray");

            text = Add(new TextBlockElement(this, this.Block, Normal));

            IsSourceChanged = true;
        }
        
        
        

        protected override void Step(TimeSpan step)
        {
            base.Step(step);

            if (IsSourceChanged)
            {
                var lines = GetLines(Model);
                if (lines == null) return;
                
                text.Clear();

                if (Parser != null)
                {
                    var  map = Parser.Parse(lines);
                    foreach (var line in map.Walk())
                    {
                        if (ShowLineNumbers && line.StartLine)
                        {
                            var sPrefix = text.Write($"{line.LineNo.ToString().PadLeft(3)}: ", prefix);
                            if (sPrefix != null)
                            {
                                sPrefix.SetModel(line.LineNo);
                            }    
                        }
                        
                        var style = line.Ident == null
                            ? Normal
                            : Scene.StyleFactory.GetPaint(this, $"font-{line.Ident.Colour ?? line.Ident.Name}");
                        
                        var span = text.Write(line.Text, style);
                        if (span != null)
                        {
                            span.SetModel(line.LineNo);
                            if (line.Token is SyntaxMarkDown.LinkIdentifier.Token link)
                            {
                                span.Url = link.Url;
                            }
                        }
                    }

                    IsSourceChanged = false;
                }
                else
                {
                    uint cc = 1;
                    foreach (var line in lines)
                    {
                        if (ShowLineNumbers)
                        {
                            var span = text.Write($"{cc.ToString().PadLeft(3)}: ", prefix);
                            if (span != null)
                            {
                                span.SetModel(cc);
                            }    
                        }
                        
                        text.WriteLine(line);
                    
                        cc++;
                    }

                    IsSourceChanged = false;    
                }
                
                

            }
            
            
        }

        public TextBlockElement.Span? GetLine(uint line)
        {
            if (text.TryGetSpanFromModel(line-1, out var s))
            {
                return s;
            }
            return null;
        }
        
    }
}