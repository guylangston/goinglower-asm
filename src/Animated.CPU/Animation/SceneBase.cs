using System;
using System.Collections;
using System.Collections.Generic;
using SkiaSharp;

namespace Animated.CPU.Animation
{
    
    public abstract class SceneBase<TModel, TStyle> : ElementBase,  IScene where TStyle:IStyleFactory
    {
        protected SceneBase(TStyle styleFactory) 
        {
            StyleFactory = styleFactory;
            SetScene(this);
        }
        
        protected SceneBase(IScene scene,  DBlock b, TStyle styleFactory) 
        {
            StyleFactory = styleFactory;
            SetScene(this);
            Block = b;
        }

        public TimeSpan Elapsed      { get; private set; }
        public TStyle   StyleFactory { get; }
        public int      Steps        { get; private set; }

        IStyleFactory IScene.StyleFactory => StyleFactory;
        
        public new TModel Model
        {
            get => (TModel)base.Model;
            set => base.Model = value;
        }
        
        private bool init = false;

        protected abstract void InitScene(DrawContext drawing);
        
        public sealed override void Init(DrawContext surface)
        {
            // nothing,handled in Draw
        }

        public virtual void StepScene(TimeSpan s)
        {
            
        }

        protected sealed override void Step(TimeSpan step)
        {
            Elapsed += step;
            Steps++;
            
            StepScene(step);
            foreach (var element in ChildrenRecursive())
            {
                if (element == this) continue;

                element.Animator?.Step(step);
                element.StepExec(step);
            }
        }
        
        protected override void Draw(DrawContext surface)
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
                
                element.DrawExec(surface);
            }
            DrawOverlay(surface);
        }

        protected abstract void DrawOverlay(DrawContext drawing);
        protected abstract void DrawBackGround(DrawContext drawing);
        
        public abstract void KeyPress(object platformKeyObject, string key);





    }

}