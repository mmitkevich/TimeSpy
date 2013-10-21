using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NLog;
using Newtonsoft.Json.Linq;

namespace TimeSpy
{

    public partial class TimeSpyForm : Form, ReactiveUI.IViewFor<TimeSpyViewModel>
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public TimeSpyViewModel ViewModel { get; set; }

        public TimeSpyForm(TimeSpyViewModel model)
        {
            ViewModel = model;
            InitializeComponent();
        }

        private void TimeSpyForm_Load(object sender, EventArgs e)
        {
            ViewModel.Connect(this);
        }

       object ReactiveUI.IViewFor.ViewModel
        {
            get { return ViewModel; }
            set
            {
                throw new NotImplementedException();
            }
        }

       private void toolStripButton1_Click(object sender, EventArgs e)
       {

       }
    }

    public class TimeSpyViewModel:ReactiveUI.ReactiveObject
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public TimeAggregator Agg;
        public JsonTreeViewModel AggTree;

        public TimeSpyForm View;

        public TimeSpyViewModel(TimeAggregator agg)
        {
            Agg = agg;
            AggTree = new JsonTreeViewModel(Agg.Root);
        }

        private string[] columns = new[] { "Id", "TotalTime", "BeginTime", "EndTime" };

        public void Connect(TimeSpyForm form)
        {
            View = form;

            AggTree.Connect(View.TreeView, x => columns);

            //_spy.NewEvent += OnNewEvent;
            Agg.NewEvent += OnNewEvent;
            Agg.NodeInserted += OnNodeInserted;
            Agg.NodeUpdated += OnNodeUpdated;
        }

        protected virtual void OnNewEvent(TimeAggregator sender, SpyEvent se)
        {
            logger.Info(se);
            if (se.EventType == SpyEventType.ActiveAppChanged)
            {
                
            }
        }

        protected virtual void OnActiveChanged(int pid)
        {

        }

        protected virtual void OnNodeInserted(TimeAggregator sender, JObject node)
        {
            var p = node.GetParentObject();
            if(p==Agg.Root)
                View.TreeView.AddObject(node);
            else
                View.TreeView.RefreshObject(p);
        }

        protected virtual void OnNodeUpdated(TimeAggregator sender, JObject node)
        {
            View.TreeView.RefreshObject(node);
        }
    }
}
