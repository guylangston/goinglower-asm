using System;
using System.Collections.Generic;
using Animated.CPU.Model;
using SkiaSharp;

namespace Animated.CPU.Animation
{
    public class DrawContext : Drawing // Use Base Class to add draw func
    {
        public IScene Scene { get; }

        public DrawContext(IScene scene, SKCanvas canvas) : base(canvas)
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
        IEnumerable<IElement> ChildrenRecursive(Func<IElement, bool> visit);
        T Add<T>(T e) where T:IElement;
        void Remove(IElement el);

        PointSelectionResult? GetSelectionAtPoint(SKPoint p);
    }


    public interface IHasMaster  // Where 'real' parent is obscured
    {
        IElement Master { get; }
    }
    
    // Animatable property
    public interface IAnimProp
    {
        public float Value     { get; set; }
        public float BaseValue { get; set; }
    }

    public class PropFloat : IAnimProp
    {
        public float Value     { get; set; }
        public float BaseValue { get; set; }
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
        string   Name       { get; }
        int      FrameCount { get; }
        TimeSpan Elapsed    { get; }
        
        IStyleFactory StyleFactory { get; }
        
        // Debugging
        SKPoint   DebugPointAt    { get; }
        List<IElement> DebugHits { get; }

        // Send commands/input/events
        void ProcessEvent(string name, object args, object platform);
        void KeyPress(string key, object platform);
        void MousePress(uint eventButton, double eventX, double eventY, object interop);
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