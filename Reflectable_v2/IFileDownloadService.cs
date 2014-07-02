using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.IO;

namespace Reflectable_v2
{
    [ServiceContract (Namespace = "urn:FileDownloadService")]
    public interface IFileDownloadService
    {
        [OperationContract]
        Stream DownloadVideo();

        [OperationContract]
        Stream DownloadPanopticonVideo();
    }
}
