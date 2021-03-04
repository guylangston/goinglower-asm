using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SkiaSharp;

namespace Animated.CPU.Animation
{
    public class NeverUse
    {
    }

    public abstract class ElementBase : IElement
    {
        private IList<IElement> elements = ImmutableList<IElement>.Empty;

        protected ElementBase()
        {
            Scene = null;
        }

        protected ElementBase(IScene scene, IElement? parent)
        {
            Scene  = scene ?? throw new ArgumentNullException(nameof(scene));
            Parent = parent;
        }

        protected ElementBase(IScene scene, IElement? parent, DBlock b)
        {
            Scene  = scene ?? throw new ArgumentNullException(nameof(scene));
            Parent = parent;
            Block  = b;
        }

        public IScene    Scene     { get; private set; }
        public IElement? Parent    { get; private set; }
        public object?   Model     { get; set; }
        public DBlock    Block     { get; set; }
        public IAnimator Animator  { get; set; } = EmptyAnimator.Instance;
        public bool      IsHidden  { get; set; }
        public bool      IsEnabled { get; set; } = true;      // Step And Draw are not called


        public T ParentAs<T>() => (T)Parent;

        private bool fistStep = true;
        public virtual void Init() {}

        public void StepExec(TimeSpan step)
        {
            if (fistStep)
            {
                fistStep = false;
                Init();
            }
            // Step() should run when Disable and/or Hidden as Step() has the logic to turn these flags on/off
            Step(step); 
        }

        public void DrawExec(DrawContext surface)
        {
            if (IsEnabled && !IsHidden)
            {
                Draw(surface);
            }
        }

        public void DecorateExec(DrawContext surface)
        {
            if (IsEnabled && !IsHidden) Decorate(surface);
        }
        
        protected abstract void Step(TimeSpan step);
        protected abstract void Draw(DrawContext surface);
        protected virtual void Decorate(DrawContext surface) {  /* Nothing by default */ }

        public IReadOnlyList<IElement> Children => (IReadOnlyList<IElement>)elements;

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

        public T GetChild<T>(int index) where T:IElement
            => (T)Children[index];

        public T GetChildByElementType<T>() where T:IElement
            => (T)Children.First(x => x is T);
        
        public IElement RecurseByModel<TModel>(Func<TModel, bool> where) 
            => ChildrenRecursive().First(x => x.Model is TModel tm && where(tm));
        
        public IElement RecurseByModel<TModel>(TModel match) 
            => ChildrenRecursive().First(x => x.Model is TModel tm && object.ReferenceEquals(tm, match));
        
        public virtual bool TryRecurseElementFromModel<T>(T findThis, out IElement found)
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
        
        public virtual IElement RecurseElementFromModelSafe<T>(T findThis)
        {
            if (TryRecurseElementFromModel(findThis, out var e))
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
    }

   
}