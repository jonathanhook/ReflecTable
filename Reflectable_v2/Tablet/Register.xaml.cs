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
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : UserControl
    {
        public event RoutedEventHandler RegisterClicked
        {
            add { AddHandler(RegisterClickedEvent, value); }
            remove { RemoveHandler(RegisterClickedEvent, value); }
        }

        public static readonly RoutedEvent RegisterClickedEvent =
            EventManager.RegisterRoutedEvent("RegisterClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Register));

        public Register()
        {
            InitializeComponent();
        }

        private void SurfaceButton_Click(object sender, RoutedEventArgs e)
        {
            RegisterButton.Visibility = Visibility.Hidden;
            Spinner.Visibility = Visibility.Visible;
            RoutedEventArgs eventArgs = new RoutedEventArgs(Register.RegisterClickedEvent);
            RaiseEvent(eventArgs);
        }
    }
}
