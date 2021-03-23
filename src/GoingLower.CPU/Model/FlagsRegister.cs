using System.Collections.Generic;
using System.Collections.Immutable;

namespace GoingLower.CPU.Model
{
    public class FlagsRegister : Register
    {
        public FlagsRegister() : base("FLAGS", "State Flags")
        {
            IdAlt = ImmutableArray.Create<string>("RFLAGS");
        }

        //https://en.wikipedia.org/wiki/FLAGS_register
        public bool CarryFlag     => (Value & 0x0001ul) > 0;
        public bool ParityFlag    => (Value & 0x0004ul) > 0;
        public bool AdjustFlag    => (Value & 0x0010ul) > 0;
        public bool ZeroFlag      => (Value & 0x0040ul) > 0;
        public bool SignFlag      => (Value & 0x0080ul) > 0;
        public bool TrapFlag      => (Value & 0x0100ul) > 0;
        public bool InterruptFlag => (Value & 0x0200ul) > 0;
        public bool DirectionFlag => (Value & 0x0400ul) > 0;
        public bool OverflowFlag  => (Value & 0x0800ul) > 0;

        public IEnumerable<(string name, bool val)> GetFlags()
        {
            // Organised my most interesting first
            yield return ("(CF)Carry", CarryFlag);
            yield return (" __ ", false);
            yield return ("(PF)Parity", ParityFlag);
            yield return (" __ ", false);
            yield return ("(AF)Adjust", AdjustFlag);
            yield return (" __ ", false);
            yield return ("(ZF)Zero", ZeroFlag);
            yield return ("(SF)Sign", SignFlag);
            yield return ("(TF)Trap", TrapFlag);
            yield return ("(IF)Interrupt", InterruptFlag);
            yield return ("(DF)Direction", DirectionFlag);
            yield return ("(OF)Overflow", OverflowFlag);
        }
        


    }
}