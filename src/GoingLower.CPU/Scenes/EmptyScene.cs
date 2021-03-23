using GoingLower.Core;
using GoingLower.Core.Elements.Effects;
using GoingLower.Core.Elements.Scenes;
using GoingLower.Core.Helpers;
using GoingLower.Core.Primitives;
using GoingLower.CPU.Animation;

namespace GoingLower.CPU.Scenes
{
  
    
    // Get rid of the less used overrides

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
}
