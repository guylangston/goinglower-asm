using System;
using System.Collections;
using System.Collections.Generic;
using SkiaSharp;

namespace Animated.CPU.Animation
{
    
    public abstract class SceneBase<TModel> : ElementBase,  IScene
    {
        protected SceneBase(IStyleFactory styleFactory) 
        {
            StyleFactory = styleFactory;
            SetScene(this);
        }
        
        protected SceneBase(IScene scene,  DBlock b, IStyleFactory styleFactory) 
        {
            StyleFactory = styleFactory;
            SetScene(this);
            Block = b;
        }

        public TimeSpan      Elapsed      { get; private set; }
        public IStyleFactory StyleFactory { get; private set; }
        public int           Steps        { get; private set; }
        
        public new TModel Model
        {
            get => (TModel)base.Model;
            set => base.Model = value;
        }
        
        private bool init = false;

        protected abstract void InitScene(SKSurface surface);
        
        public sealed override void Init(SKSurface surface)
        {
            // nothing,handled in Draw
        }

        public sealed override void Step(TimeSpan step)
        {
            Elapsed += step;
            Steps++;
            foreach (var element in ChildrenRecursive())
            {
                if (element == this) continue;
                element.Step(step);
            }
        }
        
        public sealed override void Draw(SKSurface surface)
        {
            if (!init)
            {
                InitScene(surface);
                foreach (var element in ChildrenRecursive())
                {
                    element.Init(surface);
                }
                init = true;
            }
            DrawBackGround(surface);
            foreach (var element in ChildrenRecursive())
            {
                if (element == this) continue;
                if (element is ElementBase eb && eb.IsHidden) continue;
                
                element.Draw(surface);
            }
            DrawOverlay(surface);
        }

        protected abstract void DrawOverlay(SKSurface surface);
        protected abstract void DrawBackGround(SKSurface surface);
        
        
    }

}