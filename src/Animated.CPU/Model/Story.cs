using System;
using System.Collections.Generic;
using System.Linq;

namespace Animated.CPU.Model
{
    public class Story
    {
        public int             CurrentIndex { get; set; }
        public List<StoryStep> Steps        { get; set; }

        public StoryStep      Current => Steps[CurrentIndex];
        public SourceProvider Source  { get; set; }
    }
    
    public class RegisterDelta
    {
        public string Register    { get; set; }
        public string ValueRaw { get; set; }
        public ulong? ValueParsed { get; set; }

        public override string ToString()
        {
            return $"{nameof(Register)}: {Register}, {nameof(ValueRaw)}: {ValueRaw}, {nameof(ValueParsed)}: {ValueParsed}";
        }
    }

    public class StoryStep
    {
        public ulong                        RIP   { get; set; }
        public string                       Asm   { get; set; }
        public IReadOnlyList<RegisterDelta> Delta { get; set; }

        public RegisterDelta? Get(string reg) 
            => Delta.FirstOrDefault(x => string.Equals(x.Register, reg, StringComparison.InvariantCultureIgnoreCase));
    }
}