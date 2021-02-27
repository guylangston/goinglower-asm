using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using System.Threading;
using Animated.CPU.Model;

namespace Animated.CPU.Backend.LLDB
{


    public class DebugDriverLldb : ConsoleDriver
    {
        private (string addr, string inst) next;
        private (string addr, string inst) curr;
        private Regex matchBP = new Regex("--address 0x[0-9A-F]{16}");
        private SourceProvider source;
        
        public void Start()
        {
            source = new SourceProvider()
            {
                TargetBinary = "/home/guy/RiderProjects/ConsoleApp1/ConsoleApp1/bin/Release/net5.0/ConsoleApp1"
            };
            CapturePath = Path.GetDirectoryName(source.TargetBinary);
            source.Load("/home/guy/RiderProjects/ConsoleApp1/ConsoleApp1/Program.cs");
            
            var start = new ProcessStartInfo()
            {
                FileName         = "/usr/bin/lldb",
                Arguments        = "./ConsoleApp1",
                WorkingDirectory = "/home/guy/RiderProjects/ConsoleApp1/ConsoleApp1/bin/Release/net5.0/",
                WindowStyle = ProcessWindowStyle.Normal,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
            };

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
        
        
        private IEnumerable<MemoryView.Segment> DisassembleMethod(string bpAddr)
        {
            var lines = ExecuteAndWaitForResults($"clru {bpAddr}", 1, false);

            Capture(lines, "method", "clru");
            
            // 1. Scan to "Begin 00007FFF7DD360B0, size 82"
            // 2. if line starts with path => Capture SourceMap
            // 3. Append instructions until empty line

            var i = 0;
            while (i < lines.Count && !lines[i].StartsWith("Begin "))
            {
                i++;
            }
            if (i >= lines.Count) throw new Exception("Bad Parse");

            var minLengh = "00007fff7dd360ff 8b45e4               mov     eax".Length;

            string currSource = null;
            uint    offset     = 0;
            while (i < lines.Count)
            {
                var ll = lines[i];
                if (ll.StartsWith("/"))
                {
                    currSource = ll;
                }
                else if (ll.Length >= minLengh && char.IsLetterOrDigit(ll[0]))
                {
                    var inst = ParseInstruction(ll, currSource);
                    if (inst != null)
                    {
                        inst.Offset =  offset;
                        offset      += (uint)inst.Raw.Length;
                        yield return inst;
                    }
                }
                i++;
            }

        }
        
        MemoryView.Segment ParseInstruction(string ll, string currSource)
        {
            //---------|---------|---------|---------|---------|---------|
            //012345678901234567890123456789012345678901234567890123456789
            //00007fff7dd360ff 8b45e4               mov     eax

            var bytes = ll[17..37].Trim();
            var arr   = ll[0..16];
            return new MemoryView.Segment()
            {
                Address   = ulong.Parse(arr, NumberStyles.HexNumber),
                Raw       = Convert.FromHexString(bytes),
                SourceAsm = ll[38..],
                Anchor    = source.FindAnchor(currSource)
            };
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
        
        private void CaptureRegisters(Cpu cpu, bool all = false)
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
            foreach (var line in lines)
            {
                if (StringHelper.TrySplitExclusive(line, " = ", out var r))
                {
                    var reg = cpu.SetReg(r.l.Trim(), r.r.Trim());
                    if (reg != null)
                    {
                        var diff = reg.Value > reg.Prev
                        ? reg.Value - reg.Prev
                        : reg.Prev - reg.Value;
                        Console.WriteLine($"\t{reg.Id,8}: {reg.Prev.ToString("X"),16} -> {reg.Value.ToString("X"),16} (diff: {diff.ToString("X"),16})");
                    }
                }
            }
        }

        
        
        private string ConfirmBreakPoint(string ident)
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
