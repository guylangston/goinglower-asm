namespace Animated.CPU.Animation
{
    public abstract class Element<TScene> : ElementBase where TScene:IScene
    {
        protected Element(TScene scene) : base(scene, scene)
        {
        }

        protected Element(IElement parent) : base((TScene)parent.Scene, parent)
        {
        }

        protected Element(TScene scene, DBlock block) : base(scene, scene, block)
        {
        }

        protected Element(IElement parent, DBlock block) : base((TScene)parent.Scene, parent, block)
        {
        }

        public new TScene Scene => (TScene)base.Scene;
    }
    
    public abstract class Element<TScene, TModel> : Element<TScene> where TScene:IScene
    {
        protected Element(TScene scene, TModel model) : base(scene)
        {
            Model = model;
        }

        protected Element(IElement parent, TModel model) : base(parent)
        {
            Model = model;
        }

        protected Element(TScene scene, TModel model, DBlock block) : base(scene, block)
        {
            Model = model;
        }

        protected Element(IElement parent, TModel model, DBlock block) : base(parent, block)
        {
            Model = model;
        }

        public new TModel Model
        {
            get => (TModel)base.Model;
            set => base.Model = value;
        }
    }
}