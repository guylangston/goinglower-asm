using System;
using GoingLower.Backend.LLDB;

namespace GoingLower.Console
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