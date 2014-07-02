using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Expression.Encoder.Devices;
using Microsoft.Expression.Encoder.Live;
using System.IO;
using Microsoft.Expression.Encoder;
using System.Threading;
using System.Windows;
using System.Net;
using System.Diagnostics;
using Reflectable_v2;

namespace Table
{
    public class CameraCapture
    {
        private const string VIDEO_FILE = "{0}.wmv";

        public List<EncoderDevice> AudioSources { get; private set; }
        public List<EncoderDevice> VideoSources { get; private set; }
        public bool IsCapturing { get; private set; }
        public EncoderDevice SelectedVideoDevice { get; private set; }
        public EncoderDevice SelectedAudioDevice { get; private set; }
        public string VideoFileName { get; private set; }
        public TimeSpan Length { get; private set; }

        private LiveDeviceSource dvs;
        private LiveJob job;
        private DateTime started;

        public CameraCapture()
        {
            IsCapturing = false;

            FindDevices();
            LoadDefaultDevices();
        }
       
        public void SelectAudioDeviceByName(string name)
        {
            EncoderDevice device = AudioSources.FirstOrDefault(d => d.Name == name);
            if (device != null)
            {
                SelectedAudioDevice = device;
            }
            else
            {
                SelectedAudioDevice = AudioSources[0];
            }
        }

        public void SelectVideoDeviceByName(string name)
        {
            EncoderDevice device = VideoSources.FirstOrDefault(d => d.Name == name);
            if (device != null)
            {
                SelectedVideoDevice = device;
            }
            else
            {
                SelectedVideoDevice = VideoSources[0];
            }
        }

        public void SaveDefaultDevices()
        {
            Properties.Settings.Default.AudioDevice = SelectedAudioDevice.Name;
            Properties.Settings.Default.VideoDevice = SelectedVideoDevice.Name;
            Properties.Settings.Default.Save();
        }

        private void FindDevices()
        {
            int numVideoDevices = EncoderDevices.FindDevices(EncoderDeviceType.Video).Count;
            if (numVideoDevices > 0)
            {
                VideoSources = new List<EncoderDevice>(EncoderDevices.FindDevices(EncoderDeviceType.Video));
            }

            int numAudioDevices = EncoderDevices.FindDevices(EncoderDeviceType.Audio).Count;
            if (numAudioDevices > 0)
            {
                AudioSources = new List<EncoderDevice>(EncoderDevices.FindDevices(EncoderDeviceType.Audio));
            }
        }

        private double GetUnixEpoch(DateTime dt)
        {
            TimeSpan t = (dt - new DateTime(1970, 1, 1));
            return Math.Round(t.TotalMilliseconds);
        }

        private void LoadDefaultDevices()
        {
            SelectVideoDeviceByName(Properties.Settings.Default.VideoDevice);
            SelectAudioDeviceByName(Properties.Settings.Default.AudioDevice);
        }

        public void StartCapture()
        {
            if (!IsCapturing)
            {
                job = new LiveJob();
                dvs = job.AddDeviceSource(SelectedVideoDevice, SelectedAudioDevice);
                job.ActivateSource(dvs);
                job.ApplyPreset(LivePresets.VC1HighSpeedBroadband4x3);

                double epoch = GetUnixEpoch(DateTime.UtcNow);
                string timestamp = epoch.ToString();
                VideoFileName = string.Format(VIDEO_FILE, timestamp);

                FileArchivePublishFormat fileOut = new FileArchivePublishFormat();
                fileOut.OutputFileName = Path.Combine(FileLocationUtility.GetVideoFolderLoctation(), VideoFileName);

                job.PublishFormats.Add(fileOut);
                job.StartEncoding();

                started = DateTime.UtcNow;
                IsCapturing = true;
            }
        }

        public void StopCapture()
        {
            if (IsCapturing)
            {
                job.StopEncoding();
                job.RemoveDeviceSource(dvs);

                IsCapturing = false;
                Length = DateTime.UtcNow - started;
            }
        }
    }
}
