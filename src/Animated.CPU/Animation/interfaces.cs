using System;
using System.Collections.Generic;
using Animated.CPU.Model;
using SkiaSharp;

namespace Animated.CPU.Animation
{
    public class DrawContext : Drawing // Use Base Class to add draw func
    {
        public Scene Scene { get; }

        public DrawContext(Scene scene, SKCanvas canvas) : base(canvas)
        {
            Scene = scene;
        }

        public void DrawHighlight(IElement e)
        {
            DrawHighlight(e.Block.BorderRect.ToSkRect(), e.Scene.StyleFactory.GetPaint(e, "Highlighted"), e.Block.Border.All);
        }
    }
    
    public interface IElement
    {
        IScene     Scene    { get;  }
        IElement?  Parent   { get;  }
        object     Model    { get; }
        DBlock?    Block    { get; set; }
        IAnimator? Animator { get; set; }
        
        void InitExec();
        void StepExec(TimeSpan step);
        void DrawExec(DrawContext surface);
        void DecorateExec(DrawContext surface);
        
        IReadOnlyList<IElement>? Children { get; }
        IEnumerable<IElement> ChildrenRecursive();
        T Add<T>(T e) where T:IElement;
        void Remove(IElement el);
    }


    public interface IHasMaster<out TParent> : IElement where TParent: IElement
    {
        TParent Master
        {
            get
            {
                var p = Parent;
                while (p != null)
                {
                    if (p is TParent master) return master;
                    p = p.Parent;
                }
                throw new Exception($"Master Not Found: {typeof(TParent)} in {this}");
            }
        }
    }

    public interface IAnimation
    {
        bool IsActive { get;  }
        void Start();
        bool Step(TimeSpan t);
        void Stop();
        
    }

    public interface IAnimator : IAnimation // Does not draw, just
    {
        void Add(IAnimation a);
        IEnumerable<IAnimation> Animations { get; }
    }

    public interface IStyleFactory
    {
        SKPaint GetPaint(IElement e, string id);
        SKColor GetColor(IElement e, string id);
    }
    
    
    public interface IScene : IElement
    {
        int      FrameCount   { get; }
        TimeSpan Elapsed { get; }
        
        IStyleFactory StyleFactory { get; }
        

        void KeyPress(object platformKeyObject, string key);
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
    
}