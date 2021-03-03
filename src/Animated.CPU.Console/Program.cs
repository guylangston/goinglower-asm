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
            var name = "BasicOps";
            cfg.WithProjectDir(
                "/home/guy/repo/cpu.anim/src/Sample", 
                $"{name}.cs", 
                10,
                $"Sample.BasicOps.Maths(Int32, Int32)");

            p.Start(cfg);
        }
    }
}  