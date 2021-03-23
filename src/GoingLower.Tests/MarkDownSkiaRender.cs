using System;
using Markdig;
using Markdig.Renderers;
using Markdig.Syntax;
using Xunit;
using Xunit.Abstractions;

namespace GoingLower.Tests
{
    // TODO: Move to git branch / experimental
    public class MarkDownSkiaRender
    {
        private ITestOutputHelper outp;

        public MarkDownSkiaRender(ITestOutputHelper outp)
        {
            this.outp = outp;
        }

        class TestRenderer : IMarkdownRenderer
        {
            private ITestOutputHelper outp;

            public TestRenderer(ITestOutputHelper outp)
            {
                this.outp         =  outp;
                ObjectWriteBefore += OnObjectWriteBefore;
            }

            private void OnObjectWriteBefore(IMarkdownRenderer arg1, MarkdownObject arg2)
            {
                outp.WriteLine(arg2?.ToString());
            }

            public object Render(MarkdownObject markdownObject)
            {
                outp.WriteLine(markdownObject?.ToString());
                return "TODO";
            }

            public ObjectRendererCollection                        ObjectRenderers { get; } = new ObjectRendererCollection();
            public event Action<IMarkdownRenderer, MarkdownObject> ObjectWriteBefore;
            public event Action<IMarkdownRenderer, MarkdownObject> ObjectWriteAfter;
        }
        
        [Fact]
        public void CanParse()
        {
            var txt = @"## History

A CPU's register size covers both addressing (memory pointers) and general calculations (ADD, MUL, etc). 
Conceptually, it is easier of they are the same size, but they don't need to be. 

x86 family journey:
- 16-bit [8086](https://en.wikipedia.org/wiki/Intel_8086) chip in 1976; which is we the x86 comes from
- 32-bit [80386](https://en.wikipedia.org/wiki/Intel_80386) chip in 1985
- 64-bit [AMD-64 aka x86-64](https://en.wikipedia.org/wiki/X86-64) design by AMD (not Intel) in 2000 for the AMD K8 chips. 
  Intel's cleaner but not backwards compatible IA-64 effectively failed in the marketplace.
- After 64-bit we got special purpose 128-bit computation with MMX and onwards. (These are out of scope now)

TODO: Diagram with die-size, clock speed, and transistor count

x86-64 allows 64-bit addresses @RIP but 32-bit general registers @EAX, @EBX, etc. This is effectively the dotnet model.
";
            
            var pipeline = new MarkdownPipelineBuilder()
                .Build();
            
            pipeline.Setup(new TestRenderer(outp));
            
            var result = Markdown.ToHtml(txt, pipeline);
            Assert.NotEmpty(result);
            
            
        }
    }
}