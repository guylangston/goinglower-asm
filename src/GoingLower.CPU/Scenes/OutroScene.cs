using GoingLower.Core;
using GoingLower.Core.Elements.Effects;
using GoingLower.Core.Primitives;
using GoingLower.CPU.Animation;
using GoingLower.CPU.Model;

namespace GoingLower.CPU.Scenes
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