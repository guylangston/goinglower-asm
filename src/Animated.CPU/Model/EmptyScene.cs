using System.Collections.Generic;
using Animated.CPU.Animation;
using Animated.CPU.Parsers;

namespace Animated.CPU.Model
{
    
    
    // Get rid of the less used overrides
    public abstract class SimpleSceneBase : SceneBase
    {
        protected SimpleSceneBase(string name, IStyleFactory styleFactory, DBlock b) : base(name, styleFactory, b)
        {
        }

        protected override void DrawOverlay(DrawContext drawing)
        {

        }

        protected override void DrawBackGround(DrawContext drawing)
        {

        }
     
        public override void ProcessEvent(string name, object args, object platform)
        {

        }

        public override void KeyPress(string key, object platformKeyObject)
        {

        }

        public override void MousePress(uint eventButton, double eventX, double eventY, object interop)
        {

        }

        protected override void InitSceneComplete()
        {

        }
    }
    
    public class EmptyScene : SimpleSceneBase
    {
        public EmptyScene(string name, IStyleFactory styleFactory, DBlock b) : base(name, styleFactory, b)
        {
        }

        protected override void InitScene()
        {
            for (int cc = 0; cc < 100; cc++)
                Add(new BackGroundNoiseElement(this, Block));
        }

    }
    
    
    
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

            protected override IReadOnlyList<string> GetLines(SourceFile model) => model.Lines;
        }
    }
}
