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
    public partial class InstructionsPopup : UserControl
    {
        public string Instructions
        {
            get { return (string)GetValue(InstructionsProperty); }
            set { SetValue(InstructionsProperty, value); }
        }

        public static readonly DependencyProperty InstructionsProperty =
            DependencyProperty.Register("Instructions", typeof(string), typeof(InstructionsPopup), new UIPropertyMetadata("", new PropertyChangedCallback(OnInstructionsChanged)));

        public InstructionsPopup()
        {
            InitializeComponent();
        }

        private static void OnInstructionsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            InstructionsPopup i = (InstructionsPopup)sender;
            i.InstructionsLabel.Text = i.Instructions;
        }

        private void SurfaceButton_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }
    }
}
