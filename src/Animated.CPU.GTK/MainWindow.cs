using System;
using Animated.CPU.Animation;
using Animated.CPU.Backend.LLDB;
using Animated.CPU.Model;
using Gtk;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace Animated.CPU.GTK
{
    class MainWindow : Window
    {
        private SKDrawingArea skiaView;
        private Scene scene;
        private uint timerId;
        private TimeSpan interval; 

        public MainWindow()
            : this(new Builder("MainWindow.glade"))
        {
            
        }

        private MainWindow(Builder builder)
            : base(builder.GetObject("MainWindow").Handle)
        {
            var dir = "/home/guy/repo/cpu.anim/src/Sample/bin/Release/net5.0/linux-x64/publish";
            var src = System.IO.Path.Combine("/home/guy/repo/cpu.anim/src/Sample/", "BasicOps.cs");
            

            var sourceProvider = new SourceProvider();
            sourceProvider.Load(src);
            
            var parser = new Parser(sourceProvider);
            var setup  = new Setup();
            var cpu    = new Cpu();
            setup.InitFromDisk(dir, cpu, sourceProvider);
            
            builder.Autoconnect(this);
            DeleteEvent        += OnWindowDeleteEvent;
            
            this.KeyPressEvent += KeyPress;
            
            skiaView              =  new SKDrawingArea();
            skiaView.WidthRequest =  1960;
            skiaView.HeightRequest =  1080;

            var region = new DBlock(0, 0, skiaView.WidthRequest, skiaView.HeightRequest)
                .Set(50, 0, 0);
            
            scene = new Scene(region)
            {
                Model = cpu
            };
            
            skiaView.PaintSurface += OnPaintSurface;
            skiaView.Shown        += OnShow;
            
            this.ButtonPressEvent += OnButtonPressEvent;
            
            
            skiaView.Show();
            Child = skiaView;
            
            interval = TimeSpan.FromSeconds(1/60f);
            if (timerId == 0)
            {
                timerId = GLib.Timeout.Add((uint)interval.TotalMilliseconds, OnUpdateTimer);    
            }
            
        }

        private void OnButtonPressEvent(object o, ButtonPressEventArgs args)
        {
            scene.Mouse = new SKPoint((float)args.Event.X, (float)args.Event.Y);
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
            scene?.DrawExec(new DrawContext(e.Surface.Canvas));
        }
    }
}
