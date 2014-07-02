using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Reflectable_v2
{
    [DataContract]
    public class PanopticonInfo
    {
        [DataMember]
        public int GridWidth { get; private set; }

        [DataMember]
        public int GridHeight { get; private set; }

        [DataMember]
        public int PanelWidth { get; private set; }

        [DataMember]
        public int PanelHeight { get; private set; }

        [DataMember]
        public int Width { get; private set; }

        [DataMember]
        public int Height { get; private set; }

        [DataMember]
        public float DurationMs { get; private set; }

        public PanopticonInfo(int gridWidth, int gridHeight, int panelWidth, int panelHeight, int width, int height, float durationMs)
        {
            this.GridWidth = gridWidth;
            this.GridHeight = gridHeight;
            this.PanelWidth = panelWidth;
            this.PanelHeight = panelHeight;
            this.Width = width;
            this.Height = height;
            this.DurationMs = durationMs;
        }

        public PanopticonInfo()
        {

        }
    }
}
