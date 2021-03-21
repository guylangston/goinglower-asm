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
            
            // for (int cc = 0; cc < 60; cc++)
            //     Add(new BackGroundNoiseElement(this, Block));
            
            Add(new ParticleSystemElement(this, Block));

            base.InitScene();
            
         
        }
    }
}