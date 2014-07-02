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
using Microsoft.Surface.Presentation.Controls;
using System.Windows.Media.Animation;

namespace Table
{
    /// <summary>
    /// Interaction logic for Stage2TabletopUI.xaml
    /// </summary>
    public partial class Stage2TabletopUI : UserControl
    {
        public List<Press> Presses
        {
            get { return (List<Press>)GetValue(PressesProperty); }
            set { SetValue(PressesProperty, value); }
        }

        public static readonly DependencyProperty PressesProperty =
            DependencyProperty.Register("Presses", typeof(List<Press>), typeof(Stage2TabletopUI), new UIPropertyMetadata(new List<Press>(), new PropertyChangedCallback(OnPressesChanged)));

        public string VideoFile
        {
            get { return (string)GetValue(VideoFileProperty); }
            set { SetValue(VideoFileProperty, value); }
        }

        public static readonly DependencyProperty VideoFileProperty =
            DependencyProperty.Register("VideoFile", typeof(string), typeof(Stage2TabletopUI), new UIPropertyMetadata(""));       

        private const double BORDER = 0.25;
        private const int POSTIT_VARIATION = 50;

        private Random rand;

        public Stage2TabletopUI()
        {
            rand = new Random(DateTime.UtcNow.Millisecond);
            InitializeComponent();

            /*
            Group g = new Group();
            ScatterViewItem svi = new ScatterViewItem();
            svi.TouchLeave += g.SendToBack;
            svi.Content = g;
            Scatter.Items.Add(svi);
            */
        }

        private static void OnPressesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Stage2TabletopUI s = (Stage2TabletopUI)sender;
            s.GeneratePresses();
        }

        public void AddAnnotation(Annotation a)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Point pos;
                if (GetPressScatterViewItemPosition(a.Press, out pos))
                {
                    PostIt postIt = new PostIt();
                    postIt.Text = a.Comment;
                    postIt.User = a.User;

                    ScatterViewItem svi = new ScatterViewItem();
                    svi.Content = postIt;
                    svi.Center = GetPostItStartPosition(a.User);
                    svi.CanScale = false;
                    svi.CanMove = true;

                    Scatter.Items.Add(svi);

                    pos.X += rand.Next(-POSTIT_VARIATION, POSTIT_VARIATION);
                    pos.Y += rand.Next(-POSTIT_VARIATION, POSTIT_VARIATION);

                    Duration duration = new Duration(TimeSpan.FromSeconds(0.5));
                    PointAnimation an = new PointAnimation();
                    an.Duration = duration;
                    an.FillBehavior = FillBehavior.Stop;
                    an.To = pos;

                    Storyboard sb = new Storyboard();
                    sb.Duration = duration;
                    sb.Children.Add(an);

                    Storyboard.SetTarget(an, svi);
                    Storyboard.SetTargetProperty(an, new PropertyPath(ScatterViewItem.CenterProperty));

                    sb.Completed += new EventHandler((s, e) =>
                    {
                        svi.Center = pos;
                    });

                    sb.Begin(Scatter);
                }
            }));
        }

        public void SetResearchQuestion(string s)
        {
            QuestionPostIt postIt = new QuestionPostIt();
            postIt.Text = s;

            ScatterViewItem svi = new ScatterViewItem();
            svi.Content = postIt;
            svi.Center = new Point(ActualWidth / 2.0, ActualHeight / 2.0);
            svi.CanScale = false;
            svi.CanMove = true;

            Scatter.Items.Add(svi);
        }

        private Point GetPostItStartPosition(User u)
        {
            switch (u.Id)
            {
                case 0:
                    return new Point(0.0, ActualHeight / 2.0);
                case 1:
                    return new Point(ActualWidth / 2.0, 0.0);
                case 2:
                    return new Point(ActualWidth, ActualHeight / 2.0);
                case 3:
                    return new Point(ActualWidth / 2.0, ActualHeight);
                default:
                    return new Point(0.0, 0.0);
            }
        }

        private bool GetPressScatterViewItemPosition(Press p, out Point point)
        {
            foreach (ScatterViewItem i in Scatter.Items)
            {
                object content = i.Content;
                if (content.GetType() == typeof(PressPlayer))
                {
                    PressPlayer pp = (PressPlayer)content;
                    if (pp.Press.Id == p.Id)
                    {
                        point = i.Center;
                        return true;
                    }
                }
            }

            point = new Point();
            return false;
        }

        private void GeneratePresses()
        {
            int numPresses = Presses.Count;
            if (numPresses > 0)
            {
                int rowLength = (int)(Math.Ceiling((Math.Sqrt((double)numPresses))));
                int numRows = numPresses / rowLength;

                double width = Scatter.ActualWidth;
                double height = Scatter.ActualHeight;
                double borderW = width * BORDER;
                double borderH = height * BORDER;
                double xOffset = (width - borderW) / (double)rowLength;
                double yOffset = (height - borderH) / (double)numRows;

                for (int i = 0; i < numPresses; i++)
                {
                    int cx = i % rowLength;
                    int cy = i / rowLength;
                    double x = ((double)cx * xOffset) + (borderW / 2.0);
                    double y = ((double)cy * yOffset) + (borderH / 2.0);

                    PressPlayer prpl = new PressPlayer();
                    prpl.Press = Presses[i];
                    prpl.VideoFile = FileLocationUtility.GetPathInVideoFolderLocation(VideoFile);

                    ScatterViewItem svi = new ScatterViewItem();
                    svi.Width = 160;
                    svi.Height = 120;
                    svi.Center = new Point(x, y);
                    svi.Content = prpl;

                    Scatter.Items.Add(svi);
                }
            }
        }

    }
}
