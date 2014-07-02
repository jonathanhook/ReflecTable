using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Reflectable_v2;
using System.IO;
using System.ServiceModel.Channels;
using System.Windows;

namespace Table
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false)]
    class FileDownloadService : IFileDownloadService
    {
        public string VideoFile { get; set; }
        public string PanopticonVideoFile { get; set; }

        private ServiceHost serviceHost;

        public FileDownloadService()
        {
            TcpTransportBindingElement transport = new TcpTransportBindingElement();
            transport.MaxReceivedMessageSize = long.MaxValue;
            transport.MaxBufferSize = int.MaxValue;
            transport.MaxBufferPoolSize = long.MaxValue;
            transport.TransferMode = TransferMode.Streamed;

            BinaryMessageEncodingBindingElement encoder = new BinaryMessageEncodingBindingElement();
            CustomBinding binding = new CustomBinding(encoder, transport);
            binding.ReceiveTimeout = TimeSpan.MaxValue;
            binding.SendTimeout = TimeSpan.MaxValue;
            binding.CloseTimeout = TimeSpan.MaxValue;

            serviceHost = new ServiceHost(this);
            serviceHost.AddServiceEndpoint(typeof(IFileDownloadService), binding, "net.tcp://localhost:8020");
            serviceHost.Open();
        }

        public void Close()
        {
            serviceHost.Close();
        }

        public Stream DownloadVideo()
        {
            try
            {
                string path = FileLocationUtility.GetPathInVideoFolderLocation(VideoFile);
                Stream s = File.OpenRead(path);
                return s;
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message + "\r\n" + e.StackTrace));
            }
        }

        public Stream DownloadPanopticonVideo()
        {
            try
            {
                string path = FileLocationUtility.GetPathInVideoFolderLocation(PanopticonVideoFile);
                Stream s = File.OpenRead(path);
                return s;
            }
            catch (Exception e)
            {
                throw new FaultException(new FaultReason(e.Message + "\r\n" + e.StackTrace));
            }
        }
    }
}
