using System;
using System.Collections.Generic;

namespace GoingLower.Core.Primitives
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

        public T    Prev      => prev;
        public bool IsChanged { get; set; }

        public bool SetValue(T newVal)
        {
            if (!eq.Equals(val, newVal))
            {
                if (onchange != null)
                {
                    onchange(val, newVal);
                }
                prev      = val;
                val       = newVal;
                IsChanged = true;
                return true;

            }
            return false;
        }
    }

    public class PropULong : Prop<ulong>
    {

        public PropULong(ulong val, Action<ulong, ulong>? onchange = null) : base(PropHelper.DefaultCompareULong, val, onchange)
        {
        }
    }
}