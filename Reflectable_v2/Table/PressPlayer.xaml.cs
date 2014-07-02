using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Reflectable_v2;
using System.Windows.Threading;
using System.Threading;
using System.Diagnostics;

namespace Table
{
    /// <summary>
    /// Interaction logic for PressPlayer.xaml
    /// </summary>
    public partial class PressPlayer : UserControl
    {
        public Press Press
        {
            get { return (Press)GetValue(PressProperty); }
            set { SetValue(PressProperty, value); }
        }

        public static readonly DependencyProperty PressProperty =
            DependencyProperty.Register("Press", typeof(Press), typeof(PressPlayer), new UIPropertyMetadata(null));

        public string VideoFile
        {
            get { return (string)GetValue(VideoFileProperty); }
            set { SetValue(VideoFileProperty, value); }
        }

        public static readonly DependencyProperty VideoFileProperty =
            DependencyProperty.Register("VideoFile", typeof(string), typeof(PressPlayer), new UIPropertyMetadata("", new PropertyChangedCallback(OnVideoFileChanged)));

        public PressPlayer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.25);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();

            InitializeComponent();
        }

        private static void OnVideoFileChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PressPlayer p = (PressPlayer)sender;
            p.Player.Source = new Uri(p.VideoFile, UriKind.Relative);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if(Player.Position >= Press.End)
            {
                Player.Pause();
                Player.Position = Press.Start;
                PlayButton.Visibility = Visibility.Visible;
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            PlayButton.Visibility = Visibility.Collapsed;
            Player.Position = Press.Start;
            Player.Play();
        }

        private void Player_MediaOpened(object sender, RoutedEventArgs e)
        {
        }
    }
}
