using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Reflectable_v2
{
    [DataContract]
    public class Round
    {
        public const int NUM_ROUNDS = 8;

        public enum RoundId : int
        {
            PAPER_DIVERGE = 0,
            PAPER_CONVERGE = 1,
            PAPER_TRANSCEND = 2,
            VIDEO_BROWSE = 3,
            RESEARCH_QUESTION = 4,
            VIDEO_DIVERGE = 5,
            VIDEO_CONVERGE = 6,
            COMPLETE = 7
        }

        [DataMember]
        public RoundId Id { get; private set; }

        [DataMember]
        public int Seq { get; private set; }

        [DataMember]
        public TimeSpan Length { get; private set; }

        [DataMember]
        public string Instructions { get; private set; }

        public Round(RoundId id, int seq, TimeSpan length, string Instructions)
        {
            this.Id = id;
            this.Seq = seq;
            this.Length = length;
            this.Instructions = Instructions;
        }
    }
}
