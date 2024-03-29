using System;
using Gdk;
using GoingLower.Core;
using GoingLower.Core.Primitives;
using GoingLower.CPU.Animation;
using GoingLower.CPU.Model;
using Gtk;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.Gtk;
using Device=Gdk.Device;
using UI = Gtk.Builder.ObjectAttribute;
using Window=Gtk.Window;

namespace GoingLower.UI.GTK
{
    public class MainWindow : Window
    {
        private SKDrawingArea skiaView;
        private ISceneMaster master;
        private uint timerId;
        private TimeSpan interval;
        private (double X, double Y) last;
        private string[] appArgs;

        public MainWindow(string[] args)
            : this(args, new Builder("MainWindow.glade"))
        {
            
        }

        private MainWindow(string[] args, Builder builder)
            : base(builder.GetObject("MainWindow").Handle)
        {
            this.appArgs     = args;
            
            this.AcceptFocus = true;
            
            builder.Autoconnect(this);
            
            skiaView               = new SKDrawingArea();
            skiaView.CanFocus      = false;
            skiaView.WidthRequest  = 1920;
            skiaView.HeightRequest = 1080;

            var region = new DBlock(0, 0, skiaView.WidthRequest, skiaView.HeightRequest)
                .Set(20, 0, 0);
            
            master = new GtkPresentationSceneMaster(appArgs,(cmd, arg) => {
                if (cmd == "quit") Application.Quit();
            });
            master.Init(new SKRect(0,0, skiaView.WidthRequest, skiaView.HeightRequest));
            
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
        
        [GLib.ConnectBefore]
        private void OnButtonPressEvent(object o, ButtonPressEventArgs args)
        {
            if (last == (args.Event.X, args.Event.Y)) return; // HACK: Dub click events
            
            if (args.Event.Type == EventType.ButtonPress)
            {
                master.HandleMousePress(args.Event.Button, (float)args.Event.X, (float)args.Event.Y, args);
                last = (args.Event.X, args.Event.Y);
            }
            
        }

        [GLib.ConnectBefore]        // https://stackoverflow.com/questions/35833643/how-to-manually-detect-arrow-keys-in-gtk-c-sharp
        private void KeyPress(object o, KeyPressEventArgs args)
        {
            master.HandleKeyPress(args.Event.Key.ToString(), args.Event);
        }
        
        private bool OnUpdateTimer()
        {
            master.Step(interval);
            
            this.QueueDraw();

            return true;
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            master.Draw(e.Surface);
        }

        private void OnWindowDeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
        }

        //########[ Experimental ]#######################################################
        private void OnShow(object? sender, EventArgs e)
        {
            //timerId = GLib.Timeout.Add((uint)interval.Ticks, OnUpdateTimer);
        }


        private void OnMotion(object o, MotionNotifyEventArgs args)
        {
            master.HandleMotion((float)args.Event.X, (float)args.Event.Y, args);
            // Seems to only report drag events
            // scene.ProcessEvent("DebugText", $"Motion: {args.Event.X}, {args.Event.Y}", args);
        }

        
    }
}
