using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

namespace Animated.CPU.Model
{
    public class DecodedInstruction
    {
        public string            Text  { get; set; }
        public string            OpCode { get; set; }
        public List<DecodedArg>? Args   { get; set; }

        public string? FriendlyName   { get; set; }
        public string? FriendlyMethod { get; set; }
        public string? Url            { get; set; }
        public string? Description    { get; set; }
            
        public DecodedArg? A1 => Args != null && Args.Count > 0 ? Args[0] : null;
        public DecodedArg? A2 => Args != null && Args.Count > 1 ? Args[1] : null;
        
        public static DecodedInstruction? Parse(Cpu cpu, string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return null;
            if (StringHelper.TrySplitExclusive(line, " ", out var p))
            {
                var r = new DecodedInstruction()
                {
                    Text = line,
                    OpCode = p.l.Trim().ToLower(),
                    Args = new List<DecodedArg>()
                };
                
                foreach (var (index, val) in p.r.Split(",").WithIndex())
                {
                    var a = new DecodedArg()
                    {
                        Index = index,
                        Value = val.Trim()
                    };
                    a.ParseArg(cpu, r, line);
                    r.Args.Add(a);
                }
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
        
        public const string Assign = "<=:";

        private static void BuildFriendly(Cpu cpu, DecodedInstruction inst)
        {
            if (inst == null) return;
            
            inst.Url = LookupUrlForOpCode(inst.OpCode);
            
            if (inst.OpCode == "mov")
            {
                inst.A1.InOut       = InOut.Out;
                inst.A2.InOut       = InOut.In;
                inst.FriendlyName   = "Move / Set / Assign";
                inst.FriendlyMethod = $"{inst.A1} {Assign} {inst.A2}";
                return;
            }
            
            if (inst.OpCode == "lea")
            {
                inst.A1.InOut       = InOut.Out;
                inst.A2.InOut       = InOut.In;
                inst.FriendlyName   = "Load Effective Address";
                inst.FriendlyMethod = $"{inst.A1} {Assign} {inst.A2}";
                return;
            }

            if (inst.OpCode == "xor")
            {
                inst.FriendlyName = "Bitwise Exclusive Or";
                inst.A1.InOut     = InOut.InOut;
                inst.A2.InOut     = InOut.In;
                if (inst.A1.Value == inst.A2.Value)
                {
                    inst.FriendlyMethod = $"{inst.A1} {Assign} 0";
                }
                inst.Description = "BIT Operator\n" +
                                   "0 XOR 0 = 0\n" +
                                   "1 XOR 0 = 1\n" +
                                   "0 XOR 1 = 1\n" +
                                   "1 XOR 1 = 0";
                return;
            }
            if (inst.OpCode == "and")
            {
                inst.FriendlyName = "Bitwise AND";
                inst.A1.InOut     = InOut.InOut;
                inst.A2.InOut     = InOut.In;
                inst.Description = "BIT Operator\n" +
                                   "0 AND 0 = 0\n" +
                                   "1 AND 0 = 0\n" +
                                   "0 AND 1 = 0\n" +
                                   "1 AND 1 = 1";
                return;
            }
            if (inst.OpCode == "or")
            {
                inst.FriendlyName = "Bitwise OR";
                inst.A1.InOut     = InOut.InOut;
                inst.A2.InOut     = InOut.In;
                inst.Description = "BIT Operator\n" +
                                   "0 OR 0 = 0\n" +
                                   "1 OR 0 = 1\n" +
                                   "0 OR 1 = 1\n" +
                                   "1 OR 1 = 1";
                return;
            }

            if (inst.OpCode == "inc")
            {
                inst.A1.InOut       = InOut.InOut;
                inst.FriendlyName   = "Increment ++";
                inst.FriendlyMethod = $"{inst.A1}++";
                return;
            }
            
            if (inst.OpCode == "dev")
            {
                inst.A1.InOut       = InOut.InOut;
                inst.FriendlyName   = "Decrement --";
                inst.FriendlyMethod = $"{inst.A1}--";
                return;
            }
            
            if (inst.OpCode == "cmp")
            {
                inst.A1.InOut = InOut.In;
                inst.A2.InOut = InOut.In;
                inst.Args.Add(new DecodedArg()
                {
                    IsImplied = true,
                    Value     = "RFLAGS",
                    Register  = cpu.RFLAGS,
                    InOut     = InOut.Out
                });
                inst.FriendlyName   = "Compare";
                inst.FriendlyMethod = $"flags {Assign} {inst.A1} compare to {inst.A2}";
                return;
            }
            
            if (inst.OpCode == "test")
            {
                inst.A1.InOut = InOut.In;
                inst.A2.InOut = InOut.In;
                inst.Args.Add(new DecodedArg()
                {
                    IsImplied = true,
                    Value     = "RFLAGS",
                    Register = cpu.RFLAGS,
                    InOut     = InOut.Out
                });
                inst.FriendlyName   = "Test using Bitwise AND";
                inst.FriendlyMethod = $"flags {Assign} {inst.A1} AND {inst.A2}";
                return;
            }
            
            if (inst.OpCode == "jl")
            {
                inst.Url            = LookupUrlForOpCode("Jcc");
                inst.A1.InOut       = InOut.In;
                inst.A1.Description = "Destination";
                inst.FriendlyName   = "Jump short if less (SF≠ OF).";
                inst.FriendlyMethod = $"if flags[LessThan] rip {Assign} {inst.A1}";      // Find Previous Line (is Compare then get args)
                
                inst.Args.Add(new DecodedArg()
                {
                    IsImplied = true,
                    Value     = "FLAGS",
                    Register  = cpu.RFLAGS,
                    InOut     = InOut.In
                });
                return;
            }
            
            if (inst.OpCode == "jle")
            {
                inst.Url            = LookupUrlForOpCode("Jcc");
                inst.A1.InOut       = InOut.In;
                inst.FriendlyName   = "Jump short if less or equal (ZF=1 or SF≠ OF).";
                inst.FriendlyMethod = $"if flags(<=) rip {Assign} {inst.A1}";      // Find Previous Line (is Compare then get args)
                return;
            }
            
            if (inst.OpCode == "ret")
            {
                inst.FriendlyName   = "return to caller";
                inst.FriendlyMethod = "pop the last call from the instruction stack and jump to it,";
                return;
            }

            inst.FriendlyName   = null;
            inst.FriendlyMethod = null;
            if (inst.A1 != null) inst.A1.InOut = InOut.Out | InOut.InComplete;
            if (inst.A2 != null) inst.A2.InOut = InOut.In  | InOut.InComplete;
        }

        private static string LookupUrlForOpCode(string opcode)
        {
            return $"https://www.aldeid.com/wiki/X86-assembly/Instructions/{opcode}";
        }

        public override string ToString() => OpCode.PadRight(6) +
                                             (Args == null ? "" : string.Join(',', Args));
    }
}