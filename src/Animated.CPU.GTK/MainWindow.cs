using System;
using Animated.CPU.Animation;
using Animated.CPU.Backend.LLDB;
using Animated.CPU.Model;
using Gdk;
using Gtk;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.Gtk;
using Device=Gdk.Device;
using UI = Gtk.Builder.ObjectAttribute;
using Window=Gtk.Window;

namespace Animated.CPU.GTK
{
    class MainWindow : Window
    {
        private SKDrawingArea skiaView;
        private Scene scene;
        private uint timerId;
        private TimeSpan interval;
        private (double X, double Y) last;

        public MainWindow()
            : this(new Builder("MainWindow.glade"))
        {
            
        }

        private MainWindow(Builder builder)
            : base(builder.GetObject("MainWindow").Handle)
        {
            var setup = new Setup();
            var cpu   = new Cpu();
            var cfg   = new Setup.Config()
            {
                StoryId = "Introduction-ForLoop",
                BaseFolder = "/home/guy/repo/cpu.anim/src/Sample/Scripts/Introduction-ForLoop",
                CompileBaseFolder = "/home/guy/repo/cpu.anim/src/Sample/Scripts"
            };
            setup.InitCpuFromDisk(cfg, cpu);
            
            builder.Autoconnect(this);
            
            skiaView              =  new SKDrawingArea();
            skiaView.WidthRequest =  1920;
            skiaView.HeightRequest =  1080;

            var region = new DBlock(0, 0, skiaView.WidthRequest, skiaView.HeightRequest)
                .Set(20, 0, 0);
            
            scene = new Scene(region)
            {
                Model = cpu,
                SendCommand = (cmd, obj) => {
                    if (cmd == "QUIT") Application.Quit();
                }
            };
            
            //  Window Events
            this.DeleteEvent       += OnWindowDeleteEvent;
            this.KeyPressEvent     += KeyPress;
            this.ButtonPressEvent  += OnButtonPressEvent;
            this.MotionNotifyEvent += OnMotion;
            
            // Paint Events
            skiaView.PaintSurface += OnPaintSurface;
            skiaView.Shown        += OnShow;

            skiaView.Show();
            Child = skiaView;
            
            interval = TimeSpan.FromSeconds(1/60f);
            if (timerId == 0)
            {
                timerId = GLib.Timeout.Add((uint)interval.TotalMilliseconds, OnUpdateTimer);    
            }
            
        }

        private void OnMotion(object o, MotionNotifyEventArgs args)
        {
            // Seems to only report drag events
            scene.DebugText = $"Motion: {args.Event.X}, {args.Event.Y}";
        }

        private void OnButtonPressEvent(object o, ButtonPressEventArgs args)
        {
            if (last == (args.Event.X, args.Event.Y)) return; // HACK: Dub click events
            
            if (args.Event.Button == 1 && args.Event.Type == EventType.ButtonPress)
            {
                scene.ButtonPress(args.Event.Button, args.Event.X, args.Event.Y, args);
                last = (args.Event.X, args.Event.Y);
            }
            
        }

        private void KeyPress(object o, KeyPressEventArgs args)
        {
            scene.KeyPress(args.Event, args.Event.Key.ToString());
        }
        
        private void OnShow(object? sender, EventArgs e)
        {
            //timerId = GLib.Timeout.Add((uint)interval.Ticks, OnUpdateTimer);
        }
        
        private bool OnUpdateTimer()
        {
            scene?.StepExec(interval);
            
            this.QueueDraw();

            return true;
        }

        private void OnWindowDeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            scene?.DrawExec(new DrawContext(scene, e.Surface.Canvas));
        }
    }
}
