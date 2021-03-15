using System;
using Animated.CPU.Animation;
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
    public class MainWindow : Window
    {
        private SKDrawingArea skiaView;
        private SceneBase scene;
        private uint timerId;
        private TimeSpan interval;
        private (double X, double Y) last;
        
        public  DBlock region;

        public MainWindow()
            : this(new Builder("MainWindow.glade"))
        {
            
        }

        private MainWindow(Builder builder)
            : base(builder.GetObject("MainWindow").Handle)
        {
            this.AcceptFocus = true;
            
            builder.Autoconnect(this);
            
            skiaView               = new SKDrawingArea();
            skiaView.CanFocus      = false;
            skiaView.WidthRequest  = 1920;
            skiaView.HeightRequest = 1080;

            region = new DBlock(0, 0, skiaView.WidthRequest, skiaView.HeightRequest)
                .Set(20, 0, 0);

            SetScene(null);
            
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

            Init.MainWindow = this;

        }
        
        public void SetScene(string name)
        {
            scene = Init.BuildScene(name, region);
        }

        private void OnMotion(object o, MotionNotifyEventArgs args)
        {
            // Seems to only report drag events
            scene.ProcessEvent(args, "DebugText", $"Motion: {args.Event.X}, {args.Event.Y}");
        }

        private void OnButtonPressEvent(object o, ButtonPressEventArgs args)
        {
            if (last == (args.Event.X, args.Event.Y)) return; // HACK: Dub click events
            
            if (args.Event.Button == 1 && args.Event.Type == EventType.ButtonPress)
            {
                scene.MousePress(args.Event.Button, args.Event.X, args.Event.Y, args);
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
