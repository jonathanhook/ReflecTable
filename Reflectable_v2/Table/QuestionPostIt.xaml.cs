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

namespace Table
{
    /// <summary>
    /// Interaction logic for QuestionPostIt.xaml
    /// </summary>
    public partial class QuestionPostIt : UserControl
    {
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(QuestionPostIt), new UIPropertyMetadata("", new PropertyChangedCallback(OnTextChanged)));

        public QuestionPostIt()
        {
            InitializeComponent();
        }

        private static void OnTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            QuestionPostIt p = (QuestionPostIt)sender;
            p.TextLabel.Text = p.Text;
        }
    }
}
