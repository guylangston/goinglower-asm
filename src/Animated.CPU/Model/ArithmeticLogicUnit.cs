using System;
using System.Collections.Generic;
using System.Linq;

namespace Animated.CPU.Model
{
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

        public ulong? GetInput(DecodedInstruction inst, DecodedArg arg)
        {
            throw new NotImplementedException();
        }

        public ulong? GetOutput<T>(DecodedInstruction inst, DecodedArg arg)
        {
            throw new NotImplementedException();
        }
    }
}