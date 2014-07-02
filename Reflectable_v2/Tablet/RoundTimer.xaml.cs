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
    /// Interaction logic for RoundTimer.xaml
    /// </summary>
    public partial class RoundTimer : UserControl
    {
        public event RoutedEventHandler StartRequested
        {
            add { AddHandler(StartRequestedEvent, value); }
            remove { RemoveHandler(StartRequestedEvent, value); }
        }

        public static readonly RoutedEvent StartRequestedEvent =
            EventManager.RegisterRoutedEvent("StartRequested", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RoundTimer));

        public event RoutedEventHandler RoundComplete
        {
            add { AddHandler(RoundCompleteEvent, value); }
            remove { RemoveHandler(RoundCompleteEvent, value); }
        }

        public static readonly RoutedEvent RoundCompleteEvent =
            EventManager.RegisterRoutedEvent("RoundComplete", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RoundTimer));

        public event RoutedEventHandler SkipToNext
        {
            add { AddHandler(SkipToNextEvent, value); }
            remove { RemoveHandler(SkipToNextEvent, value); }
        }

        public static readonly RoutedEvent SkipToNextEvent =
            EventManager.RegisterRoutedEvent("SkipToNext", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RoundTimer));

        public Round Round
        {
            get { return (Round)GetValue(RoundProperty); }
            set { SetValue(RoundProperty, value); }
        }

        public static readonly DependencyProperty RoundProperty =
            DependencyProperty.Register("Round", typeof(Round), typeof(RoundTimer), new UIPropertyMetadata(null, new PropertyChangedCallback(OnRoundChanged)));

        private const string ROUND_LABEL_FORMAT = "Round {0}";

        public RoundTimer()
        {
            InitializeComponent();
        }

        public void Start(DateTime startTime)
        {
            Timer.Start(startTime);

            if (Round.Id == Round.RoundId.VIDEO_CONVERGE)
            {
                EndButton.Content = Properties.Settings.Default.Finish;
            }

            StartButton.Visibility = Visibility.Collapsed;
            Spinner.Visibility = Visibility.Collapsed;
            EndButton.Visibility = Visibility.Visible;
        }

        public void Reset()
        {
            Timer.Stop();

            StartButton.Visibility = Visibility.Visible;
            EndButton.Visibility = Visibility.Collapsed;

            string labelText = string.Format(ROUND_LABEL_FORMAT, Round.Seq);
            RoundLabel.Content = labelText;
        }

        private static void OnRoundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RoundTimer rt = (RoundTimer)sender;
            rt.Timer.SetDuration(rt.Round.Length);
            rt.Reset();
        }

        private void EndButton_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(RoundTimer.SkipToNextEvent, this));
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Spinner.Visibility = Visibility.Visible;
            StartButton.Visibility = Visibility.Hidden;
            RaiseEvent(new RoutedEventArgs(RoundTimer.StartRequestedEvent, this));
        }

        private void Timer_Completed(object sender, RoutedEventArgs e)
        {
            Timer.Stop();
            RaiseEvent(new RoutedEventArgs(RoundTimer.RoundCompleteEvent, this));
        }
    }
}
