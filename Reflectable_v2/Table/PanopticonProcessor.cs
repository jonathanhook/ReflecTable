using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PanopticonCS;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using Reflectable_v2;
using System.Timers;

namespace Table
{
    class PanopticonProcessor
    {
        public delegate void ProcessingFailedEventHandler(object sender, Exception e);
        public event ProcessingFailedEventHandler ProcessingFailed;

        public delegate void PercentageChangedEventHandler(object sender, int percentage);
        public event PercentageChangedEventHandler PercentageChanged;

        public delegate void ProcessingCompletedEventHandler(object sender);
        public event ProcessingCompletedEventHandler ProcessingCompleted;

        private const string PROCESSOR = @"\Resources\PantopticonCLI\panopticon-cli.exe";
        private const string COMMAND_LINE_PARAMS = "-maxw 7 -fps 10";

        public bool IsComplete { get; private set; }
        public string Filename { get; private set; }
        public PanopticonInfo Info { get; private set; }

        private PanopticonCLI panopticonCLI;

        public PanopticonProcessor()
        {
            Filename = "";
            IsComplete = false;
        }

        public void ProcessFile(string sourceFile)
        {
            IsComplete = false;
            Filename = Path.ChangeExtension(sourceFile, ".avi");

            Assembly a = System.Reflection.Assembly.GetEntryAssembly();
            string processorDir = Path.GetDirectoryName(a.Location);
            string videoDir = FileLocationUtility.GetVideoFolderLoctation();

            panopticonCLI = new PanopticonCLI(processorDir + "\\" + PROCESSOR);
            panopticonCLI.Completed += new PanopticonCLI.CompletedEventHandler(panopticonCLI_Completed);
            panopticonCLI.ProgressChanged += new PanopticonCLI.ProgressChangedEventHandler(panopticonCLI_ProgressChanged);
            panopticonCLI.ConvertAsync(videoDir + "\\" + sourceFile, videoDir + "\\" + Filename, COMMAND_LINE_PARAMS);
        }

        private void panopticonCLI_ProgressChanged(object sender, PanopticonCLI.ProgressChangedEventArgs e)
        {
            if (PercentageChanged != null)
            {
                PercentageChanged(this, e.ProgressPercentage);
            }
        }

        private void panopticonCLI_Completed(object sender, PanopticonCLI.CompletedEventArgs e)
        {
            if (e.Error == null)
            {
                IsComplete = true;
                Info = new PanopticonInfo(panopticonCLI.GridHorizontal,
                                            panopticonCLI.GridVertical,
                                            panopticonCLI.PanelWidth,
                                            panopticonCLI.PanelHeight,
                                            panopticonCLI.OutputWidth,
                                            panopticonCLI.OutputHeight,
                                            panopticonCLI.OutputDuration);

                Timer t = new Timer(0.25);
                t.Elapsed += new ElapsedEventHandler(t_Elapsed);
                t.Start();
            }
            else if(ProcessingFailed != null)
            {
                ProcessingFailed(this, e.Error);
            }
        }

        private void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (File.Exists(FileLocationUtility.GetPathInVideoFolderLocation(Filename)) && ProcessingCompleted != null)
            {
                Timer t = (Timer)sender;
                t.Stop();

                ProcessingCompleted(this);
            }
        }


    }
}
