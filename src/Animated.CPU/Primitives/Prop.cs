using System;
using System.Collections.Generic;

namespace Animated.CPU.Primitives
{
    public class Prop<T>
    {
        private IEqualityComparer<T> eq;
        private T val;
        private T prev;
        private Action<T, T>? onchange;

        public Prop(IEqualityComparer<T> eq, T val, Action<T, T>? onchange)
        {
            this.eq  = eq ?? throw new ArgumentNullException(nameof(eq));
            this.val      = val;
            this.onchange = onchange;
        }

        public T Value
        {
            get => val;
            set => SetValue(value);
        }

        public T Prev => prev;

        public bool SetValue(T newVal)
        {
            if (!eq.Equals(val, newVal))
            {
                if (onchange != null)
                {
                    onchange(val, newVal);
                }
                prev = val;
                val  = newVal;
                return true;

            }
            return false;
        }
    }

    public static class PropHelper
    {
        public static readonly IEqualityComparer<ulong> DefaultCompareULong = new EqualityULong();
        
        public class EqualityULong : IEqualityComparer<ulong>
        {
            public bool Equals(ulong x, ulong y) => x == y;
            public int GetHashCode(ulong obj) => obj.GetHashCode();
        }
        
        public class EqualityUInt : IEqualityComparer<uint>
        {
            public bool Equals(uint x, uint y) => x == y;
            public int GetHashCode(uint obj) => obj.GetHashCode();
        }
    }

}