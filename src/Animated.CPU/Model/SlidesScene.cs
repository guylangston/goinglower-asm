using System.Collections.Generic;
using System.Linq;
using Animated.CPU.Animation;
using Animated.CPU.Parsers;

namespace Animated.CPU.Model
{
    public class SlidesScene : SceneBase<Story, StyleFactory>
    {
        private MyText text;

        public SlidesScene(string name, StyleFactory styleFactory, DBlock block) : base(name, styleFactory, block)
        {
        }

        protected override void DrawOverlay(DrawContext drawing)
        {

        }

        protected override void DrawBackGround(DrawContext drawing)
        {

        }

        protected override void InitScene()
        {
            this.text            = Add(new MyText(this, Model.CurrentSlide, Block.Inset(100, 100).Set(0, 3, 50)));
            text.Title           = "Slides";
            text.ShowLineNumbers = false;
            text.Parser          = new SourceParser(new SyntaxMarkDown());
            text.IsSourceChanged = true;
        }

        protected override void InitSceneComplete()
        {

        }

        public override void ProcessEvent(string name, object args, object platform)
        {

        }

        public override void KeyPress(string key, object platformKeyObject)
        {
            if (key == "Left")
            {
                if (Model.CurrentSlideIndex > 0)
                {
                    Model.CurrentSlideIndex--;
                    text.Model           = Model.CurrentSlide;
                    text.IsSourceChanged = true;    
                }
                
            }
            if (key == "Right")
            {

                if (Model.CurrentSlideIndex < Model.Slides.Count - 1)
                {
                    Model.CurrentSlideIndex++;
                    text.Model           = Model.CurrentSlide;
                    text.IsSourceChanged = true;    
                }
                
            }
        }

        public override void MousePress(uint eventButton, double eventX, double eventY, object interop)
        {

        }

        class MyText : TextSection<SlidesScene, StoryAnnotation>
        {
            public MyText(IElement parent, StoryAnnotation model, DBlock block) : base(parent, model, block)
            {
                
            }

            protected override void Init()
            {
                base.Init();
                normal = Normal.CloneAndUpdate(x=>x.TextSize = CPU.Model.StyleFactory.TextSizeDefault + 5);
            }

            protected override IReadOnlyList<string> GetLines(StoryAnnotation model) =>
                StringHelper.ToLines(model.Text).ToArray();
        }
    }
}
