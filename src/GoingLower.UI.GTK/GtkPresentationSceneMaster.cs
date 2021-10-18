using System;
using System.IO;
using GoingLower.Backend.LLDB;
using GoingLower.Core.Helpers;
using GoingLower.CPU.Elements;
using GoingLower.CPU.Model;
using GoingLower.CPU.Scenes;
using Gtk;

namespace GoingLower.UI.GTK
{
    public  class GtkPresentationSceneMaster : PresentationSceneMaster
    {
        private readonly string[] args;
        private string digestFile;

        public GtkPresentationSceneMaster(string[] args, Action<string, object> handleExternalCommand) : base(handleExternalCommand)
        {
            this.args = args;

            if (args.Length > 0)
            {
                if (File.Exists(args[0]))
                {
                    var fname  = Path.GetFileName(args[0]);
                    var fDir   = Path.GetDirectoryName(args[0]);
                    this.digestFile = args[0];
                    return;
                }
            }
            
            // var paths = new PathScanner(new[]
            // {
            //     "/home/guy/repo/cpu.anim",
            //     "c:\\projects\\cpu.anim",
            //     "c:\\repo\\cpu.anim"
            // });

            // if (paths.TryScanFirstDirectoryExists(out var dir))
            // {
            //     var any = 
            // }

            throw new Exception("Cannot found digest (.sty-json)");
        }

        protected override Cpu BuildCpu()
        {
            var setup = new Setup();
            var cpu = new Cpu();


            var dRepo   = new StoryDigestRepo();
            var digest = dRepo.Load(digestFile);
            
            
            var cfg = new Setup.Config()
            {
                Digest            = digest,
                StoryId           = digest.Id,
                BaseFolder        = Path.GetDirectoryName(digestFile),
                CompileBaseFolder = digest.CompileRoot,
                MainSourceFile = digest.MainSourceFile
            };
            setup.InitCpuFromDisk(cfg, cpu);
            return cpu;
        }
        
        
    }
}