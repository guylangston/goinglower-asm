using System.Collections.Generic;

namespace Animated.CPU.Model
{
    public class Story
    {
        public int             CurrentIndex { get; set; }
        public List<StoryStep> Steps        { get; set; }

        public StoryStep Current => Steps[CurrentIndex];
    }
    
    public struct RegisterDelta
    {
        public string Register    { get; set; }
        public string ValueString { get; set; }
        public ulong? ValueParsed { get; set; }
    }

    public class StoryStep
    {
        public ulong                        RIP   { get; set; }
        public string                       Asm   { get; set; }
        public IReadOnlyList<RegisterDelta> Delta { get; set; }
    }
}