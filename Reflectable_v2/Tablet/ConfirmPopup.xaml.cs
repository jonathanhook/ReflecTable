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

namespace Tablet
{
    /// <summary>
    /// Interaction logic for ConfirmPopup.xaml
    /// </summary>
    public partial class ConfirmPopup : UserControl
    {
        public event RoutedEventHandler Confirmed
        {
            add { AddHandler(ConfirmedEventHandler, value); }
            remove { RemoveHandler(ConfirmedEventHandler, value); }
        }

        public static readonly RoutedEvent ConfirmedEventHandler =
            EventManager.RegisterRoutedEvent("Confirmed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ConfirmPopup));

        public ConfirmPopup()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ConfirmPopup.ConfirmedEventHandler, this));
            Visibility = Visibility.Collapsed;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }
    }
}
