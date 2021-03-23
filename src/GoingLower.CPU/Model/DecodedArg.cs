using System.Collections.Generic;
using GoingLower.Core.Helpers;

namespace GoingLower.CPU.Model
{
    public class DecodedArg  
    {
        public int    Index { get; set; }
        public string Value { get; set; }
        
        public bool      IsImplied   { get; set; }
        public Register? Register    { get; set; }
        public InOut     InOut       { get; set; }
        public bool      IsImmediate => Value != null && Value.StartsWith("0x");
        public bool      IsPointer   => Value != null && Value.StartsWith("[");
        public string?   Description { get; set; }

        public override string ToString() => Value;
        
        public void ParseArg(Cpu cpu, DecodedInstruction decodedInstruction, string line)
        {
            Register = cpu.GetReg(Value);
        }

        public static IEqualityComparer<DecodedArg> Compare = new CompareByValue();
        
        

        class CompareByValue : IEqualityComparer<DecodedArg>
        {
            public bool Equals(DecodedArg x, DecodedArg y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Value == y.Value;
            }

            public int GetHashCode(DecodedArg obj)
            {
                return obj.Value.GetHashCode();
            }
        }

        public bool TryGetValueAsPointer(out ulong addr) => ParseHelper.TryParseHexWord(Value, out addr);
    
    }
}