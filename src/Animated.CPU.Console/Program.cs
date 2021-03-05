using System;
using Animated.CPU.Backend.LLDB;

namespace Animated.CPU.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var p    = new DebuggerDriver();
            var cfg  = new DebuggerDriver.ConfigArgs();
            var name = "PerfStructSize";
            cfg.WithProjectDir(
                "/home/guy/repo/cpu.anim/src/Sample", 
                $"{name}.cs", 
                19,
                $"Sample.PerfStructSize.M3()");

            p.Start(cfg);
        }
    }
}  