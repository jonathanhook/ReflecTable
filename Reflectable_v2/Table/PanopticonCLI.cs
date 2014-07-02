// Panopticon Command-Line-Interface Wrapper
// Dan Jackson, 2012

/*
    // *** EXAMPLE ****
 
    // Create the Panopticon command-line-interface wrapper
    PanopticonCLI panopticonCLI = new PanopticonCLI(@"..\..\..\PanopticonCLI\bin\panopticon-cli.exe");

    // Optional handler for progress updates
    panopticonCLI.ProgressChanged += (s,e) =>
    {
        Console.WriteLine("Progress: " + e.ProgressPercentage);
    };

    // Handler for completion (optional if not using asynchronous conversion)
    panopticonCLI.Completed += (s,e) =>
    {
        if (!e.Result)
        {
            Console.WriteLine("Error: " + e.Error);
        }
        else
        {
            Console.WriteLine("Completed, using video: " + panopticonCLI.OutputFile);
            Console.WriteLine("Grid: " + panopticonCLI.GridHorizontal + "+1 x " + panopticonCLI.GridVertical);
            Console.WriteLine("Interval: " + panopticonCLI.OutputDuration);
            Console.WriteLine("Full duration: " + panopticonCLI.InputDuration);
        }
    };

    // Begin background conversion
    panopticonCLI.ConvertAsync(inputVideo);
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;

namespace PanopticonCS
{
    public class PanopticonCLI : IDisposable
    {
        // Constants
        public const string DEFAULT_EXECUTABLE = "panopticon-cli.exe";

        // For clean up if destroyed
        private IList<Process> processList = new List<Process>();

        // Raw properties (should not need to access)
        public string Executable { get; protected set; }
        public int ExitCode { get; protected set; }
        public IDictionary<string, string> Results { get; protected set; }
        public string InputFile { get; protected set; }
        public string OutputFile { get; protected set; }

        // Status
        public bool IsBusy { get; protected set; }
        public bool Converted { get; protected set; }
        public Exception Error { get; protected set; }

        // Properties
        public int InputWidth { get; protected set; }
        public int InputHeight { get; protected set; }
        public float InputDuration { get; protected set; }
        public int OutputWidth { get; protected set; }
        public int OutputHeight { get; protected set; }
        public float OutputFps { get; protected set; }
        public float OutputDuration { get; protected set; }
        public int GridHorizontal { get; protected set; }
        public int GridVertical { get; protected set; }
        public int PanelWidth { get; protected set; }
        public int PanelHeight { get; protected set; }
        public int FrameCount { get; protected set; }


        // ProgressChanged Event
        public class ProgressChangedEventArgs : EventArgs
        {
            public ProgressChangedEventArgs(int progressPercentage)
            {
                ProgressPercentage = progressPercentage;
            }
            public int ProgressPercentage { get; protected set; }
        }
        public delegate void ProgressChangedEventHandler(object sender, ProgressChangedEventArgs e);
        public event ProgressChangedEventHandler ProgressChanged;

        // Completed Event
        public class CompletedEventArgs : EventArgs
        {
            public CompletedEventArgs(Exception error, bool result)
            {
                Error = error;
                Result = result;
            }
            public Exception Error { get; protected set; }
            public bool Result { get; protected set; }
        }
        public delegate void CompletedEventHandler(object sender, CompletedEventArgs e);
        public event CompletedEventHandler Completed;


        public PanopticonCLI(string executable = DEFAULT_EXECUTABLE)
        {
            InputFile = "";
            OutputFile = "";
            Executable = executable;
            ExitCode = int.MinValue;
            Results = new Dictionary<string, string>();
        }

        ~PanopticonCLI()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Cancel()
        {
            foreach (Process process in processList)
            {
                try
                {
                    if (!process.HasExited)
                    {
                        process.Kill();
                    }
                }
                catch { ; }
            }
            processList.Clear();
        }

        protected virtual void Dispose(bool disposing)
        {
            Cancel();
        }

        private void ParseLine(string line)
        {
            int sep = line.IndexOf('=');
            if (sep >= 0)
            {
                string name = line.Substring(0, sep);
                string value = line.Substring(sep + 1);
                Results[name] = value;

                // Progress
                int percent = -1;

                if (name == "%L" && int.TryParse(value, out percent))
                {
                    percent = 0 + (percent / 2);
                }
                else if (name == "%S" && int.TryParse(value, out percent))
                {
                    percent = 50 + (percent / 2);
                }

                if (percent >= 0 && ProgressChanged != null)
                {
                    ProgressChanged.Invoke(this, new ProgressChangedEventArgs(percent));
                }

            }
            return;
        }

        private float GetResultFloat(string key)
        {
            float value = 0;
            if (Results.ContainsKey(key)) { float.TryParse(Results[key], out value); }
            return value;
        }

        private int GetResultInt(string key)
        {
            return (int)GetResultFloat(key);
        }

        public bool ConvertSync(string inputVideo, string outputVideo = null, string args = "")
        {
            try
            {
                InputFile = inputVideo;
                ExitCode = int.MinValue;
                Results.Clear();
                IsBusy = true;
                Converted = false;
                Error = null;

                // If no output file specified
                if (outputVideo == null)
                {
                    //outputVideo = Path.ChangeExtension(Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString()), ".wmv");
                    outputVideo = Path.ChangeExtension(InputFile, ".pan.wmv");
                }
                OutputFile = outputVideo;

                // Check for an output file that's at least as recent as the input file
                if (File.Exists(OutputFile) && File.GetLastWriteTime(OutputFile) >= File.GetLastWriteTime(InputFile))
                {
                    // Check for a properties file that's at least as recent as the output file
                    string propertiesFile = Path.ChangeExtension(OutputFile, ".txt");
                    if (File.Exists(propertiesFile) && File.GetLastWriteTime(propertiesFile) >= File.GetLastWriteTime(OutputFile))
                    {
                        string[] propertiesLines = File.ReadAllLines(propertiesFile);
                        foreach (string line in propertiesLines)
                        {
                            ParseLine(line);
                        }
                    }
                }

                if (!Results.ContainsKey("_exit") || Results["_exit"] != "0")
                {
                    // Check if the executable exists
                    if (!File.Exists(Executable))
                    {
                        IsBusy = false;
                        Exception ex = new Exception("Conversion executable not found: " + Executable);
                        Error = ex;
                        throw ex;
                    }

                    // Create the process structure
                    ProcessStartInfo processInformation = new ProcessStartInfo();
                    processInformation.FileName = Executable;
                    processInformation.Arguments = "\"" + inputVideo + "\" -out \"" + outputVideo + "\" " + args;
                    processInformation.UseShellExecute = false;
                    processInformation.WorkingDirectory = Directory.GetCurrentDirectory();
                    processInformation.CreateNoWindow = true;
                    processInformation.RedirectStandardError = true;
                    processInformation.RedirectStandardOutput = true;

                    // Create process
                    Process process = new Process();
                    process.EnableRaisingEvents = true;
                    process.StartInfo = processInformation;

                    // For clean-up if we're disposed
                    processList.Add(process);
                    process.Exited += (sender, e) => processList.Remove(process);

                    // Start process
                    try
                    {
                        process.Start();
                    }
                    catch (Exception e)
                    {
                        IsBusy = false;
                        Exception ex = new Exception("Process exception.", e);
                        Error = ex;
                        throw ex;
                    }

                    // Parse output
                    while (!process.StandardOutput.EndOfStream)
                    {
                        string line;
                        line = process.StandardOutput.ReadLine();
                        if (line == null) { break; }
                        ParseLine(line);
                    }

                    // Check exit code
                    process.WaitForExit();
                    Results["_exit"] = process.ExitCode.ToString();

                    // Write properties file to go with the output file
                    string propertiesFile = Path.ChangeExtension(OutputFile, ".txt");
                    List<string> propertiesLines = new List<string>();
                    foreach (KeyValuePair<string, string> kvp in Results)
                    {
                        propertiesLines.Add(kvp.Key + "=" + kvp.Value);
                    }
                    File.WriteAllLines(propertiesFile, propertiesLines.ToArray());

                }

                if (Results.ContainsKey("_exit")) { int e = ExitCode; int.TryParse(Results["_exit"], out e); ExitCode = e; }
                Converted = (ExitCode == 0);
                IsBusy = false;

                InputWidth = GetResultInt("inX");
                InputHeight = GetResultInt("inY");
                InputDuration = GetResultFloat("inDuration");
                OutputWidth = GetResultInt("outX");
                OutputHeight = GetResultInt("outY");
                OutputFps = GetResultFloat("outFps");
                OutputDuration = GetResultFloat("outDuration");
                GridHorizontal = GetResultInt("gridW");
                GridVertical = GetResultInt("gridH");
                PanelWidth = GetResultInt("panelW");
                PanelHeight = GetResultInt("panelH");
                FrameCount = GetResultInt("frameCount");

                if (Completed != null)
                {
                    Completed.Invoke(this, new CompletedEventArgs(null, Converted));
                }

                return Converted;
            }
            catch (Exception e)
            {
                if (Completed != null)
                {
                    Error = e;
                    Completed.Invoke(this, new CompletedEventArgs(e, false));
                }
                throw e;
            }
        }


        public void ConvertAsync(string inputVideo, string outputVideo = null, string args = "")
        {
            Thread thread = new Thread(() => {
                try
                {
                    ConvertSync(inputVideo, outputVideo, args);
                }
                catch { ; }     // Swallow any re-thrown (ConvertSync will have called 'Completed' call-back with exception)
            });
            thread.Start();
        }


        // Video time at the specified position (coordinates are proportional 0-1)
        public float TimeAtPoint(float x, float y)
        {
            if (GridHorizontal <= 0 || GridVertical <= 0 || InputDuration <= 0 || OutputDuration <= 0 || x < 0.0f || x >= 1.0f || y < 0.0f || y >= 1.0f) { return 0.0f; }

            int panelY;

            float rowTime = (float)InputDuration / GridVertical;                    // time in 1 row
            panelY = (int)(y * GridVertical);                                       // row

            float ofs = x * ((float)(GridHorizontal + 1) / GridHorizontal);         // adjust for extra 'wrap' column

            float t = (panelY * rowTime) + (ofs * rowTime);                         // time at point

            return t;
        }

        // Panel row/column and time at the specified position and playback time (coordinates are proportional 0-1)
        public float PanelTimeAtPoint(float x, float y, float videoPosition, out int panelX, out int panelY)
        {
            if (GridHorizontal <= 0 || GridVertical <= 0 || InputDuration <= 0 || OutputDuration <= 0 || x < 0.0f || x >= 1.0f || y < 0.0f || y >= 1.0f) { panelX = -1; panelY = -1; return 0.0f; }

            float rowTime = (float)InputDuration / GridVertical;                    // time in 1 row
            panelY = (int)(y * GridVertical);                                       // row

            x -= videoPosition * (1.0f / (GridHorizontal + 1)) / OutputDuration;    // account for video playback position
            panelX = (int)(x * (GridHorizontal + 1));                               // column
            if (x < 0) { panelX = -1; }

            float t = (panelY * rowTime) + (panelX * rowTime / (GridHorizontal + 1)) + videoPosition; // time at panel's left edge
            return t;
        }

    }
}
