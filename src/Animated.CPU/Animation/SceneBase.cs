using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SkiaSharp;

namespace Animated.CPU.Animation
{
    public abstract class SceneBase : ElementBase, IScene 
    {
        public TimeSpan      Elapsed      { get; protected set; }
        public IStyleFactory StyleFactory { get; protected set; }

        public int      FrameCount { get; protected set; }
        public float    FPS        => (float)FrameCount / (float)Elapsed.TotalSeconds ;
        
        // Debugging
        public SKPoint        DebugPointAt { get; set; }
        public uint           DebugButton  { get; set; }
        public List<IElement> DebugHits    { get; } = new List<IElement>();

        
        protected abstract void DrawOverlay(DrawContext drawing);
        protected abstract void DrawBackGround(DrawContext drawing);

        public abstract void ProcessEvent(object platform, string name, object args);
        public abstract void KeyPress(object platformKeyObject, string key);
        public abstract void MousePress(uint eventButton, double eventX, double eventY, object interop);
    }
    
    
    public abstract class SceneBase<TModel, TStyle> : SceneBase where TStyle:IStyleFactory
    {
        protected SceneBase(TStyle styleFactory)  
        {
            StyleFactory = Styles = styleFactory;
            SetScene(this);
        }
        
        protected SceneBase(IScene scene,  DBlock b, TStyle styleFactory) 
        {
            StyleFactory = Styles = styleFactory;
            SetScene(this);
            Block = b;
        }

        
        public  TStyle Styles { get; }
        
        
        public new TModel Model
        {
            get => (TModel)base.Model;
            set => base.Model = value;
        }
        
        protected abstract void InitScene();
        protected abstract void InitSceneComplete();
        
        public virtual void StepScene(TimeSpan s)
        {
            
        }

        protected sealed override void Step(TimeSpan step)
        {
            Elapsed += step;
            FrameCount++;
            
            StepScene(step);
            foreach (var element in ChildrenRecursive())
            {
                if (element == this) continue;

                element.Animator?.Step(step);
                element.StepExec(step);
            }
        }
        
        protected sealed override void Init()
        {
            InitScene();
            foreach (var element in ChildrenRecursive())
            {
                if (element == this) continue;
                if (element is ElementBase eb && eb.IsHidden) continue;
                
                element.InitExec();
            }
            InitSceneComplete();
        }
        
        protected override void Draw(DrawContext surface)
        {
            Scene.DebugHits.Clear();
            DrawBackGround(surface);

            var drawOrder = ChildrenRecursive(VisitOnDraw)
                            .Where(x => x != this && (x is ElementBase eb && !eb.IsHidden))
                            .OrderBy(x => x.Block?.Z ?? 0)
                            .ToImmutableArray();
            
            foreach (var element in drawOrder.Where(x=>(x.Block?.Z ?? 0) < 100))
            {
                element.DrawExec(surface);
            }
            
            foreach (var element in drawOrder)
            {
                element.DecorateExec(surface);
            }
            
            foreach (var element in drawOrder.Where(x=>(x.Block?.Z ?? 0) >= 100))
            {
                element.DrawExec(surface);
            }
            
            DrawOverlay(surface);
        }

        private bool VisitOnDraw(IElement e)
        {
            if (e is ElementBase eb)
            {
                if (!eb.IsEnabled || eb.IsHidden) return false;
            }
            return true;
        }

      





    }

}