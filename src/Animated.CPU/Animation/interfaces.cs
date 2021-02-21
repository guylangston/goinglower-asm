using System;
using System.Collections.Generic;
using SkiaSharp;

namespace Animated.CPU.Animation
{
    public interface IElement
    {
        IScene    Scene  { get;  }
        IElement? Parent { get;  }
        object    Model  { get; }
        DBlock    Block  { get; set; }
        
        void Init(SKSurface surface);
        void Step(TimeSpan step);
        void Draw(SKSurface surface);
        
        IReadOnlyList<IElement>? Children { get; }
        IEnumerable<IElement> ChildrenRecursive();
        void Add(IElement el);
        void Remove(IElement el);
        
    }

    public interface IStyleFactory
    {
        SKPaint GetPaint(IElement e, string id);
        SKColor GetColor(IElement e, string id);
    }
    
    
    public interface IScene : IElement
    {
        int      Steps   { get; }
        TimeSpan Elapsed { get; }
        
        IStyleFactory StyleFactory { get; }
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