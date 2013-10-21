using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

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
        public Spy Spy;

        public JObject Root = new JObject();
        public JObject Last;

        public string GroupBy = "ProcessName,Url,MainWindowTitle";

        public event Action<TimeAggregator, JObject> NodeInserted;
        public event Action<TimeAggregator, JObject> NodeUpdated;
        public event Action<TimeAggregator, SpyEvent> NewEvent;

        public TimeAggregator(Spy spy)
        {
            Spy = spy;
            Last = Root;
            Last.DoDyn(x=> x.BeginTime = DateTime.Now);

            Spy.NewEvent += OnNewEvent;
            Spy.Timer.Tick += OnTimerTick;
        }

        public void RaiseNodeInserted(JObject node)
        {
            if (null != NodeInserted)
                NodeInserted(this, node);
        }

        public void RaiseNodeUpdated(JObject node)
        {
            if (null != NodeUpdated)
                NodeUpdated(this, node);
        }

        public virtual void OnNewEvent(Spy sender, SpyEvent se)
        {
            if (se.EventType != SpyEventType.ActiveAppChanged)
                return;

            var now = DateTime.Now;

            if (null != NewEvent)
                NewEvent(this, se);

            Last = Aggregate(GroupBy.Split(',').Select(g=>se.Get(g,string.Empty)).Where(s=>s.Length>0).ToArray(), now);

        }

       
        public virtual void OnTimerTick(object sender, EventArgs args)
        {
            Aggregate(Last.GetAncestorObjects().Select((dynamic x) => (string)x.Id).ToArray(), 
                DateTime.Now);
        }

        public JObject Aggregate(object[] pathItems, DateTime now)
        {
            JObject createdNode = null;
            JObject updatedNode = null;

            var lastAncestors = Last.GetAncestorObjects();
            var result = Root.WalkPath(pathItems.Length,
                    (ofs,parent) => parent.GetChild(pathItems[ofs]),
                    (ofs,parent) => parent.GetOrAddChild(pathItems[ofs], 
                        id => new JObject().DoGet(x =>
                            {
                                x["Id"] = (string)pathItems[ofs];
                                x["BeginTime"] = x["EndTime"] = now;
                                x["TotalTime"] = TimeSpan.Zero;
                                createdNode = createdNode ?? x;
                                return x;
                            }
                            /*.DoDyn(
                            x =>
                            {
                                x.Id = pathItems[ofs];
                                x.BeginTime = now;
                                x.EndTime = now;
                                x.TotalTime = TimeSpan.Zero;
                                createdNode = createdNode ?? x;
                            }*/
                        )
                    ),
                    (ofs,node) =>
                        {
                            if (ofs >= lastAncestors.Count || lastAncestors[ofs] != node)
                                //node.DoDyn(x => { x.BeginTime = x.EndTime = now; }); // switched
                                node["BeginTime"]=node["EndTime"]=now;
                            else
                            {
                                node["TotalTime"] = node.Get("TotalTime", TimeSpan.Zero) +
                                                    (now - node.Get("EndTime", now));
                                node["EndTime"] = now;
                                /* node.DoDyn(x =>
                                    {
                                        // add
                                        x.TotalTime = ((TimeSpan) x.TotalTime + (now - (DateTime) x.EndTime));
                                        x.EndTime = now;
                                    }
                                    );*/
                            }
                            updatedNode = updatedNode ?? node;
                        }
                );
            if (null != createdNode)
                RaiseNodeInserted(createdNode);
            if (null != updatedNode)
                RaiseNodeUpdated(updatedNode);
            return result;
        }
    }
}
