using System;
using System.Collections.Generic;
using System.Linq;
using GoingLower.Core.Drawing;
using GoingLower.Core.Primitives;
using SkiaSharp;

namespace GoingLower.Core.Elements
{
    public abstract class ContainerElement : ElementBase  // Strong Types are not needed here, as we only act on elements
    {
        protected ContainerElement(IElement parent, DBlock b) : base(parent, b)
        {
        }
    }

    public enum StackMode
    {
        JustLayout,
        OverrideSize,
        Stack
    }
    
    public class StackElement : ContainerElement  
    {
        public StackElement(IElement parent, DBlock b, DOrient orient, StackMode mode = StackMode.JustLayout) : base(parent, b)
        {
            Orient    = orient;
            Mode = mode;
        }

        public bool      SkipHidden { get; init; } = true;
        public DOrient   Orient     { get; init; }
        public StackMode Mode       { get; }
        public float     Gap        { get; set; }

        public void Layout()
        {
            var inner = Block.Inner;
            if (Orient == DOrient.Vert)
            {
                var off = inner.Y;
                foreach (var child in Children)
                {
                    if (SkipHidden && child is ElementBase eb && eb.IsHidden) continue;
                    
                    if (child.Block == null)
                    {
                        child.Block = new DBlock()
                        {
                            H = inner.H / Children.Count
                        };
                    }
                    
                    child.Block.Y =  off;
                    child.Block.X =  inner.X;
                    child.Block.W =  inner.W;
                    off           += child.Block.H;
                }    
            }
            else
            {
                var off = inner.X;
                foreach (var child in Children)
                {
                    if (SkipHidden &&child is ElementBase eb && eb.IsHidden) continue;
                    
                    if (child.Block == null)
                    {
                        child.Block = new DBlock()
                        {
                            W = inner.W / Children.Count
                        };
                    }
                    
                    child.Block.Y =  inner.Y;
                    child.Block.X =  off;
                    child.Block.H =  inner.H;
                    off           += child.Block.W;
                }    
            }
        }

        public override T Add<T>(T e)
        {
            var x =  base.Add(e);
            Layout();   // Layout each add, so that Init calls can set positions 
            return x;
        }
        
        public T Add<T>(T e, float size) where T: IElement
        {
            if (e.Block == null) e.Block = new DBlock();
            if (Orient == DOrient.Vert)
            {
                e.Block.H = size;
            }
            else
            {
                e.Block.W = size;
            }
            
            var x=  base.Add(e);
            Layout();   // Layout each add, so that Init calls can set positions 
            return x;
        }

        protected override void Step(TimeSpan step)
        {
            if (Mode == StackMode.OverrideSize)
            {
                IElement[] items = SkipHidden
                    ? Children.Where(x => x is ElementBase eb && !eb.IsHidden).ToArray()
                    : Children.ToArray();

                if (Orient == DOrient.Horz)
                {
                    var size = (Block.W - (items.Length * Gap)) / items.Length;
                    foreach (var item in items)
                    {
                        item.Block   ??= new DBlock();
                        item.Block.W =   size;
                    }
                }
                else
                {
                    var size = (Block.H - (items.Length * Gap)) / items.Length;
                    foreach (var item in items)
                    {
                        item.Block   ??= new DBlock();
                        item.Block.H =   size;
                    }
                }
            }
            Layout();
        }

        protected override void Draw(DrawContext surface)
        {
            Layout();
        }
    }
}