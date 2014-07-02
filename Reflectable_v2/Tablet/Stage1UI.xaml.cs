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
using System.IO;
using Reflectable_v2;
using System.Media;

namespace Tablet
{
    /// <summary>
    /// Interaction logic for Stage1UI.xaml
    /// </summary>
    public partial class Stage1UI : UserControl
    {
        public Color UserColor
        {
            get { return (Color)GetValue(UserColorProperty); }
            set { SetValue(UserColorProperty, value); }
        }

        public static readonly DependencyProperty UserColorProperty =
            DependencyProperty.Register("UserColor", typeof(Color), typeof(Stage1UI), new UIPropertyMetadata(Colors.White));

        public event RoutedEventHandler ButtonPressed
        {
            add { AddHandler(ButtonPressedEvent, value); }
            remove { RemoveHandler(ButtonPressedEvent, value); }
        }

        public static readonly RoutedEvent ButtonPressedEvent =
            EventManager.RegisterRoutedEvent("ButtonPressed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Stage1UI));

        public event RoutedEventHandler StartRequested
        {
            add { AddHandler(StartRequestedEvent, value); }
            remove { RemoveHandler(StartRequestedEvent, value); }
        }

        public static readonly RoutedEvent StartRequestedEvent =
            EventManager.RegisterRoutedEvent("StartRequested", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Stage1UI));


        public event RoutedEventHandler RoundComplete
        {
            add { AddHandler(RoundCompleteEvent, value); }
            remove { RemoveHandler(RoundCompleteEvent, value); }
        }

        public static readonly RoutedEvent RoundCompleteEvent =
            EventManager.RegisterRoutedEvent("RoundComplete", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Stage1UI));

        public Round Round
        {
            get { return (Round)GetValue(RoundProperty); }
            set { SetValue(RoundProperty, value); }
        }

        public static readonly DependencyProperty RoundProperty =
            DependencyProperty.Register("Round", typeof(Round), typeof(Stage1UI), new UIPropertyMetadata(null, new PropertyChangedCallback(OnRoundChanged)));

        public Stage1UI()
        {
            InitializeComponent();

            Loaded += new RoutedEventHandler(Stage1UI_Loaded);
        }

        private void Stage1UI_Loaded(object sender, RoutedEventArgs e)
        {
            ButtonContainer.Width = ActualHeight * (4.0 / 3.0);
        }

        private static void OnRoundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Stage1UI sui = (Stage1UI)sender;

            sui.Timer.Round = sui.Round;
            sui.InstructionsPopupWindow.Instructions = sui.Round.Instructions;
            sui.InstructionsPopupWindow.Visibility = Visibility.Visible;
            sui.ButtonCover.Visibility = Visibility.Visible;
            sui.MainButton.Background = new SolidColorBrush(sui.UserColor);
        }

        public void Start(DateTime startTime)
        {
            Timer.Start(startTime);
            ButtonCover.Visibility = Visibility.Collapsed;
        }

        private void MainButton_Click(object sender, RoutedEventArgs e)
        {
            SystemSounds.Asterisk.Play();
            RaiseEvent(new RoutedEventArgs(Stage1UI.ButtonPressedEvent, this));
        }

        private void Timer_RoundComplete(object sender, RoutedEventArgs e)
        {
            SystemSounds.Exclamation.Play();
            ConfirmPopupWindow.Visibility = Visibility.Collapsed;
            RaiseEvent(new RoutedEventArgs(Stage1UI.RoundCompleteEvent, this));
        }

        private void Timer_StartRequested(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(Stage1UI.StartRequestedEvent, this));
        }

        private void ConfirmPopupWindow_Confirmed(object sender, RoutedEventArgs e)
        {
            Timer.Reset();
            RaiseEvent(new RoutedEventArgs(Stage1UI.RoundCompleteEvent, this));
        }

        private void Timer_SkipToNext(object sender, RoutedEventArgs e)
        {
            ConfirmPopupWindow.Visibility = Visibility.Visible;
        }
    }
}
