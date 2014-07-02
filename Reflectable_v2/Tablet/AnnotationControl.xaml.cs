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
    /// Interaction logic for AnnotationControl.xaml
    /// </summary>
    public partial class AnnotationControl : UserControl
    {
        public Color UserColor
        {
            get { return (Color)GetValue(UserColorProperty); }
            set { SetValue(UserColorProperty, value); }
        }

        public static readonly DependencyProperty UserColorProperty =
            DependencyProperty.Register("UserColor", typeof(Color), typeof(AnnotationControl), new UIPropertyMetadata(Colors.White));

        public event RoutedEventHandler PressSelected
        {
            add { AddHandler(PressSelectedEvent, value); }
            remove { RemoveHandler(PressSelectedEvent, value); }
        }

        public static readonly RoutedEvent PressSelectedEvent =
            EventManager.RegisterRoutedEvent("PressSelected", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AnnotationControl));

        public Press AnnotationPress
        {
            get { return (Press)GetValue(AnnotationPressProperty); }
            set { SetValue(AnnotationPressProperty, value); }
        }

        public static readonly DependencyProperty AnnotationPressProperty =
            DependencyProperty.Register("AnnotationPress", typeof(Press), typeof(AnnotationControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnPressChanged)));

        public AnnotationControl()
        {
            InitializeComponent();
        }

        private static void OnPressChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            AnnotationControl ac = (AnnotationControl)sender;
            ac.IdEllipse.Fill = new SolidColorBrush(ac.UserColor);
        }

        private void UserControl_MouseUp(object sender, EventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(AnnotationControl.PressSelectedEvent, this));
        }
    }
}
