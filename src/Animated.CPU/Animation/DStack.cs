using System;
using System.Collections.Generic;
using System.Linq;

namespace Animated.CPU.Animation
{
    
    
    
    public class DStack
    {
        public DStack(DBlock container, DOrient orient)
        {
            Container  = container;
            Orient     = orient;
        }

        public DBlock       Container  { get; }
        public DOrient      Orient     { get; }

        private Rect inner;
        private float offset;
        private float size;

        public IEnumerable<(T model, DBlock block, int index)> Divide<T>(IReadOnlyList<T> items)
        {
            inner  = Container.Inner;
            offset = 0f;
            
            if (Orient == DOrient.Vert)
            {
                size = inner.W / items.Count;    
                
                for (int i = 0; i < items.Count; i++)
                {
                    var b = new DBlock()
                    {
                        X = inner.X + offset,
                        Y = inner.Y,
                        W = size,
                        H = inner.H
                    };

                    yield return (items[i], b, i);
                    
                    offset += size;
                }
            }
            else
            {
                
                var size   = inner.H / items.Count;
                
                for (int i = 0; i < items.Count; i++)
                {
                    var b = new DBlock()
                    {
                        X = inner.X ,
                        Y = inner.Y + offset,
                        W = inner.W,
                        H = size
                    };
                    yield return (items[i], b, i);
                    offset += size;
                }
            }
        }

        
    }
}