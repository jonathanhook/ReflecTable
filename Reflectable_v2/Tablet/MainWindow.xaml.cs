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
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;
using Reflectable_v2;
using System.ServiceModel;
using System.IO;
using System.Threading;
using System.ComponentModel;
using System.ServiceModel.Channels;
using System.Diagnostics;
using System.Windows.Threading;

namespace Tablet
{
    public partial class MainWindow : SurfaceWindow, ICallbackContract
    {
        private ITableService proxy;
        private IFileDownloadService fileProxy;
        private Round currentRound;
        private User currentUser;
        private int panopticonPercentage;
        private Stage2UI.PanopticonState panopticonState;
        private string videoFile;
        private string panopticonFile;
        private PanopticonInfo panopticonInfo;
        private List<Press> presses;
        private TimeSpan length;
        private long toDownload;
        private long downloaded;

        public MainWindow()
        {
            InitializeComponent();

            currentUser = new User(Properties.Settings.Default.UserId, null);
            panopticonPercentage = 0;
            panopticonState = Stage2UI.PanopticonState.PROCESSING;
            toDownload = 0;
            downloaded = 0;

            Stage2Control.AnnotationMade += new Stage2UI.AnnotationMadeEventHandler(Stage2Control_AnnotationMade);
            Stage2Control.ResearchQuestionSubmitted += new Stage2UI.ResearchQuestionSubmittedEventHandler(Stage2Control_ResearchQuestionSubmitted);

            Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Window w = App.Current.MainWindow;
            #if DEBUG
            w.WindowState = WindowState.Normal;
            w.WindowStyle = WindowStyle.ThreeDBorderWindow;
            #else
            w.WindowStyle = WindowStyle.None;
            w.WindowState = WindowState.Maximized;
            w.ResizeMode = ResizeMode.NoResize;
            w.Topmost = true;
            #endif
        }

        private void Stage2Control_ResearchQuestionSubmitted(object sender, string question)
        {
            proxy.SetResearchQuestion(question);
        }

        private void Stage2Control_AnnotationMade(object sender, Tablet.PopupPlayer.AnnotationMadeEventArgs e)
        {
            proxy.MakeAnnotation(new Annotation(e.User, e.Comment, e.Press));
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }

        private void HideAll()
        {
            RegisterControl.Visibility = Visibility.Hidden;
            Stage1Control.Visibility = Visibility.Hidden;
            Stage2Control.Visibility = Visibility.Hidden;
        }

        private void Register_RegisterClicked(object sender, RoutedEventArgs e)
        {
            NetTcpBinding serviceBinding = new NetTcpBinding(SecurityMode.None);
            serviceBinding.ReceiveTimeout = TimeSpan.MaxValue;
            serviceBinding.SendTimeout = TimeSpan.MaxValue;
            proxy = DuplexChannelFactory<ITableService>.CreateChannel(this, serviceBinding, new EndpointAddress(Properties.Settings.Default.ServerAddress));
                
            TcpTransportBindingElement transport = new TcpTransportBindingElement();
            transport.MaxReceivedMessageSize = long.MaxValue;
            transport.MaxBufferSize = int.MaxValue;
            transport.MaxBufferPoolSize = long.MaxValue;
            transport.TransferMode = TransferMode.Streamed;

            BinaryMessageEncodingBindingElement encoder = new BinaryMessageEncodingBindingElement();
            CustomBinding fileBinding = new CustomBinding(encoder, transport);
            fileBinding.ReceiveTimeout = TimeSpan.MaxValue;
            fileBinding.SendTimeout = TimeSpan.MaxValue;
            fileProxy = ChannelFactory<IFileDownloadService>.CreateChannel(fileBinding, new EndpointAddress(Properties.Settings.Default.FileServerAddress));

            try
            {
                DateTime serverTime = proxy.GetTime();
                OffsetDateTime.Difference = serverTime - DateTime.UtcNow;

                proxy.Register(currentUser);
            }
            catch (FaultException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (EndpointNotFoundException)
            {
                MessageBox.Show("Could not connect to the table");
            }
            finally
            {
                Register r = (Register)sender;
                r.RegisterButton.Visibility = Visibility.Visible;
                r.Spinner.Visibility = Visibility.Collapsed;
            }
        }

        private void StageControl_RoundComplete(object sender, RoutedEventArgs e)
        {
             proxy.RoundComplete(currentRound);
        }

        private void Stage1Control_ButtonPressed(object sender, RoutedEventArgs e)
        {
            proxy.ButtonPress(currentUser);
        }

        private void StageControl_StartRequested(object sender, RoutedEventArgs e)
        {
            proxy.RequestStartRound();
        }

        private void DownloadData()
        {
            panopticonState = Stage2UI.PanopticonState.DOWNLOADING;
            panopticonPercentage = 0;

            UpdatePanopticonState();
            UpdatePanopticonPercentage();

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler((s, e) =>
            {
                videoFile = proxy.GetVideoFilename();
                panopticonFile = proxy.GetPanopticonFilename();
                panopticonInfo = proxy.GetPanopticonInfo();
                presses = proxy.GetPresses();
                length = proxy.GetStudyLength();

                toDownload = proxy.GetVideoSize() + proxy.GetPanopticonSize();
                downloaded = 0;

                Thread t1 = new Thread(() =>
                {
                    if (!File.Exists(videoFile))
                    {
                        Stream videoStream = fileProxy.DownloadVideo();
                        DownloadFile(videoStream, videoFile);
                    }
                });

                Thread t2 = new Thread(() =>
                {
                    if (!File.Exists(panopticonFile))
                    {
                        Stream panopticonStream = fileProxy.DownloadPanopticonVideo();
                        DownloadFile(panopticonStream, panopticonFile);
                    }
                });

                t1.Start();
                t2.Start();
                t1.Join();
                t2.Join();
 
                panopticonState = Stage2UI.PanopticonState.READY;
                UpdatePanopticonState();
            });

            worker.RunWorkerAsync();
        }

        private void DownloadFile(Stream source, string file)
        {
            byte[] buffer = new byte[4096];
            int position = 0;
            FileStream fStream = File.OpenWrite(FileLocationUtility.GetPathInVideoFolderLocation(file));

            do
            {
                position = source.Read(buffer, 0, buffer.Length);
                fStream.Write(buffer, 0, position);
                downloaded += buffer.Length;

                double percentage = (double)downloaded / (double)toDownload;
                panopticonPercentage = (int)(percentage * 100.0);
                if (panopticonPercentage > 100)
                {
                    panopticonPercentage = 100;
                }
                UpdatePanopticonPercentage();
            }
            while (position != 0);

            fStream.Close();
        }

        private void UpdatePanopticonState()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Stage2Control.PanopticonVideoState = panopticonState;

                if (panopticonState == Stage2UI.PanopticonState.READY)
                {
                    Stage2Control.PanopticonVideoFile = FileLocationUtility.GetPathInVideoFolderLocation(panopticonFile);
                    Stage2Control.VideoFile = FileLocationUtility.GetPathInVideoFolderLocation(videoFile);
                    Stage2Control.Presses = presses;
                    Stage2Control.Length = length;
                    Stage2Control.PanopticonVideoInfo = panopticonInfo;
                }
            }));
        }

        private void UpdatePanopticonPercentage()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Stage2Control.PanopticonPercentage = panopticonPercentage;
            }));
        }

        #region WebService
        public void SetRound(Round r)
        {
            this.currentRound = r;
            HideAll();

            switch (r.Id)
            {
                case Round.RoundId.PAPER_DIVERGE:
                case Round.RoundId.PAPER_CONVERGE:
                case Round.RoundId.PAPER_TRANSCEND:
                    Stage1Control.Visibility = Visibility.Visible;
                    Stage1Control.Round = r;
                    break;
                case Round.RoundId.VIDEO_BROWSE:
                case Round.RoundId.RESEARCH_QUESTION:
                case Round.RoundId.VIDEO_DIVERGE:
                case Round.RoundId.VIDEO_CONVERGE:
                    Stage2Control.Visibility = Visibility.Visible;
                    Stage2Control.PanopticonVideoState = panopticonState;
                    Stage2Control.PanopticonPercentage = panopticonPercentage;
                    Stage2Control.Round = r;

                    if (panopticonState == Stage2UI.PanopticonState.PROCESSING)
                    {
                        Thread t = new Thread(() =>
                        {
                            if (proxy.IsPanopticonProcessingComplete())
                            {
                                DownloadData();
                            }
                        });
                        t.Start();
                    }
                    break;
                case Round.RoundId.COMPLETE:
                    App.Restart();
                    break;
            }
        }

        public void StartRound(DateTime startTime)
        {
            switch (currentRound.Id)
            {
                case Round.RoundId.PAPER_DIVERGE:
                case Round.RoundId.PAPER_CONVERGE:
                case Round.RoundId.PAPER_TRANSCEND:
                    Stage1Control.Start(startTime);
                    break;
                case Round.RoundId.VIDEO_BROWSE:
                case Round.RoundId.VIDEO_DIVERGE:
                case Round.RoundId.VIDEO_CONVERGE:
                    Stage2Control.Start(startTime);
                    break;
            }
        }

        public void PanopticonCompleted()
        {
            if (panopticonState == Stage2UI.PanopticonState.PROCESSING)
            {
                DownloadData();
            }
        }

        public void PanopticonPercentageChanged(int percentage)
        {
            this.panopticonPercentage = percentage;
            UpdatePanopticonPercentage();
        }

        public void SetUserColor(Color c)
        {
            currentUser.Color = c;
            Stage1Control.UserColor = c;
            Stage2Control.UserColor = c;
        }
        #endregion
    }
}