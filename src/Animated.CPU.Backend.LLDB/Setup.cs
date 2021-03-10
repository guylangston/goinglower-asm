using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Animated.CPU.Model;

namespace Animated.CPU.Backend.LLDB
{
    public class Setup
    {
        public void InitFromDisk(string folder, Cpu cpu, SourceProvider source, SourceFile mainSource)
        {
            var parser = new Parser(source);
            var fState = Path.Combine(folder, "S0000-method.clru");
            var method = parser.ParseCLRU(File.ReadAllLines(fState));
            cpu.Instructions = new MemoryView(method);
            
            var fInit = Path.Combine(folder, "S0001-step.state");
            foreach (var regD in parser.ParseRegisters(File.ReadAllLines(fInit).Skip(4)))
            {
                if (regD.ValueParsed != null)
                {
                    var r = cpu.SetReg(regD.Register, regD.ValueParsed.Value);
                    if (r != null)
                    {
                        r.IsChanged = false;
                    }
                }
            }

            cpu.Story = new Story()
            {
                Steps = new List<StoryStep>(),
                Source = source,
                MainFile = mainSource,
                ReadMe = File.ReadAllLines(Path.Combine(folder, "Readme.txt"))
            };
            cpu.Stack = new MemoryView(new[]
            {
                new MemoryView.Segment()
                {
                    SourceAsm = "TODO",
                    Raw       = ExampleCPU.RandomBytes(10)
                }
            });

            foreach (var stepFile in FindStepFiles(folder))
            {
                var step = parser.ParseStep(File.ReadAllLines(stepFile));
                if (step == null) throw new Exception($"Unable to parse: {stepFile}");
                cpu.Story.Steps.Add(step);
            }
            
            
            
            // HACKS
            // see: /home/guy/repo/cpu.anim/src/Sample/Scripts/Introduction-ForLoop/Slides.md
            cpu.Story.Steps[2].Comment = new StoryAnnotation()
            { 
                Format = Format.Text,
                Text = "This is part of the calling preamble,\n" +
                       "we can ignore it for now",
                Tags = new List<Tag>()
                {
                    new Tag()
                    {
                      Name  = "edi",
                      Value = "var count"
                    } 
                }
            };
            cpu.Story.Steps[3].Comment = new StoryAnnotation()
            { 
                Format = Format.Text,
                Text = "Just a fancy way of doing `mov eax, 0`",
                Tags = new List<Tag>()
                {
                    new Tag()
                    {
                        Name  = "eax",
                        Value = "var sum"
                    } 
                }
            };
            cpu.Story.Steps[4].Comment = new StoryAnnotation()
            { 
                Format = Format.Text,
                Tags = new List<Tag>()
                {
                    new Tag()
                    {
                        Name  = "esi",
                        Value = "var x"
                    } 
                }
            };
            
            cpu.Story.Slides.Add(new StoryAnnotation()
            {
                Title = "History",
                Format = Format.Markdown,
                Text = @"A CPU's register size covers both addressing (memory pointers) and general calculations (ADD, MUL, etc). 
Conceptually, it is easier of they are the same size, but they don't need to be. 

x86 family journey:
- 16-bit [8086](https://en.wikipedia.org/wiki/Intel_8086) chip in 1976; which is we the x86 comes from
- 32-bit [80386](https://en.wikipedia.org/wiki/Intel_80386) chip in 1985
- 64-bit [AMD-64 aka x86-64](https://en.wikipedia.org/wiki/X86-64) design by AMD (not Intel) in 2000 for the AMD K8 chips. 
  Intel's cleaner but not backwards compatible IA-64 effectively failed in the marketplace.
- After 64-bit we got special purpose 128-bit computation with MMX and onwards. (These are out of scope now)

TODO: Diagram with die-size, clock speed, and transistor count

x86-64 allows 64-bit addresses @RIP but 32-bit general registers @EAX, @EBX, etc. This is effectively the dotnet model."
            });
        }
        
        private IEnumerable<string> FindStepFiles(string folder)
        {
            int cc = 2;
            while(true)
            {
                var file = Path.Combine(folder, $"S{cc.ToString().PadLeft(4, '0')}-step.state");
                if (File.Exists(file))
                {
                    yield return file;
                }
                else
                {
                    break;
                }
                cc++;
            }
        }
    }
}