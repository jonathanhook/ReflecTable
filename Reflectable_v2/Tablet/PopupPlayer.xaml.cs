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
using System.Timers;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Threading;
using Reflectable_v2;

namespace Tablet
{
    public partial class PopupPlayer : UserControl
    {
        public class AnnotationMadeEventArgs : EventArgs
        {
            public Press Press { get; private set; }
            public string Comment { get; private set; }
            public User User { get; private set; }

            public AnnotationMadeEventArgs(Press press, string comment, User user)
            {
                this.Press = press;
                this.Comment = comment;
                this.User = user;
            }
        }

        public delegate void AnnotationMadeEventHandler(object sender, AnnotationMadeEventArgs e);
        public event AnnotationMadeEventHandler AnnotationMade;

        public event RoutedEventHandler Closed
        {
            add { AddHandler(ClosedEventHandler, value); }
            remove { RemoveHandler(ClosedEventHandler, value); }
        }

        public static readonly RoutedEvent ClosedEventHandler =
            EventManager.RegisterRoutedEvent("Closed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PopupPlayer));

        public PanopticonPlayer.Mode PlayerMode
        {
            get { return (PanopticonPlayer.Mode)GetValue(PlayerModeProperty); }
            set { SetValue(PlayerModeProperty, value); }
        }

        public static readonly DependencyProperty PlayerModeProperty =
            DependencyProperty.Register("PlayerMode", typeof(PanopticonPlayer.Mode), typeof(PopupPlayer), new UIPropertyMetadata(PanopticonPlayer.Mode.OVERVIEW, new PropertyChangedCallback(OnPlayerModeChanged)));

        public Press AnnotationPress
        {
            get { return (Press)GetValue(AnnotationPressProperty); }
            set { SetValue(AnnotationPressProperty, value); }
        }

        public static readonly DependencyProperty AnnotationPressProperty =
            DependencyProperty.Register("AnnotationPress", typeof(Press), typeof(PopupPlayer), new UIPropertyMetadata(null, new PropertyChangedCallback(OnPressChanged)));

        public string VideoFile
        {
            get { return (string)GetValue(VideoFileProperty); }
            set { SetValue(VideoFileProperty, value); }
        }

        public static readonly DependencyProperty VideoFileProperty =
            DependencyProperty.Register("VideoFile", typeof(string), typeof(PopupPlayer), new UIPropertyMetadata("", new PropertyChangedCallback(OnVideoFileChanged)));

        private bool loaded;

        public PopupPlayer()
        {
            loaded = false;

            DispatcherTimer t = new DispatcherTimer();
            t.Tick += new EventHandler(t_Tick);
            t.Interval = TimeSpan.FromMilliseconds(500);
            t.Start();

            InitializeComponent();
        }

        private static void OnVideoFileChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PopupPlayer p = (PopupPlayer)sender;
            p.VideoPlayer.Source = new Uri(p.VideoFile, UriKind.Relative);
            p.loaded = true;
        }

        private static void OnPlayerModeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PopupPlayer p = (PopupPlayer)sender;

            if (p.PlayerMode == PanopticonPlayer.Mode.OVERVIEW)
            {
                p.CommentBlock.Visibility = Visibility.Collapsed;
                p.OKButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                p.CommentBlock.Visibility = Visibility.Visible;
                p.OKButton.Visibility = Visibility.Visible;
            }

        }

        private static void OnPressChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PopupPlayer p = (PopupPlayer)sender;
            p.VideoPlayer.Position = p.AnnotationPress.Start;
        }

        private void t_Tick(object sender, EventArgs e)
        {
            if (loaded && AnnotationPress != null)
            {
                TimeSpan currentposition = VideoPlayer.Position;
                if (currentposition >= AnnotationPress.End)
                {
                    VideoPlayer.Position = AnnotationPress.Start;
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(PopupPlayer.ClosedEventHandler, this));
            CommentBlock.Text = "";
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (CommentBlock.Text != "")
            {
                if (AnnotationMade != null)
                {
                    AnnotationMade(this, new AnnotationMadeEventArgs(AnnotationPress, CommentBlock.Text, new User(Properties.Settings.Default.UserId, null)));
                }

                CommentBlock.Text = "";
            }
        }

        private void VideoPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (loaded && AnnotationPress != null)
            {
                VideoPlayer.Position = AnnotationPress.Start;
            }
        }

        private void MainGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainGrid.Focus();
        }

        private void MainGrid_TouchDown(object sender, TouchEventArgs e)
        {
            MainGrid.Focus();
        }
    }
}
