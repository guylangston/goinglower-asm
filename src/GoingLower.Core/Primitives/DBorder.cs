using System;

namespace GoingLower.Core.Primitives
{
    public readonly struct DBorder : IBorder
    {
        public DBorder(float all)
        {
            Top    = all;
            Bottom = all;
            Left   = all;
            Right  = all;
        }

        public DBorder(float top, float right, float bottom, float left)
        {
            Top    = top;
            Bottom = bottom;
            Left   = left;
            Right  = right;
        }


        public DBorder(DBorder copy) 
        {
            this.Top    = copy.Top;
            this.Bottom = copy.Bottom;
            this.Left   = copy.Left;
            this.Right  = copy.Right;
        }
        
        public float Top    { get; }
        public float Bottom { get; }
        public float Left   { get; }
        public float Right  { get; }

        public float All => Math.Max(Top, Math.Max(Bottom, Math.Max(Left, Right)));
    }
}