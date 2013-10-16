using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeSpy
{
    public class TimeNode:DetailedNode<object,TimeNode>
    {
        static TimeNode()
        {
            TDetail.Formatters.Add("Duration", DurationFormat); 
        }
        public static string DurationFormat(object obj)
        {
            var dt = (TimeSpan)obj;
            var sb = new StringBuilder();
            int w = (int)dt.TotalDays/7;
            if (w > 0) sb.AppendFormat("{0}w ", w);
            int d = (int)dt.TotalDays;
            if (d > 0) sb.AppendFormat("{0}d ", d);
            if (w == 0)
            {
                int h = (int)dt.Hours;
                sb.AppendFormat("{0:00}:", h);
                int mi = (int)dt.Minutes;
                sb.AppendFormat("{0:00}", mi);
                if (d==0 && dt.TotalHours < 1)
                {
                    int se = (int)dt.Seconds;
                    sb.AppendFormat(":{0:00}", se);
                    if (dt.TotalSeconds<1)
                    {
                        int ff = (int)dt.Milliseconds;
                        sb.AppendFormat(".{0:000}", ff);
                    }
                }
            }
            return sb.ToString();
        }
        public static string timeFormat = "hh:mm:ss";

        public static Detail<string> ident = new Detail<string>("ID");
        public static Detail<DateTime> beginTime = new Detail<DateTime>("BeginTime").Set("Format", timeFormat);
        public static Detail<DateTime> endTime = new Detail<DateTime>("EndTime").Set("Format", timeFormat);
        public static Detail<TimeSpan> totalTime = new Detail<TimeSpan>("TotalTime").Set("Format","Duration");

        public string Id { get { return ident.Get(this); } }
        public TimeSpan Time { get { return totalTime.Get(this); } set { totalTime.Set(this, value); } }
        public DateTime BeginTime { get { return beginTime.Get(this); } set { beginTime.Set(this, value); } }
        public DateTime EndTime { get { return endTime.Get(this); } set { endTime.Set(this, value); } }

        public static IDetail[] allDetails = new IDetail[] { ident.D, totalTime.D, beginTime.D, endTime.D };

        public override IDetail[] AllDetails(string scheme = null)
        {
            return allDetails;
        }

        public TimeNode[] Path
        {
            get 
            {
                List<TimeNode> p = new List<TimeNode>();
                for (var n = this; n.Parent!= null; n = n.Parent)
                {
                    p.Insert(0, n);
                }
                return p.ToArray();
            }
        }

        public TimeNode With(Action<TimeNode> f)
        {
            f(this);
            return this;
        }

        public List<TimeNode> WalkPath(object[] path, Func<int,TimeNode> create, Action<int, TimeNode> update, object defaultId=null)
        {
            //self 
            var list = new List<TimeNode>();
            update(-1, this);
            list.Add(this);

            var curr = this;
            var offset = 0;
            while (offset < path.Length)
            {
                object id = path[offset]??defaultId;
                if (id != null)
                {
                    //throw new ArgumentNullException(string.Format("id[{0}]", offset));
                    TimeNode next = curr.Children[id];
                    if (next==null)
                    {
                        next = create(offset);
                        curr.Children[id] = next;
                        next.Parent = curr;
                        ident.Set(next, id);
                    }
                    else
                        update(offset,next);
                    curr = next;
                    list.Add(curr);
                }
                offset++;
            }
            return list;
        }
    }

    public class TimeAggregator
    {
        public TimeNode Root = new TimeNode();
        public Spy Spy;
        public TimeNode Last;
        public event Action<TimeAggregator, IEnumerable<TimeNode>> NodeUpdated;
        public event Action<TimeAggregator, SpyEvent> NewEvent;

        public TimeAggregator(Spy spy)
        {
            Spy = spy;
            Last = Root;
            Last.BeginTime = DateTime.Now;
            Spy.NewEvent += OnNewEvent;
            Spy.Timer.Tick += OnTimerTick;
        }
        public void RaiseNodeUpdated(IEnumerable<TimeNode> nodes)
        {
            if (null != NodeUpdated)
                NodeUpdated(this, nodes);
        }

        private static IDetail[] pathTemplate = new IDetail[] { 
                                    AppDetails.ProcessName.D, 
                                        AppDetails.Url.D,
                                            AppDetails.MainWindowTitle.D, 
//                                          AppDetails.Pid.D
        };

        public virtual void OnNewEvent(Spy sender, SpyEvent se)
        {
            if (se.EventType != SpyEventType.ActiveAppChanged)
                return;

            var now = DateTime.Now;

            var path = pathTemplate.Select(p => se.Get(p.Name)).ToArray();

            var changed = Aggregate(path, now);

            RaiseNodeUpdated(new[]{changed[0]});

            if (null != NewEvent)
                NewEvent(this, se);
        }

        public virtual void OnTimerTick(object sender, EventArgs args)
        {
            var changed = Aggregate(Last.Path.Select(x=>x.Id).ToArray(), DateTime.Now);
            if(changed.Count>0)
                RaiseNodeUpdated(new []{ changed[0] });            
        }

        public List<TimeNode> Aggregate(object[] path, DateTime now)
        {
            TimeNode newNode = null;
            var lastPath = Last.Path.Select(x=>x.Id).ToArray();
            var changed = Root.WalkPath(path,
                i =>
                {
                    var n = new TimeNode();
                    n.BeginTime = n.EndTime = now;
                    newNode = newNode ?? n;
                    i++;
                    return n;
                },
                (i, node) =>
                {
                    if (i < 0) return;
                    if (i >= lastPath.Length || !object.Equals(node.Id, lastPath[i]))
                    {
                        //switched
                        node.BeginTime = node.EndTime = now;
                    } // else n==lastPath[level]
                    node.Time += now - node.EndTime;
                    node.EndTime = now;
                });
            
            Last = changed.Last();

            int first = changed.IndexOf(newNode);
            if (first >= 2)
                changed.RemoveRange(0, first - 1);    // x,x,p,n,y,y -> p,n,y,y
            return changed;
        }
    }
}
