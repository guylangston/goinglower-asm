using System;
using GoingLower.Core.Primitives;

namespace GoingLower.Core
{
    public abstract class Element<TScene> : ElementBase where TScene:IScene
    {
        protected Element(TScene scene, DBlock? block) : base(scene, block)
        {
        }

        protected Element(IElement parent, DBlock? block) : base(parent, block)
        {
            
        }

        public new IElement Parent => base.Parent ?? throw new Exception("Only Scene should not parent==null");

        public new TScene Scene => (TScene)base.Scene;
    }

    public abstract class ElementFixedParent<TParent> : ElementBase where TParent: IElement
    {
        protected ElementFixedParent(TParent parent, DBlock? b) : base(parent, b)
        {
        }

        public new TParent Parent => (TParent)base.Parent;
    }
    
    public abstract class Element<TScene, TModel> : Element<TScene> where TScene:IScene
    {
        protected Element(TScene scene, TModel model, DBlock? block) : base(scene, block)
        {
            Model = model;
        }

        protected Element(IElement parent, TModel model, DBlock? block) : base(parent, block)
        {
            Model = model;
        }

        public virtual new TModel Model
        {
            get => (TModel)base.Model;
            set => base.Model = value;
        }
    }
}