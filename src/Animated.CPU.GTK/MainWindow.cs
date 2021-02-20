using System;
using Animated.CPU.Animation;
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
        private IScene scene;
        private uint timerId;
        private TimeSpan interval; 

        public MainWindow()
            : this(new Builder("MainWindow.glade"))
        {
        }

        private MainWindow(Builder builder)
            : base(builder.GetObject("MainWindow").Handle)
        {
            builder.Autoconnect(this);
            DeleteEvent        += OnWindowDeleteEvent;
            
            this.KeyPressEvent += KeyPress;
            
            skiaView              =  new SKDrawingArea();
            skiaView.WidthRequest =  1960;
            skiaView.HeightRequest =  1080;
            skiaView.PaintSurface += OnPaintSurface;
            skiaView.Shown        += OnShow;
            
            
            skiaView.Show();
            Child = skiaView;


            scene = new Scene();
            
            interval = TimeSpan.FromSeconds(1/60f);

            if (timerId == 0)
            {
                timerId = GLib.Timeout.Add((uint)interval.TotalMilliseconds, OnUpdateTimer);    
            }
            
        }
        private void KeyPress(object o, KeyPressEventArgs args)
        {
            scene?.Step(interval);
            skiaView.QueueDraw();
        }
        private void OnShow(object? sender, EventArgs e)
        {
            //timerId = GLib.Timeout.Add((uint)interval.Ticks, OnUpdateTimer);
        }
        
        private bool OnUpdateTimer()
        {
            scene?.Step(interval);
            this.QueueDraw();

            return true;
        }

        private void OnWindowDeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            scene?.Draw(e.Surface);
           
            
            // the the canvas and properties
            // var canvas = e.Surface.Canvas;
            // Animated.CPU.Example.Draw(canvas);


            
            //
            // // draw some text
            // var paint = new SKPaint
            // {
            //     Color       = SKColors.Black,
            //     IsAntialias = true,
            //     Style       = SKPaintStyle.Fill,
            //     TextAlign   = SKTextAlign.Center,
            //     TextSize    = 24
            // };
            // var coord = new SKPoint(scaledSize.Width / 2, (scaledSize.Height + paint.TextSize) / 2);
            // canvas.DrawText("SkiaSharp", coord, paint);
        }
    }
}
