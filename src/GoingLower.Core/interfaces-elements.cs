using System;
using System.Collections.Generic;
using GoingLower.Core;
using GoingLower.Core.Drawing;
using GoingLower.Core.Primitives;
using SkiaSharp;

namespace GoingLower.Core
{
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
    
    public interface IScene : IElement
    {
        string   Name       { get; }
        int      FrameCount { get; }
        TimeSpan Elapsed    { get; }
        
        IStyleFactory StyleFactory { get; }
        
        // Debugging
        SKPoint        DebugPointAt { get; }
        List<IElement> DebugHits    { get; }

        // Send commands/input/events
        void ProcessEvent(string name, object args, object platform);
        void KeyPress(string key, object platform);
        void MousePress(uint eventButton, double eventX, double eventY, object interop);
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
        void HandleMotion(float x, float y, object args);

        // Animation 
        void Step(TimeSpan step);
        void Draw(SKSurface surface);
        
    }
    
    public interface IBorder
    {
        float Top    { get; }
        float Bottom { get; }
        float Left   { get; }
        float Right  { get; }
    }
    
    public interface IHasMaster  // Where 'real' parent is obscured buy other 'formatting' elements like stack
    {
        IElement Master { get; }
    }
    
    public interface IStyleFactory
    {
        SKPaint GetPaint(IElement e, string id);
        SKColor GetColor(IElement e, string id);
    }
    
    public class PointSelectionResult
    {
        public IElement Element   { get; set; }
        public object?  Model     { get; set; }
        public object?  Selection { get; set; }
    }
    
    public class SceneSeqArg  // Hook to allow overload
    {
        public SceneSeqArg(SceneSeq seq)
        {
            Seq = seq;
        }

        public SceneSeq Seq { get;  }
    }

}