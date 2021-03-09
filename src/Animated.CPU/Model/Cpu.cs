using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using Animated.CPU.Primitives;

namespace Animated.CPU.Model
{
    // https://software.intel.com/content/www/us/en/develop/articles/introduction-to-x64-assembly.html
    public class Cpu
    {
        public Cpu()
        {
            RegisterFile = new List<Register>()
            {
                RIP,
                
                R0,
                R1,
                R2,
                R3,
                R4,
                R5,
                R6,
                R7,
                R8,
                R9,
                R10,
                R11,
                R12,
                R13,
                R14,
                R15,
                RFLAGS,
            };
            Main = new Memory((int)Math.Pow(2, 16));
            ALU  = new ArithmeticLogicUnit(this);
        }

        // https://en.wikibooks.org/wiki/X86_Assembly/X86_Architecture
        public Register RAX { get; } = new Register("A", "Accumulator")
        {
            Description = "Used in arithmetic operations",
            IdAlt = ImmutableArray.Create<string>("RAX", "EAX", "AX", "AH", "AL")
        };

        public Register RBX { get; } = new Register("B", "Base")
        {
            IdAlt = ImmutableArray.Create<string>("RBX", "EBX", "BX")
        };
        public Register RCX { get; } = new Register("C", "Count")
        {
            IdAlt = ImmutableArray.Create<string>("RCX", "ECX", "CX")
        };
        public Register RDX { get; } = new Register("D", "Data")
        {
            IdAlt = ImmutableArray.Create<string>("RDX", "EDX", "DX")
        };
        public Register RBP { get; } = new Register("BP", "Stack Base Pointer")
        {
            IdAlt = ImmutableArray.Create<string>("RBP", "RDX", "EDX", "DX")
        };
        public Register RSI { get; } = new Register("SI", "Source Index")
        {
            IdAlt = ImmutableArray.Create<string>("RSI", "ESI")
        };
        public Register RDI { get; } = new Register("DI", "Destination Index")
        {
            IdAlt = ImmutableArray.Create<string>("RDI", "EDI")
        };
        public Register RSP { get; } = new Register("SP", "Stack Pointer")
        {
            IdAlt = ImmutableArray.Create<string>("RSP", "ESP")
        };

        public Register R0 => RAX;
        public Register R1 => RBX;
        public Register R2 => RCX;
        public Register R3 => RDX;
        public Register R4 => RBP;
        public Register R5 => RSI;
        public Register R6 => RDI;
        public Register R7 => RSP;

        public Register R8  { get; } = new Register("R8", "Register 8")   { IsExtendedReg = true };
        public Register R9  { get; } = new Register("R9", "Register 9")   { IsExtendedReg = true };
        public Register R10 { get; } = new Register("R10", "Register 10") { IsExtendedReg = true };
        public Register R11 { get; } = new Register("R11", "Register 11") { IsExtendedReg = true };
        public Register R12 { get; } = new Register("R12", "Register 12") { IsExtendedReg = true };
        public Register R13 { get; } = new Register("R13", "Register 13") { IsExtendedReg = true };
        public Register R14 { get; } = new Register("R14", "Register 14") { IsExtendedReg = true };
        public Register R15 { get; } = new Register("R15", "Register 15") { IsExtendedReg = true };

        public Register    RIP    { get; } = new Register("IP", "Instruction Pointer")
        {
            IdAlt = ImmutableArray.Create<string>("RIP")
        };
        public FlagsRegister RFLAGS { get; } = new FlagsRegister();
        public Prop<ulong> CLK    { get; } = new PropULong(0);

        public List<Register> RegisterFile { get; }

        public IMemory L1   { get; set; }
        public IMemory L2   { get; set; }
        public IMemory L3   { get; set; }
        public IMemory Main { get; set; }

        public MemoryView Instructions { get; set; }
        public MemoryView Stack        { get; set; }

        public ArithmeticLogicUnit ALU { get; set; }

        // Animation & Story Replay
        public Story Story { get; set; }

        public void Step()
        {
            CLK.Value++;
        }

        public Register? SetReg(string name, ulong value)
        {
            foreach (var register in RegisterFile)
            {
                if (register.Match(name))
                {
                    if (register.SetValue(value))
                    {
                        return register;
                    }

                    return null;
                }
            }
            return null;
        }

        public Register? GetReg(string name)
        {
            foreach (var reg in RegisterFile)
            {
                if (reg.Match(name))
                {
                    return reg;
                }
            }
            return null;
        }
        
       
    }

    [Flags]
    public enum InOut 
    {
        In = 1, 
        Out = 2,
        InOut = In + Out,
        InComplete = 4,
    }


    public class RegisterMode
    {
        public string Id      { get; set; }
        public int    Size    { get; set; }
        public ulong  BitMask { get; set; }
    }

   
    public class Register : Prop<ulong>
    {
        public Register(string id, string name) : base(PropHelper.DefaultCompareULong, 0, null)
        {
            Id   = id;
            Name = name;
        }

        public string                Id            { get; set; }
        public string                Name          { get; set; }
        public string                Description   { get; set; }
        public string?               LastUsedAs    { get; set; }
        public bool                  IsExtendedReg { get; set; }
        
        // TODO: Refactor to use RegisterMode
        public IReadOnlyList<string> IdAlt         { get; set; }
        
        public string ValueHex       => Value.ToString("X").PadLeft(64 / 8 * 2, '0');

        public int LastUsedAsSize
        {
            get
            {
                if (LastUsedAs == null) return 64;
                if (LastUsedAs.StartsWith("e")) return 32;

                return 64;

            }
        }

        public IEnumerable<string> AllIds()
        {
            yield return Id;
            foreach (var ii in IdAlt)
            {
                yield return ii;
            }
        }
        
        

        public override string ToString() => $"{Id}/{Name} = {Value:X}";

        public bool Match(string rId)
        {
            if (string.Equals(Id, rId, StringComparison.InvariantCultureIgnoreCase)) return true;

            if (IdAlt != null)
            {
                foreach (var a in IdAlt)
                {
                    if (string.Equals(a, rId, StringComparison.InvariantCultureIgnoreCase)) return true;    
                }    
            }
            

            return false;
        }

        public RegisterDelta ToDelta()
        {
            return new RegisterDelta()
            {
                Register    = Id,
                ValueParsed = Value,
                ValueRaw    = ValueHex
            };
        }
    }

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
            yield return ("Carry", CarryFlag);
            yield return ("Parity", ParityFlag);
            yield return ("Adjust", AdjustFlag);
            yield return ("Zero", ZeroFlag);
            yield return ("Sign", SignFlag);
            yield return ("Trap", TrapFlag);
            yield return ("Interrupt", InterruptFlag);
            yield return ("Direction", DirectionFlag);
            yield return ("Overflow", OverflowFlag);
        }
        


    }

    public interface IMemory
    {
        byte GetByte(ulong ptr);// 8
        ushort GetWord(ulong ptr);// 16
        uint GetDWord(ulong ptr);// 32
        ulong GetQWord(ulong ptr);// 64

        void SetByte(ulong ptr, byte val);// 8
        void SetWord(ulong ptr, ushort val);// 16
        void SetDWord(ulong ptr, uint val);// 32
        void SetQWord(ulong ptr, ulong val);// 64
    }

    public class Memory : IMemory
    {
        private byte[] block;

        public Memory(int size)
        {
            block = new byte[size];
        }

        public byte GetByte(ulong ptr) => block[ptr];

        public ushort GetWord(ulong ptr)
        {
            throw new System.NotImplementedException();
        }

        public uint GetDWord(ulong ptr)
        {
            throw new System.NotImplementedException();
        }

        public ulong GetQWord(ulong ptr)
        {
            throw new System.NotImplementedException();
        }

        public void SetByte(ulong ptr, byte val)
        {
            block[ptr] = val;
        }

        public void SetWord(ulong ptr, ushort val)
        {
            throw new System.NotImplementedException();
        }

        public void SetDWord(ulong ptr, uint val)
        {
            throw new System.NotImplementedException();
        }

        public void SetQWord(ulong ptr, ulong val)
        {
            throw new System.NotImplementedException();
        }
    }
}

