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
        public ulong     RIP       => Cpu.Story.Current.RIP;

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
        
        public RegisterDelta? Get(StoryStep? step, DecodedInstruction inst, DecodedArg arg)
        {
            if (arg.IsImmediate)
            {
                return new RegisterDelta()
                {
                    Register    = "Ox",
                    ValueRaw    = arg.Value,
                    ValueParsed = ParseHelper.ParseHexWord(arg.Value.Remove(0, 2))
                };
            }
            
            if (step == null) return null;
            var r = Cpu.GetReg(arg.Value);
            if (r != null)
            {
                foreach (var id in r.AllIds())
                {
                    var dt = step.Get(id);
                    if (dt != null) return dt;
                }
            }
            return null;
        }
        
        public RegisterDelta? GetInput(DecodedInstruction inst, DecodedArg arg)
        {

            if (Cpu.Story.CurrentIndex > 0)
            {
                var ii  = Cpu.Story.Steps[Cpu.Story.CurrentIndex - 1];

                return Get(ii, inst, arg);
            }

            return null;
        }

        public RegisterDelta? GetOutput(DecodedInstruction inst, DecodedArg arg)
        {
            var d = Get(StoryStep, inst, arg);
            if (d != null) return d;

            var r = Cpu.GetReg(arg.Value);
            if (r != null) return r.ToDelta();
            return null;
        }

    }
}