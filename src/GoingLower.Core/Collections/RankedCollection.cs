using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Atk;

namespace GoingLower.Core.Collections
{
    public class RankedCollection<T>
    {
        static readonly IReadOnlyCollection<T> empty = ImmutableArray<T>.Empty;
        readonly List<List<T>> inner = new List<List<T>>();
        readonly Func<T, int> getRank;

        public RankedCollection(Func<T, int> getRank)
        {
            this.getRank = getRank;
        }

        public IReadOnlyCollection<T> this[int rank]
        {
            get
            {
                if (inner.Count < rank) return empty;
                return (IReadOnlyCollection<T>)inner[rank] ?? empty;
            }
        }

        public int MaxRank => inner.Count;

        public void Add(T item)
        {
            var r = getRank(item);
            while(inner.Count <= r) inner.Add(new List<T>());

            inner[r].Add(item);
        }
    }

    public static class BiMap
    {
        public static BiMap<TA, TB> Generate<T, TA, TB>(IEnumerable<T> items, Func<T, TA> getA, Func<T, TB> getB)
        {
            var res = new BiMap<TA, TB>();
            foreach (var item in items)
            {
                res.Add(getA(item), getB(item));
            }

            return res;
        }
        
    }

    public class BiMap<TA, TB>
    {
        private Dictionary<TA, TB> ab = new Dictionary<TA, TB>();
        private Dictionary<TB, TA> ba = new Dictionary<TB, TA>();


        public IReadOnlyCollection<TA> AllA => ab.Keys;
        public IReadOnlyCollection<TB> AllB => ba.Keys;
        
        
        public void Add(TA a, TB b)
        {
            ab[a] = b;
            ba[b] = a;
        }

        public TA this[TB b] => ba[b];
        public TB this[TA a] => ab[a];


        public TA GetA(TB b) => ba[b];
        public TB GetB(TA a) => ab[a];
    }
}
