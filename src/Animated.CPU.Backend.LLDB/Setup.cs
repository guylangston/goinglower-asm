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
                Binary     = cfg.ReadAsTextLinesElseNull(source, "Code.bin"),
                Outro      = cfg.ReadAsTextLinesElseNull(source, "Outro.md"),
            };

            foreach (var stepFile in FindStepFiles(cfg.BaseFolder))
            {
                var step = parser.ParseStep(File.ReadAllLines(stepFile));
                if (step == null) throw new Exception($"Unable to parse: {stepFile}");
                story.Steps.Add(step);
            }
            
            
            foreach (var slideFile in FindSlideFiles(cfg.BaseFolder))
            {
                story.Slides.Add(ParseSlide(slideFile));
            }

            if (!story.Slides.Any())
            {
                story.Slides.Add(new StoryAnnotation()
                {
                    Title = "No Slides",
                    Text = "Sorry, there no slides available"
                });
            }

            TryEnrich(story, cfg);

            return story;
        }

        private StoryAnnotation ParseSlide(string slideFile)
        {
            // TODO: Front Matter: Title, Name, etc
            return new StoryAnnotation()
            {
                Text = File.ReadAllText(slideFile),
                Format = Format.Markdown
            };
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
        
        private IEnumerable<string> FindSlideFiles(string folder)
        {
            int cc = 1;
            while(true)
            {
                var file = Path.Combine(folder, $"Slide-{cc.ToString().PadLeft(3, '0')}.md");
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