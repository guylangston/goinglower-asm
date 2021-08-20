using System;
using System.IO;
using GoingLower.Backend.LLDB;
using GoingLower.CPU.Model;

namespace GoingLower.Console
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "new" && args[1] == "digest")
            {
                var dig = new StoryDigest()
                {
                    Id         = "NewId",
                    Title      = "Unnamed",
                    DescText   = "# Unnamed",
                    DescFormat = "text/markdown",
                    Arch       = Environment.MachineName,// Should be CPU Id
                    Version    = 1,
                    Created    = DateTime.UtcNow,
                    Modified   = DateTime.UtcNow,
                    
                    Author     = "Guy Langston",
                    License    = "https://creativecommons.org/licenses/by/4.0/",
                    ProjectUrl = "https://www.goinglower.dev",
                    AuthorUrl  = "https://www.guylangston.net",
                };
                var repo           = new StoryDigestRepo();
                var file = "unnamed.sty-json";
                repo.Write(dig, file);
                System.Console.WriteLine($"Wrote: {file}");
                
                System.Console.WriteLine(File.ReadAllText(file));
                
                
                return 0;
            }
            

            if (args.Length > 0 && args[0] == "capture")
            {
                var p    = new DebuggerDriver();
                var cfg  = new DebuggerDriver.ConfigArgs();
                
                cfg.WithProjectDir(
                    "/home/guy/repo/cpu.anim/src/GoingLower.Samples", 
                    $"Scripts/PrimeSieve/PrimeSieve.cs", 
                    null,
                    $"GoingLower.Samples.Scripts.PrimeSieve");

                cfg.StepCount = 100;

                p.Start(cfg);
                return 0;
            }


            System.Console.Error.WriteLine("Bad args");
            return -1;
        }
    }
}  