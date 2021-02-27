using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using System.Threading;
using Animated.CPU.Model;

namespace Animated.CPU.Backend.LLDB
{
    public class DebuggerDriver : ConsoleDriver
    {
        private (string addr, string inst) next;
        private (string addr, string inst) curr;
        private Regex matchBP = new Regex("--address 0x[0-9A-F]{16}");
        private SourceProvider source;
        private Parser parser;

        public class ConfigArgs
        {
            public string TargetBinary     { get; set; } = "/home/guy/RiderProjects/ConsoleApp1/ConsoleApp1/bin/Release/net5.0/ConsoleApp1";
            public string WorkingDirectory { get; set; } = "/home/guy/RiderProjects/ConsoleApp1/ConsoleApp1/bin/Release/net5.0/";
            public string PathLLDB         { get; set; } = "/usr/bin/lldb";
            public string SourceFile       { get; set; } = "/home/guy/RiderProjects/ConsoleApp1/ConsoleApp1/Program.cs";

        }
        
        public void Start(ConfigArgs args)
        {
            source = new SourceProvider()
            {
                TargetBinary = args.TargetBinary
            };
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
            ExecuteAndWaitForResults("bpmd Program.cs:30");
            ExecuteAndWaitForResults("run", 1d);
            
            var bpAddr = ConfirmBreakPoint("ProgramC.M3()");
            if (bpAddr == null) throw new Exception("BP not found");
            
            var source = DisassembleMethod(bpAddr).ToArray();
            foreach (var inst in source)
            {
                Console.WriteLine($"// {inst}");
            }

            CaptureRegisters(cpu, true);

            for (int i = 0; i < 20; i++)
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
            parser.ParseRegisters(cpu, lines);
        }
       

        string ConfirmBreakPoint(string ident)
        {
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
