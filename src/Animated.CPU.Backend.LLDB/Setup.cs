using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Animated.CPU.Model;

namespace Animated.CPU.Backend.LLDB
{
    public class Setup
    {
        public class Config
        {
            public string CompileBaseFolder { get; set; }  // /home/guy/repo/cpu.anim/src/Sample
            public string BaseFolder        { get; set; }  // /home/guy/repo/cpu.anim/src/Sample/Scripts/Introduction-ForLoop
            public string StoryId           { get; set; }  // Introduction-ForLoop
            
            public bool TryGetPath(string rel, out string fullPath)
            {
                var p = System.IO.Path.Combine(BaseFolder, rel);
                if (Directory.Exists(p) || File.Exists(p))
                {
                    fullPath = p;
                    return true;
                }

                fullPath = null;
                return false;
            }
            
            public string GetExpectedPath(string rel)
            {
                if (TryGetPath(rel, out var p))
                {
                    return p;
                }
                throw new Exception($"Not found: {rel}");
            }

            public SourceFile ReadAsTextLinesElseNull(SourceProvider prov, string rel)
            {
                if (TryGetPath(rel, out var p))
                {
                    return prov.Load(p);
                }
                return null;
            }
        }
        
        public void InitCpuFromDisk(Config cfg, Cpu cpu)
        {
            cpu.Story = BuildStory(cfg);
            
            cpu.Instructions = new MemoryView(cpu.Story.Disasmbled);
            
            foreach (var regD in cpu.Story.Steps.First().Delta)
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

        }

        public Story BuildStory(Config cfg)
        {
            var source = new SourceProvider();
            var main   = source.Load(cfg.GetExpectedPath("CodeClean.txt"));
            main.Title = "Source.cs";
            
            var parser = new Parser(source);
            var dis = parser.ParseCLRU(File.ReadAllLines(cfg.GetExpectedPath("S0000-method.clru"))).ToList();
            
            var story = new Story()
            {
                Steps      = new List<StoryStep>(),
                Source     = source,
                Disasmbled = dis,
                MainFile   = main,
                ReadMe     = cfg.ReadAsTextLinesElseNull(source, "Readme.txt"),
                IL         = cfg.ReadAsTextLinesElseNull(source, "Code.il"),
                Asm        = cfg.ReadAsTextLinesElseNull(source, "Code.asm"),
                Binary      = cfg.ReadAsTextLinesElseNull(source, "Code.bin"),
                Outro = cfg.ReadAsTextLinesElseNull(source, "Outro.md"),
            };

            foreach (var stepFile in FindStepFiles(cfg.BaseFolder))
            {
                var step = parser.ParseStep(File.ReadAllLines(stepFile));
                if (step == null) throw new Exception($"Unable to parse: {stepFile}");
                story.Steps.Add(step);
            }

            TryEnrich(story, cfg);

            return story;
        }

        private void TryEnrich(Story story, Config cfg)
        {
            if (cfg.StoryId == "Introduction-ForLoop")
            {
                Enrich_IntroductionForLoop(story, cfg);
                
            }
        }

        private void Enrich_IntroductionForLoop(Story story, Config cfg)
        {
             // HACKS
            // see: /home/guy/repo/cpu.anim/src/Sample/Scripts/Introduction-ForLoop/Slides.md
            story.Steps[2].Comment = new StoryAnnotation()
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
            story.Steps[3].Comment = new StoryAnnotation()
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
            story.Steps[4].Comment = new StoryAnnotation()
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
            
            story.Slides.Add(new StoryAnnotation()
            {
                Title = "History",
                Format = Format.Markdown,
                Text = 
@"
A CPU's register size covers both addressing (memory pointers) and general calculations 
(ADD, MUL, etc). Conceptually, it is easier of they are the same size, 
but they don't need to be. 

x86 family journey:
- 16-bit [8086](https://en.wikipedia.org/wiki/Intel_8086) chip in 1976; 
  which is we the x86 comes from
- 32-bit [80386](https://en.wikipedia.org/wiki/Intel_80386) chip in 1985
- 64-bit [AMD-64 aka x86-64](https://en.wikipedia.org/wiki/X86-64) design by 
  AMD (not Intel) in 2000 for the AMD K8 chips. 
  Intel's cleaner but not backwards compatible IA-64 effectively failed in the marketplace.
- After 64-bit we got special purpose 128-bit computation with MMX and onwards. 
  (These are out of scope now)

TODO: Diagram with die-size, clock speed, and transistor count

x86-64 allows 64-bit addresses @RIP but 32-bit general registers @EAX, @EBX, etc. 
This is effectively the dotnet model."
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