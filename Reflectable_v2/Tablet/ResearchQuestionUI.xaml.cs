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
    /// Interaction logic for ResearchQuestionControl.xaml
    /// </summary>
    public partial class ResearchQuestionUI : UserControl
    {
        public delegate void ResearchQuestionSubmittedEventHandler(object sender, string question);
        public event ResearchQuestionSubmittedEventHandler ResearchQuestionSubmitted;

        public ResearchQuestionUI()
        {
            InitializeComponent();
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            if (QuestionBox.Text != "")
            {
                Submit.Visibility = Visibility.Hidden;
                Spinner.Visibility = Visibility.Visible;

                if (ResearchQuestionSubmitted != null)
                {
                    ResearchQuestionSubmitted(this, QuestionBox.Text);
                }
            }
        }
    }
}
