using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeSpy
{
    public struct DateTimeRange
    {
        DateTime Begin;
        DateTime End;
    }

    public class SpyStat
    {
        public AppInfo App;

        public DateTimeRange Interval;

        public Dictionary<string, int> Events = new Dictionary<string, int>();
    }
}
