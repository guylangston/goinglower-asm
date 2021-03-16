using System;
using Animated.CPU.Animation;
using Animated.CPU.Backend.LLDB;
using Animated.CPU.Model;
using Gtk;

namespace Animated.CPU.GTK
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
            var cfg = new Setup.Config()
            {
                StoryId           = "Introduction-ForLoop",
                BaseFolder        = "/home/guy/repo/cpu.anim/src/Sample/Scripts/Introduction-ForLoop",
                CompileBaseFolder = "/home/guy/repo/cpu.anim/src/Sample/Scripts"
            };
            setup.InitCpuFromDisk(cfg, cpu);
            return cpu;
        }
    }
}