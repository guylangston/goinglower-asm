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
                MainFile = mainSource
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