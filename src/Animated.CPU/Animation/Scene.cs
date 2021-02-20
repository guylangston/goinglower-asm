using System;
using System.Collections.Generic;
using SkiaSharp;

namespace Animated.CPU.Animation
{
    public interface IScene
    {
        int      Steps   { get; }
        TimeSpan Elapsed { get; }

        void Init(SKSurface surface);
        
        void Step(TimeSpan step);
        void Draw(SKSurface surface);

        void Add(IElement el);
        void Remove(IElement el);
    }

    public static class SceneHelper
    {
        public static void Add(this IScene scene, IEnumerable<IElement> items)
        {
            foreach (var item in items)
            {
                scene.Add(item);
            }
        }
    }
    
    

    public interface IElement
    {
        IScene Scene { get; }
        void Step(TimeSpan step);
        void Draw(SKSurface surface);
    }

    public abstract class SceneBase : IScene
    {
        public TimeSpan Elapsed { get; private set; }
        public int      Steps   { get; private set; }

        private readonly List<IElement> elements = new List<IElement>();

        public IReadOnlyList<IElement> Elements => elements;

        private bool init = false;
        public abstract void Init(SKSurface surface);
        
        public void Step(TimeSpan step)
        {
            Elapsed += step;
            Steps++;
            foreach (var element in elements)
            {
                element.Step(step);
            }
        }
        
        public void Draw(SKSurface surface)
        {
            if (!init)
            {
                Init(surface);
                init = true;
            }
            DrawBackGround(surface);
            foreach (var element in elements)
            {
                element.Draw(surface);
            }
            DrawOverlay(surface);
        }

        protected abstract void DrawOverlay(SKSurface surface);
        protected abstract void DrawBackGround(SKSurface surface);
        
        public void Add(IElement el)
        {
            elements.Add(el);
        }
        public void Remove(IElement el)
        {
            elements.Remove(el);
        }
    }

    public abstract class Element<T> : IElement where T:IScene
    {
        protected Element(T scene, DBlock b)
        {
            Scene = scene;
            Block = b;
        }

        public DBlock Block { get; set; }

        public T Scene { get; }

        IScene IElement.Scene => this.Scene;

        public abstract void Step(TimeSpan step);
        public abstract void Draw(SKSurface surface);
    }
    
    public class CountDown
    {
        public CountDown(float duration)
        {
            Remaining = Duration = duration;
        }

        public float Duration  { get; set; }
        public float Remaining { get; set; }

        public bool Step(TimeSpan s)
        {
            Remaining -= (float)s.TotalSeconds;
            if (Remaining <= 0)
            {
                Remaining = Duration;
                return true;
            }

            return false;
        }
    }
}