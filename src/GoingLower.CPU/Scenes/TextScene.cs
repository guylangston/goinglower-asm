using System.Collections.Generic;
using GoingLower.Core;
using GoingLower.Core.Elements.Scenes;
using GoingLower.Core.Helpers;
using GoingLower.Core.Primitives;
using GoingLower.CPU.Elements;
using GoingLower.CPU.Model;
using GoingLower.CPU.Parsers;

namespace GoingLower.CPU.Scenes
{
    public class TextScene : SimpleSceneBase
    {
        public TextScene(string name, IStyleFactory styleFactory, DBlock b, SourceFile fs) : base(name, styleFactory, b)
        {
            SourceFile = fs;
        }
        
        public SourceFile                         SourceFile { get;  }
        public TextSection<TextScene, SourceFile> Text       { get; protected set; }

        
        protected override void InitScene()
        {
            this.Text            = Add(new MyText(this, SourceFile, Block.Inset(100, 100).Set(0, 3, 50)));
            Text.Title           = "Links & Further Reading...";
            Text.ShowLineNumbers = false;
            Text.Parser          = new SourceParser(new SyntaxMarkDown());
        }

        class MyText : TextSection<TextScene, SourceFile>
        {
            public MyText(IElement parent, SourceFile model, DBlock block) : base(parent, model, block)
            {
            }
            
            
            protected override void Init()
            {
                base.Init();
                normal = Normal.CloneAndUpdate(x=>x.TextSize = Theme.TextSizeDefault + 5);
            }


            protected override IReadOnlyList<string> GetLines(SourceFile model) => model.Lines;
        }
    }
}