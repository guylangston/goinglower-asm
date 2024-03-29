using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using GoingLower.Core.Animation;
using GoingLower.Core.Drawing;
using GoingLower.Core.Helpers;
using GoingLower.Core.Primitives;
using SkiaSharp;

namespace GoingLower.Core
{
    public sealed class NeverUse {}
    
    public abstract class ElementBase : IElement
    {
        private IList<IElement> elements = ImmutableList<IElement>.Empty;
        private static  int nextId = 0;
        private int id;
        private object? model;

        protected ElementBase() // Only for IScene
        {
            if (this is not IScene) throw new Exception($"Expected: IScene but got {this.GetType().Name}");
            
            id    = nextId++;
        }

        protected ElementBase(IElement parent, DBlock? b)
        {
            if (parent is null) throw new Exception("Must have parent");
            if (this is IScene) throw new Exception("Use IScene constructor");
            
            Parent = parent;
            Scene  = parent is IScene ss ?  ss : parent.Scene ?? throw new Exception("Parent must have a valid scene");
            Block  = b;
            id     = nextId++;
        }

        protected Action<object?>? OnModelChanged { get; set; }
        public virtual object? Model
        {
            get => model;
            set
            {
                if (!object.Equals(model, value))
                {
                    model = value;
                    if (OnModelChanged != null)
                    {
                        OnModelChanged(value);    
                    }
                }
            }
        }

        

        public virtual IScene    Scene        { get; private set; }
        public         IElement? Parent       { get; private set; }
        public         DBlock?   Block        { get; set; }
        public         IAnimator Animator     { get; set; } = AnimatorEmpty.Instance;
        public         bool      IsHidden     { get; set; }
        public         bool      IsEnabled    { get; set; } = true;      // Step And Draw are not called
        public         bool      InitComplete { get; private set; }


        public T ParentAs<T>() => (T)Parent;
        public bool HasChildren => elements.Count > 0;

        public List<IElement> PathToRoot()
        {
            var a = new List<IElement>();
            IElement? x = this;
            while (x != null)
            {
                a.Add(x);
                x = x.Parent;
            }
            a.Reverse();
            return a;
        }

        public virtual PointSelectionResult? GetSelectionAtPoint(SKPoint p)
        {
            if (Block != null && Block.Contains(p))
            {
                return new PointSelectionResult()
                {
                    Element = this,
                    Model   = Model
                };
            }
            return null;
        } 


        public string DebugId => StringHelper.Join(PathToRoot(), x => x.ToString(), ">");

        
        public void InitExec()
        {
            if (InitComplete) return;
            try
            {
                Init();
                InitComplete = true;
            }
            catch (Exception e)
            {
                throw new Exception($"Init Failed: {DebugId}", e);
            }
            
        }

        public void StepExec(TimeSpan step)
        {
            if (!InitComplete) InitExec();
            
            // Step() should run when Disable and/or Hidden as Step() has the logic to turn these flags on/off
            Step(step); 
        }

        public void DrawExec(DrawContext surface)
        {
            if (!Scene.DebugPointAt.IsEmpty)
            {
                if (Block != null && Block.Contains(Scene.DebugPointAt))
                {
                    Scene.DebugHits.Add(this);
                }
            }
            if (IsEnabled && !IsHidden)
            {
                try
                {
                    Draw(surface);
                }
                catch (Exception e)
                {
                    throw new Exception($"Draw Failed: {DebugId}", e);
                }
            }
        }

        public void DecorateExec(DrawContext surface)
        {
            if (IsEnabled && !IsHidden)
            {
                try
                {
                    Decorate(surface);
                }
                catch (Exception e)
                {
                    throw new Exception($"Draw Failed: {DebugId}", e);
                }
            }
        }

        protected virtual void Init() {}
        protected abstract void Step(TimeSpan step);
        protected abstract void Draw(DrawContext surface);
        protected virtual void Decorate(DrawContext surface) {  /* Nothing by default */ }
        
        public int                     ChildrenCount => elements?.Count ?? 0;
        public IReadOnlyList<IElement> Children      => (IReadOnlyList<IElement>)elements;
        
        public IEnumerable<IElement> ChildrenRecursive()
        {
            yield return this;
            foreach (var e in elements)
            {
                foreach (var ee in e.ChildrenRecursive())
                {
                    yield return ee;
                }
            }
        }
        
        public IEnumerable<IElement> ChildrenRecursive(Func<IElement, bool> visit)
        {
            if (visit(this))
            {
                yield return this;
                foreach (var e in elements.Where(visit))
                {
                    foreach (var ee in e.ChildrenRecursive(visit))
                    {
                        yield return ee;
                    }
                }    
            }
        }
        
        public IEnumerable<T> ChildrenAre<T>() => this.Children.Where(x => x is T).Cast<T>();
        public IEnumerable<T> ChildrenRecursiveAre<T>() => this.ChildrenRecursive().Where(x => x is T).Cast<T>();

        public T GetElementByIndex<T>(int index) where T:IElement
            => (T)Children[index];

        public T GetElementByTypeImmediate<T>() where T:IElement
            => (T)Children.First(x => x is T);
        
        public IElement GetElementByModelRecurse<TModel>(Func<TModel, bool> where) 
            => ChildrenRecursive().First(x => x.Model is TModel tm && where(tm));
        
        public IElement GetElementByModelRecurse<TModel>(TModel match) 
            => ChildrenRecursive().First(x => x.Model is TModel tm && object.ReferenceEquals(tm, match));
        
        public virtual bool TryGetElementFromModelRecurse<T>(T findThis, out IElement found)
        {
            foreach (var element in ChildrenRecursive())
            {
                if (object.ReferenceEquals(element.Model, findThis))
                {
                    found = element;
                    return true;
                }
            }

            found = null;
            return false;
        }
        
        public virtual IElement GetElementFromModelRecurseSafe<T>(T findThis)
        {
            if (TryGetElementFromModelRecurse(findThis, out var e))
            {
                return e;
            }

            throw new Exception($"Expected Model Not Found: {typeof(T)} / {findThis}");
        }

        public void ClearChildren()
        {
            if (elements is List<IElement> ee)
            {
                ee.Clear();    
            }
            
        }
            
            

        public virtual T Add<T>(T e) where T:IElement
        {
            if (e.Scene == null) throw new Exception();
            if (e.Parent != this) throw new Exception($"Bad parent. Should be {this}, but was {e.Parent}");
            if (elements is ImmutableList<IElement>)
            {
                elements = new List<IElement>();
            }
            elements.Add(e);
            return e;
        }

        public virtual void Remove(IElement el)
        {
            elements.Remove(el);
        }

        protected void SetScene(IScene neverUse)
        {
            Scene = neverUse;
        }

        public int IndexInParent
        {
            get
            {
                var list = Parent?.Children;
                if (list == null) return -1;
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] == this) return i;
                }
                return -1;
            }
        }

        public override string ToString() => $"#{id}:^{IndexInParent}:{GetType().Name}:{Model}";


        protected SKPaint GetStyle(string style) => Scene.StyleFactory.GetPaint(this, style);
    }

   
}