using System.Collections.Generic;

namespace GoingLower.CPU.Model
{
    public class PhaseFetch
    {
        public ArithmeticLogicUnit Alu { get; }

        public PhaseFetch(ArithmeticLogicUnit alu)
        {
            this.Alu = alu;
        }

        public ulong   RIP    => Alu.StoryStep?.RIP ?? 0;
        public byte[]? Memory => Alu.Cpu.Instructions?.GetByAddress(Alu.StoryStep?.RIP ?? 0)?.Raw;

        public override string ToString() => "Fetch";
    }
    
    public class PhaseDecode
    {
        public ArithmeticLogicUnit Alu { get; }

        public PhaseDecode(ArithmeticLogicUnit alu)
        {
            this.Alu = alu;
        }

        public DecodedInstruction? DecodeResult => DecodedInstruction.Parse(Alu.Cpu, Alu.StoryStep?.Asm);
        
        public override string ToString() => "Decode";
    }

    public class PhaseExecute
    {
        public ArithmeticLogicUnit Alu { get; }

        public PhaseExecute(ArithmeticLogicUnit alu)
        {
            this.Alu = alu;
        }
        
        public string? AsmText => Alu.StoryStep?.Asm;
        public DecodedInstruction? DecodeResult => DecodedInstruction.Parse(Alu.Cpu, Alu.StoryStep?.Asm);

        
        public override string ToString() => "Execute";
    }
}