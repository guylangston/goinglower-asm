using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        
        public void Start()
        {
            var start = new ProcessStartInfo()
            {
                FileName         = "/usr/bin/lldb",
                Arguments        = "./ConsoleApp1",
                WorkingDirectory = "/home/guy/RiderProjects/ConsoleApp1/ConsoleApp1/bin/Debug/net5.0/",
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
            
            //ExecuteAndWaitForResults("clrstack");
            DisassembleMethod(bpAddr);
            


            for (int i = 0; i < 20; i++)
            {
                CaptureRegisters(cpu);
                Step(cpu);
                cpu.CLK.Value++;
            }
            CaptureRegisters(cpu);
            
            ExecuteAndWaitForResults("quit");
        }
        
        
        private void DisassembleMethod(string bpAddr)
        {
            ExecuteAndWaitForResults($"clru {bpAddr}");
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
        
        private void CaptureRegisters(Cpu cpu)
        {
            var lines = ExecuteAndWaitForResults("register read", echo: false);
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