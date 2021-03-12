using Animated.CPU.Animation;
using Animated.CPU.Backend.LLDB;
using Animated.CPU.Model;
using Gtk;

namespace Animated.CPU.GTK
{
    public static class Init
    {
        public static SceneBase BuildScene(DBlock region)
        {
            var setup = new Setup();
            var cpu   = new Cpu();
            var cfg = new Setup.Config()
            {
                StoryId           = "Introduction-ForLoop",
                BaseFolder        = "/home/guy/repo/cpu.anim/src/Sample/Scripts/Introduction-ForLoop",
                CompileBaseFolder = "/home/guy/repo/cpu.anim/src/Sample/Scripts"
            };
            setup.InitCpuFromDisk(cfg, cpu);
            
            var scene = new SceneExecute(region)
            {
                Model = cpu,
                SendCommand = (cmd, obj) => {
                    if (cmd == "QUIT") Application.Quit();
                }
            };
            return scene;
        }
    }
}