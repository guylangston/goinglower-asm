using System.Collections.Generic;
using System.IO;
using System.Linq;
using Animated.CPU.Model;

namespace Animated.CPU.Backend.LLDB
{
    public class Setup
    {
        public void InitFromDisk(string folder, Cpu cpu, SourceProvider source)
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
                    cpu.SetReg(regD.Register, regD.ValueParsed.Value);    
                }
                
            }

            cpu.Story = new Story()
            {
                Steps = new List<StoryStep>(),
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
                cpu.Story.Steps.Add(parser.ParseStep(File.ReadAllLines(stepFile)));
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