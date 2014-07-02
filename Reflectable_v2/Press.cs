using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Reflectable_v2
{
    [DataContract]
    public class Press
    {
        [DataMember]
        public Guid Id { get; private set; }

        [DataMember]
        public TimeSpan End { get; private set; }

        [DataMember]
        public TimeSpan Start { get; private set; }
        
        [DataMember]
        public User User { get; private set; }

        public Press(TimeSpan start, TimeSpan end, User user)
        {
            this.Start = start;
            this.End = end;
            this.User = user;

            Id = Guid.NewGuid();
        }
    }
}
