using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Reflectable_v2
{
    [ServiceContract (Namespace = "urn:TableService", CallbackContract=typeof(ICallbackContract))]
    public interface ITableService
    {
        [OperationContract(IsOneWay = true)]
        void Register(User u);

        [OperationContract(IsOneWay = true)]
        void RoundComplete(Round r);

        [OperationContract(IsOneWay = true)]
        void RequestStartRound();

        [OperationContract(IsOneWay = true)]
        void ButtonPress(User u);

        [OperationContract]
        bool IsPanopticonProcessingComplete();

        [OperationContract]
        string GetPanopticonFilename();

        [OperationContract]
        string GetVideoFilename();

        [OperationContract]
        long GetPanopticonSize();

        [OperationContract]
        long GetVideoSize();

        [OperationContract]
        PanopticonInfo GetPanopticonInfo();

        [OperationContract]
        List<Press> GetPresses();

        [OperationContract]
        TimeSpan GetStudyLength();

        [OperationContract(IsOneWay = true)]
        void MakeAnnotation(Annotation a);

        [OperationContract(IsOneWay = true)]
        void SetResearchQuestion(string s);

        [OperationContract]
        DateTime GetTime();
    }
}