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

namespace Tablet
{
    /// <summary>
    /// Interaction logic for PanopticonPlayer.xaml
    /// </summary>
    public partial class PanopticonPlayer : UserControl
    {
        public delegate void AnnotationMadeEventHandler(object sender, Tablet.PopupPlayer.AnnotationMadeEventArgs e);
        public event AnnotationMadeEventHandler AnnotationMade;

        public event RoutedEventHandler VideoLoaded
        {
            add { AddHandler(VideoLoadedEvent, value); }
            remove { RemoveHandler(VideoLoadedEvent, value); }
        }

        public static readonly RoutedEvent VideoLoadedEvent =
            EventManager.RegisterRoutedEvent("VideoLoaded", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PanopticonPlayer));

        public List<Press> Presses
        {
            get { return (List<Press>)GetValue(PressesProperty); }
            set { SetValue(PressesProperty, value); }
        }

        public static readonly DependencyProperty PressesProperty =
            DependencyProperty.Register("Presses", typeof(List<Press>), typeof(PanopticonPlayer), new UIPropertyMetadata(new List<Press>()));

        public Color UserColor
        {
            get { return (Color)GetValue(UserColorProperty); }
            set { SetValue(UserColorProperty, value); }
        }

        public static readonly DependencyProperty UserColorProperty =
            DependencyProperty.Register("UserColor", typeof(Color), typeof(PanopticonPlayer), new UIPropertyMetadata(Colors.White));

        public Mode PlayerMode
        {
            get { return (Mode)GetValue(PlayerModeProperty); }
            set { SetValue(PlayerModeProperty, value); }
        }

        public static readonly DependencyProperty PlayerModeProperty =
            DependencyProperty.Register("PlayerMode", typeof(Mode), typeof(PanopticonPlayer), new UIPropertyMetadata(Mode.OVERVIEW, new PropertyChangedCallback(OnPlayerModeChanged)));

        public string VideoFile
        {
            get { return (string)GetValue(VideoFileProperty); }
            set { SetValue(VideoFileProperty, value); }
        }

        public static readonly DependencyProperty VideoFileProperty =
            DependencyProperty.Register("VideoFile", typeof(string), typeof(PanopticonPlayer), new UIPropertyMetadata("", new PropertyChangedCallback(OnVideoFileChanged)));

        public string PanopticonFile
        {
            get { return (string)GetValue(PanopticonFileProperty); }
            set { SetValue(PanopticonFileProperty, value); }
        }

        public static readonly DependencyProperty PanopticonFileProperty =
            DependencyProperty.Register("PanopticonFile", typeof(string), typeof(PanopticonPlayer), new UIPropertyMetadata("", new PropertyChangedCallback(OnPanopticonFileChanged)));

        public PanopticonInfo PanopticonVideoInfo
        {
            get { return (PanopticonInfo)GetValue(PanopticonVideoInfoProperty); }
            set { SetValue(PanopticonVideoInfoProperty, value); }
        }

        public static readonly DependencyProperty PanopticonVideoInfoProperty =
            DependencyProperty.Register("PanopticonVideoInfo", typeof(PanopticonInfo), typeof(PanopticonPlayer), new UIPropertyMetadata(new PanopticonInfo(), new PropertyChangedCallback(OnPanopticonInfoChanged)));

        public TimeSpan StudyLength
        {
            get { return (TimeSpan)GetValue(StudyLengthProperty); }
            set { SetValue(StudyLengthProperty, value); }
        }

        public static readonly DependencyProperty StudyLengthProperty =
            DependencyProperty.Register("StudyLength", typeof(TimeSpan), typeof(PanopticonPlayer), new UIPropertyMetadata(TimeSpan.FromSeconds(0)));

        public enum Mode
        {
            OVERVIEW,
            DIVERGENCE
        };

        private const double BORDER_SIZE = 0.045;
        private const double GAP_MODIFIER = 0.85;

        private bool popupOpen;
        private List<AnnotationControl> annotationControls;

        public PanopticonPlayer()
        {
            InitializeComponent();

            popupOpen = false;
            annotationControls = new List<AnnotationControl>();

            PopupPlayer.AnnotationMade += new Tablet.PopupPlayer.AnnotationMadeEventHandler(PopupPlayer_AnnotationMade);
        }

        private void PopupPlayer_AnnotationMade(object sender, Tablet.PopupPlayer.AnnotationMadeEventArgs e)
        {
            PopupPlayer.Visibility = Visibility.Hidden;
            popupOpen = false;

            if (AnnotationMade != null)
            {
                AnnotationMade(this, e);
            }
        }

        private void VideoPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            GenerateOverlay();
            RaiseEvent(new RoutedEventArgs(PanopticonPlayer.VideoLoadedEvent, this));
        }

        private void PopupPlayer_Closed(object sender)
        {
            popupOpen = false;
            PopupPlayer.Visibility = Visibility.Collapsed;
        }

        private static void OnPanopticonFileChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PanopticonPlayer p = (PanopticonPlayer)sender;
            p.VideoPlayer.Source = new Uri(p.PanopticonFile, UriKind.Relative);
        }

        private static void OnVideoFileChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PanopticonPlayer p = (PanopticonPlayer)sender;
            p.PopupPlayer.VideoFile = p.VideoFile;
        }

        private static void OnPanopticonInfoChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PanopticonPlayer p = (PanopticonPlayer)sender;
            p.GenerateOverlay();
        }

        private static void OnPlayerModeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PanopticonPlayer p = (PanopticonPlayer)sender;
            p.PopupPlayer.PlayerMode = p.PlayerMode;
        }

        private void GenerateOverlay()
        {
            int numCells = PanopticonVideoInfo.GridHeight * PanopticonVideoInfo.GridWidth;
            int studyLengthMs = (int)StudyLength.TotalMilliseconds;
            int rowLengthMs = (int)(studyLengthMs / numCells) * PanopticonVideoInfo.GridWidth;

            double canvasH = OverlayCanvas.ActualHeight;
            double canvasW = canvasH * 1.3333;
            // HACK!

            double cw = (canvasW / (double)(PanopticonVideoInfo.GridWidth + 1)) * (double)PanopticonVideoInfo.GridWidth;
            double ch = canvasH;
            double xOffset = (canvasW / (double)(PanopticonVideoInfo.GridWidth + 1)) / 2.0;

            foreach (Press p in Presses)
            {
                int length = (int)(p.End - p.Start).TotalMilliseconds;
                int startPosMs = (int)p.Start.TotalMilliseconds;
                int posMs = startPosMs + (length / 2);
   
                double posX = (double)(posMs % rowLengthMs) / (double)rowLengthMs;
                double posY = (double)(posMs / rowLengthMs) / (double)PanopticonVideoInfo.GridHeight;

                AnnotationControl ac = new AnnotationControl();
                ac.UserColor = p.User.Color.HasValue ? p.User.Color.Value : Colors.White;
                ac.AnnotationPress = p;
                ac.Height = ch / (double)PanopticonVideoInfo.GridHeight;
                ac.PressSelected += new RoutedEventHandler(ac_PressSelected);
                annotationControls.Add(ac);

                Canvas.SetLeft(ac, (posX * cw) + xOffset);
                Canvas.SetTop(ac, posY * ch);
                OverlayCanvas.Children.Add(ac);
            }
        }

        void ac_PressSelected(object sender, RoutedEventArgs e)
        {
            if (!popupOpen)
            {
                AnnotationControl ac = (AnnotationControl)sender;

                PopupPlayer.Visibility = Visibility.Visible;
                PopupPlayer.AnnotationPress = ac.AnnotationPress;
                popupOpen = true;
            }
        }

        void VideoPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            VideoPlayer.Position = TimeSpan.Zero;
        }

        private void PopupPlayer_Closed(object sender, RoutedEventArgs e)
        {
            PopupPlayer.Visibility = Visibility.Hidden;
            popupOpen = false;
        }
    }
}
