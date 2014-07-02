using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.IO;
using System.Collections;
using Reflectable_v2;

namespace Table
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
            : base()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            MessageBox.Show(ex.Message + "\r\n" +
                            ex.StackTrace);
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message + "\r\n" +
                            e.Exception.StackTrace);
        }

        public static void Restart()
        {
            Window w = new MainWindow();
            w.Show();

            App.Current.MainWindow.Close();
            App.Current.MainWindow = w;

#if DEBUG
            w.WindowState = WindowState.Normal;
            w.WindowStyle = WindowStyle.ThreeDBorderWindow;
#else
            w.WindowStyle = WindowStyle.None;
            w.WindowState = WindowState.Maximized;
            w.ResizeMode = ResizeMode.NoResize;
            w.Topmost = true;
#endif
        }
    }
}
