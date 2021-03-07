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
            var name = "Introduction";
            cfg.WithProjectDir(
                "/home/guy/repo/cpu.anim/src/Sample", 
                $"{name}.cs", 
                null,
                $"Sample.Introduction.ForLoop");

            p.Start(cfg);
        }
    }
}  