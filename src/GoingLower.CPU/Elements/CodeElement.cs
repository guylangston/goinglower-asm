using System;
using System.Collections.Generic;
using System.Linq;
using GoingLower.Core;
using GoingLower.Core.Elements;
using GoingLower.Core.Primitives;
using GoingLower.CPU.Animation;
using GoingLower.CPU.Model;
using GoingLower.CPU.Parsers;
using GoingLower.CPU.Scenes;
using SkiaSharp;

namespace GoingLower.CPU.Elements
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