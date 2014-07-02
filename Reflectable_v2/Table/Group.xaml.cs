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
using Microsoft.Surface.Presentation.Controls;

namespace Table
{
    /// <summary>
    /// Interaction logic for Group.xaml
    /// </summary>
    public partial class Group : UserControl
    {
        public Group()
        {
            InitializeComponent();
        }

        public void SendToBack(object sender, EventArgs e)
        {
            DependencyObject result = (DependencyObject)sender;
            while (result != null && result.GetType() != typeof(ScatterViewItem))
            {
                result = VisualTreeHelper.GetParent(result);
            }

            if (result != null)
            {
                ScatterViewItem container = (ScatterViewItem)result;
                container.SetRelativeZIndex(RelativeScatterViewZIndex.Bottommost);
            }
        }

    }
}
