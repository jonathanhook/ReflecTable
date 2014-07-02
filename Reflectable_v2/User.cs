using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Media;
using System.Windows.Media;

namespace Reflectable_v2
{
    [DataContract]
    public class User
    {
        [DataMember]
        public int Id { get; private set; }

        [DataMember]
        public Color? Color { get; set; }

        public User(int id, Color? color)
        {
            this.Id = id;
            this.Color = color;
        }
    }
}
