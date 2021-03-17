using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SkiaSharp;

namespace Animated.CPU.Animation
{
    public abstract class SceneBase<TModel, TStyle> : SceneBase where TStyle:IStyleFactory
    {
        protected SceneBase(string name, TStyle styleFactory, DBlock block) : base(name, styleFactory, block)  
        {
            SetScene(this);
        }

        public TStyle Styles => (TStyle)StyleFactory;
        
        public new TModel Model
        {
            get => (TModel)base.Model;
            set => base.Model = value;
        }
    }
    

    public abstract class SceneBase : ElementBase, IScene 
    {
        protected SceneBase(string name, IStyleFactory styleFactory, DBlock b)
        {
            base.Block   = b ?? throw new ArgumentNullException(nameof(b));
            this.Name    = name ?? throw new ArgumentNullException(nameof(name));
            StyleFactory = styleFactory ?? throw new ArgumentNullException(nameof(styleFactory));
        }

        public override IScene Scene => this;

        public TimeSpan      Elapsed      { get; protected set; }
        public IStyleFactory StyleFactory { get; protected set; }

        public new DBlock Block
        {
            get
            {
                return base.Block ?? throw new Exception();
            }
            set
            {
                throw new Exception();
            }
        }

        public string Name       { get; }
        public int    FrameCount { get; protected set; }
        public float  FPS        => (float)FrameCount / (float)Elapsed.TotalSeconds ;
        
        // Debugging
        public SKPoint        DebugPointAt { get; set; }
        public uint           DebugButton  { get; set; }
        public List<IElement> DebugHits    { get; } = new List<IElement>();

        
        protected abstract void DrawOverlay(DrawContext drawing);
        protected abstract void DrawBackGround(DrawContext drawing);
        
        protected abstract void InitScene();
        protected abstract void InitSceneComplete();


        public abstract void ProcessEvent(string name, object args, object platform);
        public abstract void KeyPress(string key, object platformKeyObject);
        public abstract void MousePress(uint eventButton, double eventX, double eventY, object interop);
        
        // Send external command
        public Action<string, object>? SendHostCommand { get; set; }
        
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