using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Microsoft.Surface.Presentation.Controls;
using Reflectable_v2;

namespace Table
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false)]
    public class TableService : ITableService
    {
        public delegate void TabletRegisteredEventHandler(object sender, ICallbackContract tablet);
        public event TabletRegisteredEventHandler TabletRegistered;

        public delegate void RoundCompletedEventHandler(object sender, Round r);
        public event RoundCompletedEventHandler RoundCompleted;

        public delegate void StartRoundRequestedEventHandler(object sender);
        public event StartRoundRequestedEventHandler StartRoundRequested;

        public delegate void ButtonPressedEventHandler(object sender, User u);
        public event ButtonPressedEventHandler ButtonPressed;

        public TableService()
        {
        }

        public void Register()
        {
            if (TabletRegistered != null)
            {
                TabletRegistered(this, OperationContext.Current.GetCallbackChannel<ICallbackContract>());
            }
        }

        public void RoundComplete(Round r)
        {
            if (RoundCompleted != null)
            {
                RoundCompleted(this, r);
            }
        }

        public void RequestStartRound()
        {
            if (StartRoundRequested != null)
            {
                StartRoundRequested(this);
            }
        }

        public void ButtonPress(User u)
        {
            if (ButtonPressed != null)
            {
                ButtonPressed(this, u);
            }
        }
    }
}
