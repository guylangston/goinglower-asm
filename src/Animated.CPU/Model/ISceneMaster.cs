using System;
using System.Collections.Generic;
using Animated.CPU.Animation;
using SkiaSharp;

namespace Animated.CPU.Model
{
    public enum SceneSeq
    {
        Beginning,
        Prior,
        Next,
        End
    }

    public class SceneSeqArg  // Hook to allow overload
    {
        public SceneSeqArg(SceneSeq seq)
        {
            Seq = seq;
        }

        public SceneSeq Seq { get;  }
    }
    
    public interface ISceneMaster
    {
        IScene? CurrentScene { get; set; }
        
        // Control
        void Init(SKRect viewSize);
        void MoveToScene(SceneSeqArg seq);
        IReadOnlyList<string> GetAllScenes();

        // Input
        void HandleKeyPress(string key, object native);
        void HandleMousePress(uint button, float x, float y, object native);

        // Animation 
        void Step(TimeSpan step);
        void Draw(SKSurface surface);
    }

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

        public int CurrentIndex => current == null ? -1 : all.IndexOf(current.Name);
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