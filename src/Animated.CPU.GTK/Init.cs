using Animated.CPU.Animation;
using Animated.CPU.Backend.LLDB;
using Animated.CPU.Model;
using Gtk;

namespace Animated.CPU.GTK
{
    public static class Init
    {
        private static SceneExecute execute;
        public static Cpu cpu;
        public static MainWindow MainWindow { get; set; }
        
        
        public static SceneBase BuildScene(string name, DBlock region)
        {
            if (cpu == null)
            {
                var setup = new Setup();
                cpu   = new Cpu();
                var cfg = new Setup.Config()
                {
                    StoryId           = "Introduction-ForLoop",
                    BaseFolder        = "/home/guy/repo/cpu.anim/src/Sample/Scripts/Introduction-ForLoop",
                    CompileBaseFolder = "/home/guy/repo/cpu.anim/src/Sample/Scripts"
                };
                setup.InitCpuFromDisk(cfg, cpu);
            }
            
            switch (name)
            {
                case "Execute":
                    if (execute != null) return execute;
                    
                    execute = new SceneExecute(region)
                    {
                        Model           = cpu,
                        SendHostCommand = SendCommand 
                    };
                    return execute;
                
                case "Layers":
                default:
                    var layers = new SceneLayers(new StyleFactory(), region)
                    {
                        Model           = cpu,
                        SendHostCommand = SendCommand 
                    };
                    return layers;
            }
            
            
            
           
        }
        
        

        private static void SendCommand(string cmd, object obj)
        {
            if (cmd == "QUIT") Application.Quit();
            
            if (cmd == "Scene")
            {
                MainWindow.SetScene(obj.ToString());
            }
        }
    }
}