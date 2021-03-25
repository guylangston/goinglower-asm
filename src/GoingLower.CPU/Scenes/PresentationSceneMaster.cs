using System;
using System.Collections.Generic;
using System.Linq;
using GoingLower.Core;
using GoingLower.Core.Controllers;
using GoingLower.Core.Helpers;
using GoingLower.Core.Primitives;
using GoingLower.CPU.Animation;
using GoingLower.CPU.Model;
using GoingLower.CPU.Scenes;
using SkiaSharp;

namespace GoingLower.CPU.Scenes
{
    public abstract class PresentationSceneMaster : SceneMasterBase
    {
        private StyleFactory styles;
        private SKRect size;
        private Cpu cpu;
        private SKPaint cutGuid;
        public Action<string, object> HandleExternalCommand { get; }
        public TimeSpan                 MouseDrawTimeOut      { get; set; }

        public PresentationSceneMaster(Action<string, object> handleExternalCommand) : base(new []
        {
            "Intro",
            "Layers",
            "Execute",
            "Outro",
            //"Slides", 
            //"MindMap"
        })
        {
            MouseDrawTimeOut      = TimeSpan.FromSeconds(3);
            HandleExternalCommand = handleExternalCommand;
            cutGuid = new SKPaint()
            {
                Style       = SKPaintStyle.Stroke,
                StrokeWidth = 1,
                Color       = SKColor.Parse("#383838")
            };
        }
        
        
        
        // May be a live debugger or 'cooked' on disk... External libs
        protected abstract Cpu BuildCpu();

        public override void Init(SKRect viewSize)
        {
            this.size = viewSize;
            styles    = new StyleFactory();

            cpu = BuildCpu();

            CurrentScene = SceneFactory("MindMap"); // "Intro");
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
                "outro" => new OutroScene("Outro", styles,  dBlock, cpu.Story.Outro)
                {
                    
                },
                "mindmap" => new MindMapScene("MindMap", styles,  dBlock),
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


        public struct PointInTime
        {
            public SKPoint  Point { get; set; }
            public DateTime At    { get; set; }
        }

        private List<PointInTime> mouse = new List<PointInTime>();
        
        public override void HandleMousePress(uint button, float x, float y, object native)
        {
            base.HandleMousePress(button, x, y, native);
            mouse.Add(new PointInTime()
            {
                Point = new SKPoint(-1, -1),
                At    =  DateTime.Now
            });
        }

        public override void HandleMotion(float x, float y, object args)
        {
            mouse.Add(new PointInTime()
            {
                Point = new SKPoint(x, y),
                At    =  DateTime.Now
            });
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

        public override void Step(TimeSpan step)
        {
            base.Step(step);
            
            var n = DateTime.Now.Subtract(MouseDrawTimeOut);
            mouse.RemoveAll(x => x.At < n);
        }

        

        public override void Draw(SKSurface surface)
        {
            surface.Canvas.Clear(styles.bg);

            surface.Canvas.DrawRect(size, cutGuid);

            base.Draw(surface);

            if (CurrentIndex >= 0)
            {
                surface.Canvas.DrawText($"{CurrentIndex + 1}/{this.GetAllScenes().Count}: {CurrentScene?.Name}", 10, 20, styles.TextH1);
            }
            else
            {
                surface.Canvas.DrawText(CurrentScene?.Name, 10, 20, styles.TextH1);
            }

            if (mouse.Any())
            {
                var mPath = new SKPath();
                var move  = true;
                foreach (var pp in mouse)
                {
                    if (pp.Point.X == -1 && pp.Point.Y == -1)
                    {
                        move = true;
                        continue;
                    }
                    if (move)
                    {
                        mPath.MoveTo(pp.Point);
                    }
                    else
                    {
                        mPath.LineTo(pp.Point);
                    }
                    move = false;
                }
                
                // Shadow
                surface.Canvas.Save();
                surface.Canvas.Translate(styles.Annotate.StrokeWidth, styles.Annotate.StrokeWidth);
                surface.Canvas.DrawPath(mPath, styles.Annotate2);
                surface.Canvas.Restore();
                
                surface.Canvas.DrawPath(mPath, styles.Annotate);
                
            }

            

        }
    }
}