using System;
using Gtk;
using SkiaSharp;

namespace GoingLower.UI.GTK
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            
            
            Application.Init();

            var app = new Application("org.goinglower.ui.gtk", GLib.ApplicationFlags.None);
            app.Register(GLib.Cancellable.Current);

            var win = new MainWindow(args);
            app.AddWindow(win);

            win.Show();
            Application.Run();
        }
    }
}
