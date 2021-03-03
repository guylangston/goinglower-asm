using System.Collections.Generic;

namespace Animated.CPU.Model
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

        public DecodedInstruction? Asm => DecodedInstruction.Parse(Alu.Cpu, Alu.StoryStep?.Asm);
        
        public override string ToString() => "Decode";
    }

    public class PhaseExecute
    {
        public ArithmeticLogicUnit Alu { get; }

        public PhaseExecute(ArithmeticLogicUnit alu)
        {
            this.Alu = alu;
        }
        
        public DecodedInstruction? Asm => DecodedInstruction.Parse(Alu.Cpu, Alu.StoryStep?.Asm);

        public IEnumerable<Register> Inputs => Alu.LoosyMathRegs(Alu.StoryStep?.Asm);

        public IEnumerable<Register> Changes => Alu.UsedRegisters();

        public override string ToString() => "Execute";
    }
}