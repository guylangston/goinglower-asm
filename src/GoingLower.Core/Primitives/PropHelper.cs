using System.Collections.Generic;

namespace GoingLower.Core.Primitives
{
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