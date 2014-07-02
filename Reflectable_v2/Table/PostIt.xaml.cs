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

namespace Table
{
    /// <summary>
    /// Interaction logic for PostIt.xaml
    /// </summary>
    public partial class PostIt : UserControl
    {
        public User User
        {
            get { return (User)GetValue(UserProperty); }
            set { SetValue(UserProperty, value); }
        }

        public static readonly DependencyProperty UserProperty =
            DependencyProperty.Register("User", typeof(User), typeof(PostIt), new UIPropertyMetadata(null, new PropertyChangedCallback(OnUserChanged)));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(PostIt), new UIPropertyMetadata("", new PropertyChangedCallback(OnTextChanged)));

        public PostIt()
        {
            InitializeComponent();
        }

        private static void OnUserChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PostIt p = (PostIt)sender;
            p.BackgroundBorder.Background = new SolidColorBrush(MainWindow.GetColorFromUser(p.User));
        }

        private static void OnTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PostIt p = (PostIt)sender;
            p.TextLabel.Text = p.Text;
        }
    }
}
