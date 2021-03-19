using System;
using System.Collections.Generic;
using System.Linq;
using Animated.CPU.Animation;
using Animated.CPU.Parsers;
using SkiaSharp;

namespace Animated.CPU.Model.ModelElements
{
    public class CodeElement : TextSection<SceneExecute, SourceFile>
    {


        public CodeElement(IElement parent, SourceFile model, DBlock block) : base(parent, model, block)
        {
            Title  = model.Title ?? model.Name;
            Parser = new SourceParser(new SyntaxCSharp());

        }

        protected override IReadOnlyList<string> GetLines(SourceFile model)
        {
            return model.Lines;
        }

        protected override void Init()
        {
            normal = Scene.StyleFactory.GetPaint(this, "FixedFontGray");
            prefix = Scene.StyleFactory.GetPaint(this, "FixedFontDarkGray");

            text = Add(new TextBlockElement(this, this.Block, Normal));

            IsSourceChanged = true;
        }


    }
}