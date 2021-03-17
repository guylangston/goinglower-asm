using System;
using System.Linq;
using Animated.CPU.Animation;
using SkiaSharp;

namespace Animated.CPU.Model
{
    public abstract class PresentationSceneMaster : SceneMasterBase
    {
        private StyleFactory styles;
        private SKRect size;
        private Cpu cpu;
        public Action<string, object> HandleExternalCommand { get; }

        public PresentationSceneMaster(Action<string, object> handleExternalCommand) : base(new []
        {
            "Intro",
            "Layers",
            "Debugger",
            "Outro"
        })
        {
            HandleExternalCommand = handleExternalCommand;
        }
        
        // May be a live debugger or 'cooked' on disk... External libs
        protected abstract Cpu BuildCpu();

        public override void Init(SKRect viewSize)
        {
            this.size = viewSize;
            styles    = new StyleFactory();

            cpu = BuildCpu();
            
            CurrentScene = SceneFactory("Intro");
        }

        public override IScene? SceneFactory(string name)
        {
            name = name.ToLowerInvariant();
            var dBlock = new DBlock(0,0, size.Width, size.Height)
                .Set(30,0, 0);
            return name switch
            {
                "intro" => new IntroScene("Intro", styles,  dBlock),
                "layers" => new SceneLayers(this, cpu, styles, dBlock),
                "debugger" => new SceneExecute(this, cpu, dBlock),
                "outro" => new TextScene("Outro", styles,  dBlock, cpu.Story.Outro),
                _ => throw new Exception($"Not Found: {name}")
            };
        }

        public override IScene? NextScene(SceneSeqArg complete)
        {
            var all = GetAllScenes();
            if (complete.Seq == SceneSeq.Beginning)
            {
                return GetScene(all.First());
            }
            
            if (complete.Seq == SceneSeq.End)
            {
                return GetScene(all.Last());
            }

            if (complete.Seq == SceneSeq.Next)
            {
                if (CurrentScene == null) return GetScene(all.First());
                var currIdx = all.IndexOf(CurrentScene.Name);
                if (currIdx < all.Count-1)
                {
                    return GetScene(all[currIdx+1]);
                }
                return CurrentScene;
            }
            
            if (complete.Seq == SceneSeq.Prior)
            {
                if (CurrentScene == null) return GetScene(all.Last());
                var currIdx = all.IndexOf(CurrentScene.Name);
                if (currIdx > 0)
                {
                    return GetScene(all[currIdx-1]);
                }
                return CurrentScene;
            }

            throw new NotSupportedException();
        }

        public override void HandleKeyPress(string key, object native)
        {
            if (key == "q")
            {
                HandleExternalCommand("quit", null);
            }
            if (key == "comma")
            {
                CurrentScene = this.NextScene(new SceneSeqArg(SceneSeq.Prior));
                return;
            }
            if (key == "period")
            {
                CurrentScene = this.NextScene(new SceneSeqArg(SceneSeq.Next));
                return;
            }
            base.HandleKeyPress(key, native);
        }

        public override void HandleCommand(string cmd, object obj)
        {
            var ccmd = cmd.ToLowerInvariant().Trim();
            if (ccmd == "quit") HandleExternalCommand(cmd, obj);
        }

        public override void Draw(SKSurface surface)
        {
            surface.Canvas.Clear(styles.bg);
            
            base.Draw(surface);
            
            surface.Canvas.DrawText($"{CurrentIndex+1}/{this.GetAllScenes().Count}: {CurrentScene?.Name}", 10, 20, styles.TextH1);
        }
    }
}