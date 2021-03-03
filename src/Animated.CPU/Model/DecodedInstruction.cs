using System.Collections.Generic;
using System.Linq;

namespace Animated.CPU.Model
{
    public class DecodedArg 
    {
        public bool      IsImplied { get; set; }
        public string    Text      { get; set; }
        public Register? Register  { get; set; }
        public InOut     InOut     { get; set; }

        public override string ToString() => Text;
    }
    
    public class DecodedInstruction
    {
        public string           OpCode { get; set; }
        public List<DecodedArg> Args   { get; set; } = new List<DecodedArg>();

        public string FriendlyName   { get; set; }
        public string FriendlyMethod { get; set; }
        public string Url            { get; set; }
            
        public DecodedArg? A1 => Args?[0];
        public DecodedArg? A2 => Args != null && Args.Count > 1 ? Args[1] : null;
        
        public static DecodedInstruction? Parse(Cpu cpu, string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return null;
            if (StringHelper.TrySplitExclusive(line, " ", out var p))
            {
                var r = new DecodedInstruction()
                {
                    OpCode = p.l.Trim().ToLower(),
                    Args = p.r.Split(",")
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
                inst.A2.InOut       = InOut.In;
                inst.FriendlyName   = "Move";
                inst.FriendlyMethod = $"{inst.A1} {Assign} {inst.A2}";
                return;
            }

            if (inst.OpCode == "xor")
            {
                inst.FriendlyName = "Exclusive Or";
                inst.A1.InOut     = InOut.InOut;
                inst.A2.InOut     = InOut.In;
                if (inst.A1.Text == inst.A2.Text)
                {
                    inst.FriendlyMethod = $"{inst.A1} {Assign} 0";
                }
                return;
            }

            if (inst.OpCode == "inc")
            {
                inst.A1.InOut       = InOut.InOut;
                inst.FriendlyName   = "Increment ++";
                inst.FriendlyMethod = $"{inst.A1}++";
                return;
            }
            
            if (inst.OpCode == "cmp")
            {
                inst.A1.InOut = InOut.In;
                inst.A2.InOut = InOut.In;
                inst.Args.Add(new DecodedArg()
                {
                    IsImplied = true,
                    Text      = "RFLAGS",
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
                    Text      = "RFLAGS",
                    InOut     = InOut.Out
                });
                inst.FriendlyName   = "AND (without changing A1)";
                inst.FriendlyMethod = $"flags {Assign} {inst.A1} AND {inst.A2}";
                return;
            }
            
            if (inst.OpCode == "jl")
            {
                inst.A1.InOut       = InOut.In;
                inst.FriendlyName   = "Jump If Less Than";
                inst.FriendlyMethod = $"if flags(<) rip {Assign} {inst.A1}";      // Find Previous Line (is Compare then get args)
                return;
            }
            
            if (inst.OpCode == "jle")
            {
                inst.A1.InOut       = InOut.In;
                inst.FriendlyName   = "Jump If Less Than Or Equal";
                inst.FriendlyMethod = $"if flags(<=) rip {Assign} {inst.A1}";      // Find Previous Line (is Compare then get args)
                return;
            }

            inst.FriendlyName   = "???";
            inst.FriendlyMethod = "TODO";
            if (inst.A1 != null) inst.A1.InOut = InOut.Out | InOut.InComplete;
            if (inst.A2 != null) inst.A2.InOut = InOut.In  | InOut.InComplete;
        }

       

        public override string ToString() => OpCode.PadRight(6) +
                                             (Args == null ? "" : string.Join(',', Args));
    }
}