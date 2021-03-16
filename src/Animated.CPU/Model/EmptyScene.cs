using Animated.CPU.Animation;

namespace Animated.CPU.Model
{
    public class EmptyScene : SceneBase<string, StyleFactory>
    {
        public EmptyScene(string name, StyleFactory styleFactory, DBlock block) : base(name, styleFactory, block)
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

        protected override void InitScene()
        {
            for (int cc = 0; cc < 100; cc++)
                Add(new BackGroundNoise(this, Block));
        }

        protected override void InitSceneComplete()
        {

        }
    }
}