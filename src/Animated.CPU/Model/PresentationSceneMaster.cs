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
        private SKPaint cutGuid;
        public Action<string, object> HandleExternalCommand { get; }

        public PresentationSceneMaster(Action<string, object> handleExternalCommand) : base(new []
        {
            "Intro",
            "Layers",
            "Execute",
            "Outro",
            //"Slides", 
        })
        {
            HandleExternalCommand = handleExternalCommand;
            cutGuid = new SKPaint()
            {
                Style       = SKPaintStyle.Stroke,
                StrokeWidth = 1,
                Color       = SKColor.Parse("#383838")
            };
        }
        
        public const string Version = "0.7-alpha";
        
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
                "execute" => new SceneExecute(this, cpu, dBlock),
                "slides" => new SlidesScene("Slides", styles, dBlock)
                {
                    Model = cpu.Story
                },
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

        private IScene? last;
        public override void HandleKeyPress(string key, object native)
        {
            if (key == "q")
            {
                HandleExternalCommand("quit", null);
            }
            if (key == "s")
            {
                if (CurrentScene?.Name == "Slides")
                {
                    CurrentScene = last;
                }
                else
                {
                    last         = CurrentScene;
                    CurrentScene = this.GetScene("Slides");
                }
                return;
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
            
            surface.Canvas.DrawRect(size, cutGuid);
            
            base.Draw(surface);
            
            surface.Canvas.DrawText($"{CurrentIndex+1}/{this.GetAllScenes().Count}: {CurrentScene?.Name}", 10, 20, styles.TextH1);
        }
    }
}