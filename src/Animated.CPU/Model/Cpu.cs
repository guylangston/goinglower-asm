using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
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
        public Register RAX { get; } = new Register("RAX", "Accumulator")
        {
            Description = "Used in arithmetic operations",
            IdAlt = ImmutableArray.Create<string>("EAX", "EX")
        };

        public Register RBX { get; } = new Register("RBX", "Base")
        {
            IdAlt = ImmutableArray.Create<string>("EBX", "BX")
        };
        public Register RCX { get; } = new Register("RCX", "Count")
        {
            IdAlt = ImmutableArray.Create<string>("ECX", "CX")
        };
        public Register RDX { get; } = new Register("RDX", "Data");
        public Register RBP { get; } = new Register("RBP", "Stack Base Pointer");
        public Register RSI { get; } = new Register("RSI", "Source Index")
        {
            IdAlt = ImmutableArray.Create<string>("ESI", "SI")
        };
        public Register RDI { get; } = new Register("RDI", "Destination Index")
        {
            IdAlt = ImmutableArray.Create<string>("EDI", "DI")
        };
        public Register RSP { get; } = new Register("RSP", "Stack Pointer")
        {
            IdAlt = ImmutableArray.Create<string>("ESP", "SP")
        };

        public Register R0 => RAX;
        public Register R1 => RBX;
        public Register R2 => RCX;
        public Register R3 => RDX;
        public Register R4 => RBP;
        public Register R5 => RSI;
        public Register R6 => RDI;
        public Register R7 => RSP;

        public Register R8  { get; } = new Register("R8", "Register 8");
        public Register R9  { get; } = new Register("R9", "Register 9");
        public Register R10 { get; } = new Register("R10", "Register 10");
        public Register R11 { get; } = new Register("R11", "Register 11");
        public Register R12 { get; } = new Register("R12", "Register 12");
        public Register R13 { get; } = new Register("R13", "Register 13");
        public Register R14 { get; } = new Register("R14", "Register 14");
        public Register R15 { get; } = new Register("R15", "Register 15");

        public Register    RIP    { get; } = new Register("RIP", "Instruction Pointer");
        public Register    RFLAGS { get; } = new Register("RFLAGS", "Flags");
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
                if (string.Equals(register.Id, name, StringComparison.InvariantCultureIgnoreCase))
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
    }

    public class ArithmeticLogicUnit
    {
        public ArithmeticLogicUnit(Cpu cpu)
        {
            Cpu     = cpu;
            Fetch   = new PhaseFetch(this);
            Decode  = new PhaseDecode(this);
            Execute = new PhaseExecute(this);
        
        }

        public Cpu       Cpu       { get; }
        public StoryStep StoryStep => Cpu.Story.Current;

        public PhaseFetch   Fetch   { get; }
        public PhaseDecode  Decode  { get; }
        public PhaseExecute Execute { get; }
        

        public IEnumerable<object> Phases()
        {
            yield return Fetch;
            yield return Decode;
            yield return Execute;
        
        }
        
        public IEnumerable<Register> LoosyMathRegs(string asm)
        {
            
            foreach (var register in Cpu.RegisterFile)
            {
                if (asm != null && asm.Contains(register.Id, StringComparison.InvariantCultureIgnoreCase))
                {
                    yield return register;
                }
            }
        }
        
        public IEnumerable<Register> UsedRegisters()
        {
            if (StoryStep != null)
            {
                foreach (var delta in StoryStep.Delta)
                {
                    var r = Cpu.RegisterFile.FirstOrDefault(x => x.IsChanged && x.Match(delta.Register));
                    if (r != null) yield return r;
                }    
            }
        }

    }

    public class PhaseFetch
    {
        private ArithmeticLogicUnit alu;

        public PhaseFetch(ArithmeticLogicUnit alu)
        {
            this.alu = alu;
        }

        public ulong RIP    => alu.StoryStep.RIP;
        public byte[]?  Memory => alu.Cpu.Instructions?.GetByAddress(alu.StoryStep?.RIP ?? 0)?.Raw;

        public override string ToString() => "Fetch";
    }
    

    [Flags]
    public enum InOut { 
        Inp = 1, 
        Out = 2,
        InpOut = Inp + Out
    }
    
    public class DecodedArg 
    {
        public bool        IsImplied { get; set; }
        public string      Text      { get; set; }
        public Register?   Register  { get; set; }
        public InOut InOut      { get; set; }

        public override string ToString() => Text;
    }
    
    public class DecodedInstruction
    {
        public string           OpCode { get; set; }
        public List<DecodedArg> Args   { get; set; } = new List<DecodedArg>();

        public string FriendlyName   { get; set; }
        public string FriendlyMethod { get; set; }
        public string Url { get; set; }
            
        public DecodedArg A1 => Args?[0];
        public DecodedArg A2 => Args != null && Args.Count > 1 ? Args[1] : null;
        
        public static DecodedInstruction? Parse(Cpu cpu, string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return null;
            if (StringHelper.TrySplitExclusive(line, " ", out var p))
            {
                var r = new DecodedInstruction()
                {
                    OpCode = p.l.Trim().ToLower(),
                    Args   = p.r.Split(",")
                              .Select(x => new DecodedArg()
                              {
                                  Text =x.Trim() 
                              })
                              .ToList()
                };
                BuildFriendly(cpu, r);
                return r;
            }
            var ri = new DecodedInstruction()
            {
                OpCode = line.Trim()
            };
            BuildFriendly(cpu, ri);
            return ri;
        }
        
        public const string Assign = ":=";

        private static void BuildFriendly(Cpu cpu, DecodedInstruction inst)
        {
            if (inst == null) return;
            
            inst.Url = "https://www.aldeid.com/wiki/X86-assembly/Instructions/"+inst.OpCode;
            
            if (inst.OpCode == "mov")
            {
                inst.A1.InOut       = InOut.Out;
                inst.A2.InOut       = InOut.Inp;
                inst.FriendlyName   = "Move";
                inst.FriendlyMethod = $"{inst.A1} {Assign} {inst.A2}";
                return;
            }

            if (inst.OpCode == "xor")
            {
                inst.FriendlyName = "Exclusive Or";
                inst.A1.InOut      = InOut.InpOut;
                inst.A2.InOut      = InOut.Inp;
                if (inst.A1.Text == inst.A2.Text)
                {
                    inst.FriendlyMethod = $"{inst.A1} {Assign} 0";
                }
                return;
            }

            if (inst.OpCode == "inc")
            {
                inst.A1.InOut        = InOut.InpOut;
                inst.FriendlyName   = "Increment ++";
                inst.FriendlyMethod = $"{inst.A1}++";
                return;
            }
            
            if (inst.OpCode == "cmp")
            {
                inst.A1.InOut        = InOut.Inp;
                inst.A2.InOut        = InOut.Inp;
                inst.Args.Add(new DecodedArg()
                {
                    IsImplied = true,
                    Text = "RFLAGS",
                    InOut = InOut.Out
                });
                inst.FriendlyName   = "Compare";
                inst.FriendlyMethod = $"flags {Assign} {inst.A1} compare to {inst.A2}";
                return;
            }
            
            if (inst.OpCode == "test")
            {
                inst.A1.InOut = InOut.Inp;
                inst.A2.InOut = InOut.Inp;
                inst.Args.Add(new DecodedArg()
                {
                    IsImplied = true,
                    Text      = "RFLAGS",
                    InOut     = InOut.Out
                });
                inst.FriendlyName   = "AND (without changing A1)";
                inst.FriendlyMethod = $"flags {Assign} {inst.A1} AND {inst.A2}";
                return;
            }
            
            if (inst.OpCode == "jl")
            {
                inst.A1.InOut        = InOut.Inp;
                inst.FriendlyName   = "Jump If Less Than";
                inst.FriendlyMethod = $"if flags(<) rip {Assign} {inst.A1}";      // Find Previous Line (is Compare then get args)
                return;
            }
            
            if (inst.OpCode == "jle")
            {
                inst.A1.InOut       = InOut.Inp;
                inst.FriendlyName   = "Jump If Less Than Or Equal";
                inst.FriendlyMethod = $"if flags(<=) rip {Assign} {inst.A1}";      // Find Previous Line (is Compare then get args)
                return;
            }
        }

       

        public override string ToString() => OpCode.PadRight(6) +
                                             (Args == null ? "" : string.Join(',', Args));
    }

    public class PhaseDecode
    {
        private ArithmeticLogicUnit alu;

        public PhaseDecode(ArithmeticLogicUnit alu)
        {
            this.alu = alu;
        }

        public DecodedInstruction? Asm => DecodedInstruction.Parse(alu.Cpu, alu.StoryStep?.Asm);

       

        
        

        public override string ToString() => "Decode";
    }

    public class PhaseExecute
    {
        private ArithmeticLogicUnit alu;

        public PhaseExecute(ArithmeticLogicUnit alu)
        {
            this.alu = alu;
        }

        public IEnumerable<Register> Inputs => alu.LoosyMathRegs(alu.StoryStep?.Asm);

        public IEnumerable<Register> Changes => alu.UsedRegisters();

        public override string ToString() => "Execute";
    }

   
    public class Register : Prop<ulong>
    {
        public Register(string id, string name) : base(PropHelper.DefaultCompareULong, 0, null)
        {
            Id   = id;
            Name = name;
        }

        public string                Id          { get; set; }
        public IReadOnlyList<string> IdAlt       { get; set; }
        public string                Name        { get; set; }
        public string                Description { get; set; }

        public string ValueHex => Value.ToString("X").PadLeft(64 / 8 * 2, '0');

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

