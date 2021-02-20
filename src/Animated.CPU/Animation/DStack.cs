using System;
using System.Collections.Generic;
using System.Linq;

namespace Animated.CPU.Animation
{
    public class DStack
    {
        public DStack(DBlock container, DOrient orient, DBlockProps innerStyle)
        {
            Container  = container;
            Orient     = orient;
            InnerStyle = innerStyle;
        }

        public List<DBlock> Children   { get; } = new List<DBlock>();
        public DBlock       Container  { get; }
        public DOrient      Orient     { get; }
        public DBlockProps  InnerStyle { get; }

        public DStack Add(DBlock block)
        {
            throw new NotImplementedException();
        }
        
        public DStack Add(float size)
        {
            throw new NotImplementedException();
        }

        public void Divide<T>(IReadOnlyList<T> items)
        {
            if (Orient == DOrient.Vert)
            {
                var inner  = Container.Inner;
                var offset = 0f;
                var size   = inner.W / items.Count;
                
                for (int i = 0; i < items.Count; i++)
                {
                    var b = new DBlock(InnerStyle)
                    {
                        X = inner.X + offset,
                        Y = inner.Y,
                        W = size,
                        H = inner.H,

                        Domain =  items[i]
                    };
                    Children.Add(b);
                    offset += size;
                }
            }
            else
            {
                var inner  = Container.Inner;
                var offset = 0f;
                var size   = inner.H / items.Count;
                
                for (int i = 0; i < items.Count; i++)
                {
                    var b = new DBlock(InnerStyle)
                    {
                        X = inner.X ,
                        Y = inner.Y + offset,
                        W = inner.W,
                        H = size,

                        Domain =  items[i]
                    };
                    Children.Add(b);
                    offset += size;
                }
            }
        }

        public void Divide(int count) => Divide(Enumerable.Repeat(0, count).Select(x => (object)null).ToList());
    }
}