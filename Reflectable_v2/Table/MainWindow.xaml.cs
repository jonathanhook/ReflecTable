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
using System.Threading;
using Table;
using System.ServiceModel;
using System.Diagnostics;
using Reflectable_v2;
using System.IO;

namespace Table
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false)]
    public partial class MainWindow : SurfaceWindow, ITableService
    {
        private const int MAX_CLIENTS = 4;

        private ServiceHost serviceHost;
        private Dictionary<int, ICallbackContract> tablets;
        private Round currentRound;
        private bool roundInProgress;
        private bool gameStarted;
        private CameraCapture camera;
        private DateTime startTime;
        private List<Press> presses;
        private PanopticonProcessor panopticonProcessor;
        private DateTime lastPercentageUpdate;
        private FileDownloadService fileDownloadService;
        private bool researchQuestionSet;
        private Dictionary<Round.RoundId, Round> rounds;

        public MainWindow()
        {
            rounds = new Dictionary<Round.RoundId, Round>()
            {
                { Round.RoundId.PAPER_DIVERGE, new Round(Round.RoundId.PAPER_DIVERGE, 1, Properties.Settings.Default.Round_1_Time, Properties.Settings.Default.Round_1_Instructions) },
                { Round.RoundId.PAPER_CONVERGE, new Round(Round.RoundId.PAPER_CONVERGE, 2, Properties.Settings.Default.Round_2_Time, Properties.Settings.Default.Round_2_Instructions) },
                { Round.RoundId.PAPER_TRANSCEND, new Round(Round.RoundId.PAPER_TRANSCEND, 3, Properties.Settings.Default.Round_3_Time, Properties.Settings.Default.Round_3_Instructions) },
                { Round.RoundId.VIDEO_BROWSE, new Round(Round.RoundId.VIDEO_BROWSE, 4, Properties.Settings.Default.Round_4_Time, Properties.Settings.Default.Round_4_Instructions) },
                { Round.RoundId.RESEARCH_QUESTION, new Round(Round.RoundId.RESEARCH_QUESTION, 0, TimeSpan.Zero, "") },
                { Round.RoundId.VIDEO_DIVERGE, new Round(Round.RoundId.VIDEO_DIVERGE, 5, Properties.Settings.Default.Round_5_Time, Properties.Settings.Default.Round_5_Instructions) },
                { Round.RoundId.VIDEO_CONVERGE, new Round(Round.RoundId.VIDEO_CONVERGE, 6, Properties.Settings.Default.Round_6_Time, Properties.Settings.Default.Round_6_Instructions) },
                { Round.RoundId.COMPLETE, new Round(Round.RoundId.COMPLETE, 0, new TimeSpan(), Properties.Settings.Default.Complete_Instructions) }
            };

            currentRound = rounds[Round.RoundId.PAPER_DIVERGE];
            tablets = new Dictionary<int, ICallbackContract>();
            roundInProgress = false;
            gameStarted = false;
            camera = new CameraCapture();
            presses = new List<Press>();
            lastPercentageUpdate = DateTime.UtcNow;
            fileDownloadService = new FileDownloadService();
            researchQuestionSet = false;

            panopticonProcessor = new PanopticonProcessor();
            panopticonProcessor.PercentageChanged += new PanopticonProcessor.PercentageChangedEventHandler(panopticonProcessor_PercentageChanged);
            panopticonProcessor.ProcessingCompleted += new PanopticonProcessor.ProcessingCompletedEventHandler(panopticonProcessor_ProcessingCompleted);
            panopticonProcessor.ProcessingFailed += new PanopticonProcessor.ProcessingFailedEventHandler(panopticonProcessor_ProcessingFailed);

            NetTcpBinding serviceBinding = new NetTcpBinding(SecurityMode.None);
            serviceBinding.ReceiveTimeout = TimeSpan.MaxValue;
            serviceBinding.SendTimeout = TimeSpan.MaxValue;
            serviceHost = new ServiceHost(this);
            serviceHost.AddServiceEndpoint(typeof(ITableService), serviceBinding, "net.tcp://localhost:8010");
            serviceHost.Open();

            Loaded += new RoutedEventHandler(MainWindow_Loaded);

            InitializeComponent();
        }

        public void Disconnect()
        {
            serviceHost.Close();
            fileDownloadService.Close();
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

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (camera.IsCapturing)
            {
                camera.StopCapture();
            }
        }

        private void StartGame()
        {
            camera.StartCapture();
            startTime = DateTime.UtcNow;
            gameStarted = true;
        }

        private void panopticonProcessor_ProcessingCompleted(object sender)
        {
            fileDownloadService.PanopticonVideoFile = panopticonProcessor.Filename;

            tablets.AsParallel().ForAll((kvp) =>
            {
                try
                {
                    kvp.Value.PanopticonCompleted();
                }
                catch (CommunicationObjectAbortedException)
                {
                }
            });
        }

        private void panopticonProcessor_PercentageChanged(object sender, int percentage)
        {
            tablets.AsParallel().ForAll((kvp) =>
            {
                try
                {
                    kvp.Value.PanopticonPercentageChanged(percentage);
                }
                catch (CommunicationObjectAbortedException)
                {
                }
            });
        }

        void panopticonProcessor_ProcessingFailed(object sender, Exception e)
        {
            StartPanopticonProcessing();
        }

        private static Color CovertColor(System.Drawing.Color c)
        {
            return Color.FromArgb(c.A, c.R, c.G, c.B);
        }

        public static Color GetColorFromUser(User u)
        {
            Color c;
            switch (u.Id)
            {
                case 0:
                    c = CovertColor(Properties.Settings.Default.User0Colour);
                    break;
                case 1:
                    c = CovertColor(Properties.Settings.Default.User1Colour);
                    break;
                case 2:
                    c = CovertColor(Properties.Settings.Default.User2Colour);
                    break;
                case 3:
                    c = CovertColor(Properties.Settings.Default.User3Colour);
                    break;
                default:
                    c = Colors.White;
                    break;
            }

            return c;
        }

        private void StartPanopticonProcessing()
        {
            Thread t = new Thread(() =>
            {
                camera.StopCapture();
                fileDownloadService.VideoFile = camera.VideoFileName;
                panopticonProcessor.ProcessFile(camera.VideoFileName);
            });
            t.Start();
        }

        #region WebService
        public void Register(User u)
        {
            try
            {
                ICallbackContract tablet = OperationContext.Current.GetCallbackChannel<ICallbackContract>();

                if (tablets.ContainsKey(u.Id))
                {
                    tablets.Remove(u.Id);
                }

                tablet.SetUserColor(GetColorFromUser(u));
                tablets[u.Id] = tablet;
                tablet.SetRound(currentRound);
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message + "\r\n" + e.StackTrace));
            }
        }

        public void RoundComplete(Round r)
        {
            try
            {
                if (r.Id == Round.RoundId.PAPER_TRANSCEND)
                {
                    StartPanopticonProcessing();
                }

                int i = (int)(r.Id + 1);
                Round.RoundId newId = (Round.RoundId)(i % Round.NUM_ROUNDS);

                if (newId != currentRound.Id)
                {
                    currentRound = rounds[newId];
                    roundInProgress = false;

                    bool restart = false;
                    switch (currentRound.Id)
                    {
                        case Round.RoundId.VIDEO_BROWSE:
                            Dispatcher.BeginInvoke(new Action(() =>
                            {
                                Tabletop.VideoFile = camera.VideoFileName;
                                Tabletop.Presses = presses;
                                Tabletop.Visibility = Visibility.Visible;
                                Screensaver.Visibility = Visibility.Hidden;
                            }));
                            break;
                        case Round.RoundId.COMPLETE:
                            restart = true;
                            break;
                    }

                    tablets.AsParallel().ForAll((kvp) =>
                    {
                        try
                        {
                            kvp.Value.SetRound(currentRound);
                        }
                        catch (CommunicationObjectAbortedException)
                        {
                        }
                    });

                    if (restart)
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            Disconnect();
                            App.Restart();
                        }));
                    }
                }
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message + "\r\n" + e.StackTrace));
            }
        }

        public void RequestStartRound()
        {
            try
            {
                if (!roundInProgress)
                {
                    roundInProgress = true;

                    if (currentRound.Id == Round.RoundId.PAPER_DIVERGE && !gameStarted)
                    {
                        StartGame();
                    }

                    DateTime startTime = DateTime.UtcNow;
                    tablets.AsParallel().ForAll((kvp) =>
                    {
                        try
                        {
                            kvp.Value.StartRound(startTime);
                        }
                        catch (CommunicationObjectAbortedException)
                        {
                        }
                    });
                }
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message + "\r\n" + e.StackTrace));
            }
        }

        public void ButtonPress(User u)
        {
            try
            {
                if (gameStarted && roundInProgress)
                {
                    Color c = GetColorFromUser(u);
                    u.Color = c;

                    TimeSpan end = DateTime.UtcNow - startTime;
                    TimeSpan start = end - Properties.Settings.Default.PressClipLength;
                    if (start < TimeSpan.Zero)
                    {
                        start = TimeSpan.Zero;
                    }

                    if (end < TimeSpan.Zero)
                    {
                        end = TimeSpan.Zero;
                    }

                    Press p = new Press(start, end, u);
                    presses.Add(p);
                }
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message + "\r\n" + e.StackTrace));
            }
        }

        public bool IsPanopticonProcessingComplete()
        {
            try
            {
                return panopticonProcessor.IsComplete;
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message + "\r\n" + e.StackTrace));
            }
        }

        public string GetPanopticonFilename()
        {
            try
            {
                return panopticonProcessor.Filename;
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message + "\r\n" + e.StackTrace));
            }
        }

        public string GetVideoFilename()
        {
            try
            {
                return camera.VideoFileName;
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message + "\r\n" + e.StackTrace));
            }
        }

        public PanopticonInfo GetPanopticonInfo()
        {
            try
            {
                return panopticonProcessor.Info;
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message + "\r\n" + e.StackTrace));
            }
        }

        public List<Press> GetPresses()
        {
            try
            {
                return presses;
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message + "\r\n" + e.StackTrace));
            }
        }

        public TimeSpan GetStudyLength()
        {
            try
            {
                return camera.Length;
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message + "\r\n" + e.StackTrace));
            }
        }

        public void MakeAnnotation(Annotation a)
        {
            try
            {
                Tabletop.AddAnnotation(a);
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message + "\r\n" + e.StackTrace));
            }
        }

        public void SetResearchQuestion(string s)
        {
            try
            {
                if (!researchQuestionSet)
                {
                    researchQuestionSet = true;
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Tabletop.SetResearchQuestion(s);
                    }));
                }
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message + "\r\n" + e.StackTrace));
            }
        }

        public long GetPanopticonSize()
        {
            try
            {
                string path = FileLocationUtility.GetPathInVideoFolderLocation(panopticonProcessor.Filename);
                FileInfo info = new FileInfo(path);
                return info.Length;
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message + "\r\n" + e.StackTrace));
            }
        }

        public long GetVideoSize()
        {
            try
            {
                string path = FileLocationUtility.GetPathInVideoFolderLocation(camera.VideoFileName);
                FileInfo info = new FileInfo(path);
                return info.Length;
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message + "\r\n" + e.StackTrace));
            }
        }

        public DateTime GetTime()
        {
            try
            {
                return DateTime.UtcNow;
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message + "\r\n" + e.StackTrace));
            }
        }
        #endregion
    }
}