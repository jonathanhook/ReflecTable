using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Reflectable_v2
{
    [DataContract]
    public class Annotation
    {
        [DataMember]
        public User User { get; private set; }

        [DataMember]
        public string Comment { get; private set; }

        [DataMember]
        public Press Press { get; private set; }

        public Annotation(User user, string comment, Press press)
        {
            this.User = user;
            this.Comment = comment;
            this.Press = press;
        }
    }
}
