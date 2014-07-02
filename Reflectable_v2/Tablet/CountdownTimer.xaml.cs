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
using System.Windows.Threading;
using System.Diagnostics;

namespace Tablet
{
    /// <summary>
    /// Interaction logic for CountdownTimer.xaml
    /// </summary>
    public partial class CountdownTimer : UserControl
    {
        public event RoutedEventHandler Completed
        {
            add { AddHandler(CompletedEvent, value); }
            remove { RemoveHandler(CompletedEvent, value); }
        }

        public static readonly RoutedEvent CompletedEvent = 
            EventManager.RegisterRoutedEvent("Completed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CountdownTimer));

        private const string TIMER_FORMAT = "{0} : {1:00}";

        private DateTime started;
        private DispatcherTimer timer;
        private TimeSpan duration;

        public CountdownTimer()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
        }

        public void SetDuration(TimeSpan duration)
        {
            this.duration = duration;
            TimerLabel.Content = string.Format(TIMER_FORMAT, (int)duration.Minutes, (int)duration.Seconds);
            Pie.StartAngle = 360.0;
        }

        public void Start(DateTime started)
        {
            this.started = started;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 250);
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
            timer.Interval = TimeSpan.Zero;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            DateTime serverNow = OffsetDateTime.GetOffsetTime();
            TimeSpan progress = duration - (serverNow - started);
            TimerLabel.Content = string.Format(TIMER_FORMAT, (int)progress.Minutes, (int)progress.Seconds);
            
            if (progress <= TimeSpan.Zero)
            {
                RoutedEventArgs eventArgs = new RoutedEventArgs(CountdownTimer.CompletedEvent);
                RaiseEvent(eventArgs);
            }

            double percent = 1.0 - (progress.TotalMilliseconds / duration.TotalMilliseconds);
            if (percent > 1.0)
            {
                percent = 1.0;
            }
            else if (percent < 0.0)
            {
                percent = 0.0;
            }

            Pie.StartAngle = (percent * 360.0);
        }
    }
}
