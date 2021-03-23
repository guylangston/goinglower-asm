using System;
using System.IO;
using GoingLower.Backend.LLDB;
using GoingLower.CPU.Elements;
using GoingLower.CPU.Model;
using Gtk;

namespace GoingLower.UI.GTK
{
    public  class GtkPresentationSceneMaster : PresentationSceneMaster
    {
        
        public GtkPresentationSceneMaster(Action<string, object> handleExternalCommand) : base(handleExternalCommand)
        {
        }

        protected override Cpu BuildCpu()
        {
            var setup = new Setup();
            var cpu = new Cpu();

            var root = Directory.Exists("/home/guy/repo/cpu.anim") 
                ? "/home/guy/repo/cpu.anim"
                : Directory.Exists("c:\\projects\\cpu.anim") 
                    ? "c:\\projects\\cpu.anim"
                    : throw new Exception("Root Dir?");
            
            var cfg = new Setup.Config()
            {
                StoryId           = "Introduction-ForLoop",
                BaseFolder        = Path.Combine(root, "src/Sample/Scripts/Introduction-ForLoop"),
                CompileBaseFolder = Path.Combine(root, "src/Sample/Scripts")
            };
            setup.InitCpuFromDisk(cfg, cpu);
            return cpu;
        }
    }
}