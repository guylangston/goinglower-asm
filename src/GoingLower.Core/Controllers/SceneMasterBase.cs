using System;
using System.Collections.Generic;
using GoingLower.Core.Drawing;
using GoingLower.Core.Helpers;
using SkiaSharp;

namespace GoingLower.Core.Controllers
{
  
    public abstract class SceneMasterBase : ISceneMaster
    {
        protected Dictionary<string, IScene> scenes = new Dictionary<string, IScene>();
        private readonly IReadOnlyList<string> all;
        private IScene? current;

        protected SceneMasterBase(IReadOnlyList<string> all)
        {
            this.all = all;
        }

        public IScene? CurrentScene
        {
            get => current;
            set => current = value;
        }
        
        
        public void MoveToScene(SceneSeqArg seq)
        {
            CurrentScene = NextScene(seq);
        }

        public int CurrentIndex
        {
            get
            {
                if (current == null) return -1;
                
                return all.IndexOfElse(current.Name, -2);
            }
        }

        public IReadOnlyList<string> GetAllScenes() => all;

        public abstract void Init(SKRect viewSize);
        public abstract IScene SceneFactory(string name);
        public abstract IScene? NextScene(SceneSeqArg seq);
        public abstract void HandleCommand(string cmd, object obj);

        public virtual IScene GetScene(string name)
        {
            name = name.ToLowerInvariant();
            if (scenes.TryGetValue(name, out var exists))
            {
                return exists;
            }

            var newScene  = SceneFactory(name);
            if (newScene != null)
            {
                scenes[name] = newScene;    
            }
            return newScene;
        }


        public virtual void HandleKeyPress(string key, object native)
        {
            current?.KeyPress(key, native);
        }

        public virtual void HandleMousePress(uint button, float x, float y, object native)
        {
            current?.MousePress(button, x, y, native);
        }

        public virtual void HandleMotion(float x, float y, object args)
        {
            // Nothing by default
        }

        public virtual void Step(TimeSpan step)
        {
            current?.StepExec(step);
        }

        public virtual void Draw(SKSurface surface)
        {
            var ctx = new DrawContext(current, surface.Canvas);
            current?.DrawExec(ctx);
        }
    }
}