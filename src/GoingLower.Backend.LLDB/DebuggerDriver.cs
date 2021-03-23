using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using System.Threading;
using GoingLower.Core.Helpers;
using GoingLower.CPU.Model;

namespace GoingLower.Backend.LLDB
{
    public class DebuggerDriver : ConsoleDriver
    {
        private (string addr, string inst) next;
        private (string addr, string inst) curr;
        private Regex matchBP = new Regex("--address 0x[0-9A-F]{16}");
        private SourceProvider source;
        private Parser parser;
        private ConfigArgs args;

        public class ConfigArgs
        {
            // /home/guy/repo/cpu.anim/src/Sample/Sample.csproj
            public void WithProjectDir(string proj, 
                string source,
                int? breakOnLine,
                string confirm
                )
            {
                string relRuntime = "bin/Release/net5.0/linux-x64/publish";
                if (!Directory.Exists(proj))
                {
                    throw new Exception($"Directory Not Found: (rel):{proj}");
                }
                ProjectDir       = proj;
                WorkingDirectory = Path.Combine(proj, relRuntime);
                if (!Directory.Exists(WorkingDirectory))
                {
                    throw new Exception($"Directory Not Found: (WorkingDirectory):{WorkingDirectory}");
                }

                var projName = Path.GetFileName(proj);
                TargetBinary     = Path.Combine(WorkingDirectory, projName);
                if (!File.Exists(TargetBinary))
                {
                    throw new Exception($"File Not Found: (TargetBinary):{TargetBinary}");
                }
                
                SourceFile       = Path.Combine(proj, source);
                if (!File.Exists(SourceFile))
                {
                    throw new Exception($"File Not Found: (SourceFile):{SourceFile}");
                }

                if (breakOnLine == null)
                {
                    var all = File.ReadAllLines(SourceFile);
                    var bk  = all.WithIndex().FirstOrDefault(x => x.val != null && x.val.Contains("// Break-Here"));
                    if (bk.index > 0)
                    {
                        breakOnLine = bk.index + 1;
                    }
                    else
                    {
                        throw new Exception("Cannot find marker `// Break-Here` ");
                    }
                }

                BreakPoint             = $"{source}:{breakOnLine}";
                BreakPointConfirmation = confirm;


            }
            
            // With Defaults
            public string PathLLDB  { get; set; } = "/usr/bin/lldb";
            public int    StepCount { get; set; } = 50; 

            // No Default 
            public string ProjectDir             { get; set; }
            public string TargetBinary           { get; set; } // "/home/guy/RiderProjects/ConsoleApp1/ConsoleApp1/bin/Release/net5.0/ConsoleApp1";
            public string WorkingDirectory       { get; set; } // "/home/guy/RiderProjects/ConsoleApp1/ConsoleApp1/bin/Release/net5.0/";
            public string SourceFile             { get; set; } // "/home/guy/RiderProjects/ConsoleApp1/ConsoleApp1/Program.cs";
            public string BreakPoint             { get; set; } // "Program.cs:30";
            public string BreakPointConfirmation { get; set; } // "ProgramC.M3()";
             
        }
        
        public void Start(ConfigArgs args)
        {
            this.args = args ?? throw new ArgumentNullException(nameof(args));
            if (string.IsNullOrWhiteSpace(args.BreakPoint)) throw new Exception("Requires Breakpoint eg 'Program.cs:30'");
            if (string.IsNullOrWhiteSpace(args.BreakPointConfirmation)) throw new Exception("Requires Breakpoint Confirmation eg 'Program.Main()'");


            Console.WriteLine("{0,20}: {1}", "ProjectDir", args.ProjectDir);
            Console.WriteLine("{0,20}: {1}", "WorkingDirectory", args.WorkingDirectory);
            Console.WriteLine("{0,20}: {1}", "TargetBinary", args.TargetBinary);
            Console.WriteLine("{0,20}: {1}", "SourceFile", args.SourceFile);

            source      = new SourceProvider(args.TargetBinary);
            CapturePath = Path.GetDirectoryName(source.TargetBinary);
            
            source.Load(args.SourceFile);
            
            var start = new ProcessStartInfo()
            {
                FileName               = args.PathLLDB,
                Arguments              = args.TargetBinary,
                WorkingDirectory       = args.WorkingDirectory,
                WindowStyle            = ProcessWindowStyle.Normal,
                RedirectStandardError  = true,
                RedirectStandardInput  = true,
                RedirectStandardOutput = true,
            };

            this.parser = new Parser(source);
            Open(start);
            
        }
        
        protected override void ProcessOutput()
        {
            var cpu = new Cpu();

            ExecuteAndWaitForResults("version");
            ExecuteAndWaitForResults($"bpmd {args.BreakPoint}");
            ExecuteAndWaitForResults("run", 1d);
            
            var bpAddr = ConfirmBreakPoint(args.BreakPointConfirmation);
            if (bpAddr == null) throw new Exception("BP not found");
            
            var source = DisassembleMethod(bpAddr).ToArray();
            foreach (var inst in source)
            {
                Console.WriteLine($"// {inst}");
            }

            CaptureRegisters(cpu, true);

            for (int i = 0; i < args.StepCount; i++)
            {
                CaptureRegisters(cpu);
                Step(cpu);
                cpu.CLK.Value++;
            }
            CaptureRegisters(cpu);
            
            ExecuteAndWaitForResults("quit");
        }

        

        IEnumerable<MemoryView.Segment> DisassembleMethod(string bpAddr)
        {
            var lines = ExecuteAndWaitForResults($"clru {bpAddr}", 1, false);

            Capture(lines, "method", "clru");

            return parser.ParseCLRU(lines);
        }


        void Step(Cpu cpu)
        {
            curr = next;
            var lines = ExecuteAndWaitForResults("si", echo:false);
            var t     = lines.FirstOrDefault(x => x.Trim().StartsWith("->"));
            if (t != null)
            {
                t = t.Trim().Remove(0, "-> ".Length).Trim();
                if (StringHelper.TrySplitExclusive(t, ": ", out var r))
                {
                    next = r;
                }
            }
        }
        
        void CaptureRegisters(Cpu cpu, bool all = false)
        {
            var lines = ExecuteAndWaitForResults("register read" + (all ? " --all":""), echo: false);
            Capture(Enumerable.Union(
                (IEnumerable<string>)new string[]
                {
                    $"curr={curr}",
                    $"next={next}",
                    $"RIP={cpu.RIP.ValueHex}"
                },
                (IEnumerable<string>)lines
            ), "step", "state");
            if (curr.addr != null) Console.WriteLine($"[{curr.addr}] {curr.inst}");
            foreach (var rd in parser.ParseRegisters(lines))
            {
                if (rd.ValueParsed != null)
                {
                    cpu.SetReg(rd.Register, rd.ValueParsed.Value);    
                }
                
            }
             
        }
       

        string ConfirmBreakPoint(string ident)
        {
            if (ident == null) return "<Skipped>";
            IReadOnlyList<string> cmdTxt = LastResult;

            var l = cmdTxt.FirstOrDefault(x => x.Contains("Setting breakpoint: breakpoint set"));
            if (l == null)
            {
                throw new Exception("Could not find ANY breakpoints");
            }

            if (!l.Contains(ident))
            {
                throw new Exception($"break points found, but not {ident} ({l})");
            }

            var m = matchBP.Match(l);
            if (m != null)
            {
                return m.Value.Remove(0, "--address ".Length);
            }

            return null;
        }
        
        
    }
}
