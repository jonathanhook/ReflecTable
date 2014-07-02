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
using System.Media;

namespace Tablet
{
    public partial class Stage2UI : UserControl
    {
        public enum PanopticonState
        {
            PROCESSING,
            DOWNLOADING,
            READY
        }

        public delegate void AnnotationMadeEventHandler(object sender, Tablet.PopupPlayer.AnnotationMadeEventArgs e);
        public event AnnotationMadeEventHandler AnnotationMade;

        public delegate void ResearchQuestionSubmittedEventHandler(object sender, string question);
        public event ResearchQuestionSubmittedEventHandler ResearchQuestionSubmitted;

        public event RoutedEventHandler StartRequested
        {
            add { AddHandler(StartRequestedEvent, value); }
            remove { RemoveHandler(StartRequestedEvent, value); }
        }

        public Color UserColor
        {
            get { return (Color)GetValue(UserColorProperty); }
            set { SetValue(UserColorProperty, value); }
        }

        public static readonly DependencyProperty UserColorProperty =
            DependencyProperty.Register("UserColor", typeof(Color), typeof(Stage2UI), new UIPropertyMetadata(Colors.White, new PropertyChangedCallback(OnUserColorChanged)));

        public static readonly RoutedEvent StartRequestedEvent =
            EventManager.RegisterRoutedEvent("StartRequested", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Stage2UI));

        public event RoutedEventHandler RoundComplete
        {
            add { AddHandler(RoundCompleteEvent, value); }
            remove { RemoveHandler(RoundCompleteEvent, value); }
        }

        public static readonly RoutedEvent RoundCompleteEvent =
            EventManager.RegisterRoutedEvent("RoundComplete", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Stage2UI));

        public Round Round
        {
            get { return (Round)GetValue(RoundProperty); }
            set { SetValue(RoundProperty, value); }
        }

        public static readonly DependencyProperty RoundProperty =
            DependencyProperty.Register("Round", typeof(Round), typeof(Stage2UI), new UIPropertyMetadata(null, new PropertyChangedCallback(OnRoundChanged)));

        public PanopticonState PanopticonVideoState
        {
            get { return (PanopticonState)GetValue(PanopticonStateProperty); }
            set { SetValue(PanopticonStateProperty, value); }
        }

        public static readonly DependencyProperty PanopticonStateProperty =
            DependencyProperty.Register("PanopticonState", typeof(PanopticonState), typeof(Stage2UI), new UIPropertyMetadata(PanopticonState.PROCESSING, new PropertyChangedCallback(OnPanopticonVideoStateChanged)));

        public int PanopticonPercentage
        {
            get { return (int)GetValue(PanopticonPercentageProperty); }
            set { SetValue(PanopticonPercentageProperty, value); }
        }

        public static readonly DependencyProperty PanopticonPercentageProperty =
            DependencyProperty.Register("PanopticonPercentage", typeof(int), typeof(Stage2UI), new UIPropertyMetadata(0, new PropertyChangedCallback(OnPanopticonPercentageChanged)));

        public PanopticonInfo PanopticonVideoInfo
        {
            get { return (PanopticonInfo)GetValue(PanopticonVideoInfoProperty); }
            set { SetValue(PanopticonVideoInfoProperty, value); }
        }

        public static readonly DependencyProperty PanopticonVideoInfoProperty =
            DependencyProperty.Register("PanopticonVideoInfo", typeof(PanopticonInfo), typeof(Stage2UI), new UIPropertyMetadata(new PanopticonInfo(), new PropertyChangedCallback(OnPanopticonInfoChanged)));

        public string PanopticonVideoFile
        {
            get { return (string)GetValue(PanopticonVideoFileProperty); }
            set { SetValue(PanopticonVideoFileProperty, value); }
        }

        public static readonly DependencyProperty PanopticonVideoFileProperty =
            DependencyProperty.Register("PanopticonVideoFile", typeof(string), typeof(Stage2UI), new UIPropertyMetadata("", new PropertyChangedCallback(OnPanopticonVideoFileChanged)));

        public List<Press> Presses
        {
            get { return (List<Press>)GetValue(PressesProperty); }
            set { SetValue(PressesProperty, value); }
        }

        public static readonly DependencyProperty PressesProperty =
            DependencyProperty.Register("Presses", typeof(List<Press>), typeof(Stage2UI), new UIPropertyMetadata(new List<Press>(), new PropertyChangedCallback(OnPressesChanged)));

        public string VideoFile
        {
            get { return (string)GetValue(VideoFileProperty); }
            set { SetValue(VideoFileProperty, value); }
        }

        public static readonly DependencyProperty VideoFileProperty =
            DependencyProperty.Register("VideoFile", typeof(string), typeof(Stage2UI), new UIPropertyMetadata("", new PropertyChangedCallback(OnVideoFileChanged)));

        public TimeSpan Length
        {
            get { return (TimeSpan)GetValue(LengthProperty); }
            set { SetValue(LengthProperty, value); }
        }

        public static readonly DependencyProperty LengthProperty =
            DependencyProperty.Register("Length", typeof(TimeSpan), typeof(Stage2UI), new UIPropertyMetadata(TimeSpan.FromSeconds(0), new PropertyChangedCallback(OnLengthChanged)));

        public Stage2UI()
        {
            InitializeComponent();

            Player.AnnotationMade += new PanopticonPlayer.AnnotationMadeEventHandler(Player_AnnotationMade);
            PanopticonStatusLabel.Content = string.Format(Properties.Settings.Default.Panopticon_Processing_Label, 0);
            QuestionUI.ResearchQuestionSubmitted += new ResearchQuestionUI.ResearchQuestionSubmittedEventHandler(QuestionUI_ResearchQuestionSubmitted);
        }

        private void QuestionUI_ResearchQuestionSubmitted(object sender, string question)
        {
            if (ResearchQuestionSubmitted != null)
            {
                ResearchQuestionSubmitted(sender, question);
            }

            RaiseEvent(new RoutedEventArgs(Stage2UI.RoundCompleteEvent, this));
        }

        private void Player_AnnotationMade(object sender, Tablet.PopupPlayer.AnnotationMadeEventArgs e)
        {
            if (AnnotationMade != null)
            {
                AnnotationMade(this, e);
            }
        }

        public void Start(DateTime startTime)
        {
            Timer.Start(startTime);
            PanopticonCover.Visibility = Visibility.Collapsed;
        }

        private static void OnUserColorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Stage2UI sui = (Stage2UI)sender;
            sui.Player.UserColor = sui.UserColor;
        }

        private static void OnPanopticonVideoStateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Stage2UI sui = (Stage2UI)sender;

            switch (sui.PanopticonVideoState)
            {
                case PanopticonState.PROCESSING:
                    sui.PanopticonStatusLabel.Content = string.Format(Properties.Settings.Default.Panopticon_Processing_Label, sui.PanopticonPercentage);
                    sui.PanopticonStatusUI.Visibility = Visibility.Visible;
                    break;
                case PanopticonState.DOWNLOADING:
                    sui.PanopticonStatusLabel.Content = string.Format(Properties.Settings.Default.Panopticon_Downloading_Label, sui.PanopticonPercentage);
                    sui.PanopticonStatusUI.Visibility = Visibility.Visible;
                    break;
            }
        }

        private static void OnPanopticonPercentageChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Stage2UI sui = (Stage2UI)sender;

            switch (sui.PanopticonVideoState)
            {
                case PanopticonState.PROCESSING:
                    sui.PanopticonStatusLabel.Content = string.Format(Properties.Settings.Default.Panopticon_Processing_Label, sui.PanopticonPercentage);
                    break;
                case PanopticonState.DOWNLOADING:
                    sui.PanopticonStatusLabel.Content = string.Format(Properties.Settings.Default.Panopticon_Downloading_Label, sui.PanopticonPercentage);
                    break;
            }
        }

        private static void OnPanopticonInfoChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Stage2UI sui = (Stage2UI)sender;
            sui.Player.PanopticonVideoInfo = sui.PanopticonVideoInfo;
        }

        private static void OnPanopticonVideoFileChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Stage2UI sui = (Stage2UI)sender;
            sui.Player.PanopticonFile = sui.PanopticonVideoFile;
        }

        private static void OnVideoFileChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Stage2UI sui = (Stage2UI)sender;
            sui.Player.VideoFile = sui.VideoFile;
        }

        private static void OnPressesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Stage2UI sui = (Stage2UI)sender;
            sui.Player.Presses = sui.Presses;
        }

        private static void OnRoundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Stage2UI sui = (Stage2UI)sender;

            sui.Timer.Round = sui.Round;
            sui.PanopticonCover.Visibility = Visibility.Visible;
            sui.InstructionsPopupWindow.Instructions = sui.Round.Instructions;
            sui.InstructionsPopupWindow.Visibility = Visibility.Visible;
            sui.QuestionUI.Visibility = Visibility.Hidden;

            switch (sui.Round.Id)
            {
                case Round.RoundId.RESEARCH_QUESTION:
                    sui.QuestionUI.Visibility = Visibility.Visible;
                    break;
                case Round.RoundId.VIDEO_BROWSE:
                    sui.Player.PlayerMode = PanopticonPlayer.Mode.OVERVIEW;
                    break;
                case Round.RoundId.VIDEO_CONVERGE:
                case Round.RoundId.VIDEO_DIVERGE:
                    sui.Player.PlayerMode = PanopticonPlayer.Mode.DIVERGENCE;
                    break;
            }
        }

        private static void OnLengthChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Stage2UI sui = (Stage2UI)sender;
            sui.Player.StudyLength = sui.Length;
        }

        private void Timer_RoundComplete(object sender, RoutedEventArgs e)
        {
            SystemSounds.Exclamation.Play();

            if (Round.Id != Reflectable_v2.Round.RoundId.VIDEO_CONVERGE)
            {
                ConfirmPopupWindow.Visibility = Visibility.Collapsed;
                RaiseEvent(new RoutedEventArgs(Stage2UI.RoundCompleteEvent, this));
            }
        }

        private void Timer_StartRequested(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(Stage2UI.StartRequestedEvent, this));
        }

        private void Player_VideoLoaded(object sender, RoutedEventArgs e)
        {
            PanopticonStatusUI.Visibility = Visibility.Hidden;
        }

        private void ConfirmPopupWindow_Confirmed(object sender, RoutedEventArgs e)
        {
            Timer.Reset();
            RaiseEvent(new RoutedEventArgs(Stage2UI.RoundCompleteEvent, this));
        }

        private void Timer_SkipToNext(object sender, RoutedEventArgs e)
        {
            ConfirmPopupWindow.Visibility = Visibility.Visible;
        }
    }
}
