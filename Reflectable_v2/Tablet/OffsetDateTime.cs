using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tablet
{
    public static class OffsetDateTime
    {
        public static TimeSpan Difference { get; set; }

        public static DateTime GetOffsetTime()
        {
            DateTime now = DateTime.UtcNow;
            return now + Difference;
        }
    }
}
