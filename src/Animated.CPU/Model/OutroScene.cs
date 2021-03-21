using Animated.CPU.Animation;

namespace Animated.CPU.Model
{
    public class OutroScene : TextScene
    {
        public OutroScene(string name, IStyleFactory styleFactory, DBlock b, SourceFile fs) : base(name, styleFactory, b, fs)
        {
        }

        protected override void InitScene()
        {
            Add(new ParticleSystemElement(this, Block, 70));
            
            base.InitScene();
        }
    }
}