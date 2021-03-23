using GoingLower.Core.Drawing;
using GoingLower.Core.Primitives;

namespace GoingLower.Core.Elements.Scenes
{
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
}