using Animated.CPU.Animation;

namespace Animated.CPU.Model
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
