using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NLog;

namespace TimeSpy
{


    public partial class TimeSpyForm : Form
    {
        private Dictionary<int, TimeNode> _byPid = new Dictionary<int, TimeNode>();

        private Spy _spy;
        private TimeAggregator _agg;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private int _activePid = 0;

        public TimeSpyForm()
        {
            _spy = new Spy(new WinUserActivityHook(),new WinDesktop());
            _agg = new TimeAggregator(_spy);
            InitializeComponent();
        }

        private void TimeSpyForm_Load(object sender, EventArgs e)
        {
            MainTree.Configure(new[]{_agg.Root}, TimeNode.allDetails);

            //_spy.NewEvent += OnNewEvent;
            _agg.NewEvent += OnNewEvent;
            _agg.NodeUpdated += OnNodeUpdated;
            _spy.Start();
        }

        private void OnNewEvent(TimeAggregator sender, SpyEvent se)
        {
            logger.Info(se);
            if (se.EventType == SpyEventType.ActiveAppChanged)
            {
                OnActiveChanged(AppDetails.Pid.Get(se));
            }
        }

        private void OnActiveChanged(int pid)
        {
            _activePid = pid;
           
        }

        private void OnNodeUpdated(TimeAggregator sender, IEnumerable<TimeNode> agns)
        {
            foreach (var n in agns)
                MainTree.RefreshObject(n);
        }
    
    }
}
