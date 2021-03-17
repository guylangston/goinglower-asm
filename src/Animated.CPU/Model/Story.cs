using System;
using System.Collections.Generic;
using System.Linq;

namespace Animated.CPU.Model
{
    public class Story
    {
        public int                      CurrentIndex      { get; set; }
        public int                      CurrentSlideIndex { get; set; }
        public List<StoryStep>          Steps             { get; set; }
        public SourceProvider           Source            { get; set; }
        public SourceFile               MainFile          { get; set; }
        public SourceFile               ReadMe            { get; set; }
        public List<MemoryView.Segment> Disasmbled        { get; set; }
        public List<StoryAnnotation>    Slides            { get; } = new List<StoryAnnotation>();
        public SourceFile?              IL                { get; set; }
        public SourceFile?              Asm               { get; set; }
        public SourceFile?              Binary            { get; set; }
        public SourceFile?              Outro             { get; set; }
        
        public StoryStep?       Current      => GeneralHelper.ByIndexOrDefault(Steps, CurrentIndex);
        public StoryAnnotation? CurrentSlide => GeneralHelper.ByIndexOrDefault(Slides, CurrentSlideIndex);
        
    }

    public enum Format
    {
        Text,
        Markdown,
        Html,
        Image
    }

    public class Tag
    {
        public string Name { get; set; }
        public string Value    { get; set; }
    }

    public class StoryAnnotation
    {
        public string?   Location    { get; set; }
        public string?   Title       { get; set; }
        public string?   Attribution { get; set; }
        public string    Text        { get; set; }
        public Format    Format      { get; set; }
        public List<Tag> Tags        { get; set; }

        public int? SlideLink { get; set; } // Store Slide link
        
    }

    public class StoryStep
    {
        public ulong                        RIP     { get; set; }
        public string                       Asm     { get; set; }
        public IReadOnlyList<RegisterDelta> Delta   { get; set; }
        public StoryAnnotation?             Comment { get; set; }
        
        public RegisterDelta? Get(string reg) 
            => Delta.FirstOrDefault(x => string.Equals(x.Register, reg, StringComparison.InvariantCultureIgnoreCase));
    }

    public class RegisterDelta
    {
        public string Register    { get; set; }
        public string ValueRaw    { get; set; }
        public ulong? ValueParsed { get; set; }
        
        public StoryAnnotation? Comment { get; set; }

        public override string ToString()
        {
            return $"{nameof(Register)}: {Register}, {nameof(ValueRaw)}: {ValueRaw}, {nameof(ValueParsed)}: {ValueParsed}";
        }
    }
}