using System;
using System.Collections.Generic;
using SkiaSharp;

namespace Animated.CPU.Animation
{
    public abstract class ContainerElement : ElementBase  // Strong Types are not needed here, as we only act on elements
    {
    

        protected ContainerElement(IScene scene, IElement? parent) : base(scene, parent)
        {
        }

        protected ContainerElement(IScene scene, IElement? parent, DBlock b) : base(scene, parent, b)
        {
        }
    }
    
    public class StackElement : ContainerElement  
    {
        public StackElement(IScene scene, IElement? parent, DOrient orient) : base(scene, parent)
        {
            Orient = orient;
        }

        public StackElement(IScene scene, IElement? parent, DBlock b, DOrient orient) : base(scene, parent, b)
        {
            Orient = orient;
        }

        public DOrient Orient { get; set; }

        public void Layout()
        {
            var inner = Block.Inner;
            if (Orient == DOrient.Vert)
            {
                var off = inner.Y;
                foreach (var child in Children)
                {
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

        public override void Step(TimeSpan step)
        {   
            Layout();
        }

        public override void Draw(SKSurface surface)
        {
            
        }
    }
}