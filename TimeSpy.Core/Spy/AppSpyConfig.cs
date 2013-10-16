using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeSpy
{
    public class TimeSpyConfig
    {
        public TimeSpan Interval { get; set; }
        public TimeSpan IntervalMouseMove { get; set; }

        public TimeSpyConfig()
        {
            Interval = TimeSpan.FromSeconds(1);
            IntervalMouseMove = TimeSpan.FromSeconds(1);
        }
    }
}
