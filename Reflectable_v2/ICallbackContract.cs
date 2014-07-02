using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Windows.Media;

namespace Reflectable_v2
{
    public interface ICallbackContract
    {
        [OperationContract(IsOneWay = true)]
        void SetRound(Round r);

        [OperationContract(IsOneWay = true)]
        void StartRound(DateTime startTime);

        [OperationContract(IsOneWay = true)]
        void PanopticonCompleted();

        [OperationContract(IsOneWay = true)]
        void PanopticonPercentageChanged(int percentage);

        [OperationContract(IsOneWay = true)]
        void SetUserColor(Color c);
    }
}
