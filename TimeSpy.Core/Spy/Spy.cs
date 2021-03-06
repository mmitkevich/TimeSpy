﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace TimeSpy
{
    public enum SpyEventType : short
    {
        MouseLeftClick = 1,
        MouseRightClick = 2,
        KeyPress = 3,
        ActiveAppChanged = 4,
        MouseWheel = 5,
        MouseMove = 6,
    }

    public class SpyEvent:JObject
    {
        public SpyEventType EventType { get { return this.GetOrSet("EventType",SpyEventType.ActiveAppChanged); } set { this.Set("EventType",value); } }
        public DateTime Time { get { return this.GetOrSet("Time",default(DateTime)); } set { this.Set("Time",value); } }

        public SpyEvent(SpyEventType eventType)
        {
            EventType = eventType;
            Time = DateTime.Now;
        }

        public SpyEvent()
        {
            Time = DateTime.Now;
        }
        public override string ToString()
        {
            return string.Format("T={0},E={1},{2}", Time, EventType, base.ToString());
        }
    }

    public static class AppDetails
    {
        public static Detail<string> Url = new Detail<string>("Url").Set("Width", 100);
        public static Detail<int> Pid = new Detail<int>("Pid").Set("Width", 100);
        public static Detail<int> OldPid = new Detail<int>("OldPid");
        public static Detail<string> ProcessName = new Detail<string>("ProcessName").Set("Width", 100);
        public static Detail<string> MainWindowTitle = new Detail<string>("MainWindowTitle").Set("Width", 200);

        //public static IDetail[] All = new [] { Pid.D, ProcessName.D, MainWindowTitle.D, Url.D };
    }

    public class Spy
    {
        public IUserActivityHook UserHook;
        public IDesktop Desktop;
        public Timer Timer;
        public TimeSpyConfig Config = new TimeSpyConfig();
        
        public AppInfo ForegroundApp = new AppInfo();

        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public event Action<Spy, SpyEvent> NewEvent;
        public DateTime LastMouseMove;

        public Spy(IUserActivityHook userHook, IDesktop desktop)
        {
            UserHook = userHook;
            UserHook.OnMouseActivity += OnMouseActivity;
            UserHook.KeyPress += OnKeyPress;

            Desktop = desktop;
            
            Timer = new Timer();
            Timer.Interval = (int)Config.Interval.TotalMilliseconds;
            Timer.Tick += OnTimerTick;
        }

        protected void RaiseNewEvent(SpyEvent ev)
        {
            if (NewEvent != null)
                NewEvent(this, ev);
        }

        protected virtual void OnMouseActivity(object sender, MouseEventArgs args)
        {
            SpyEventType type = SpyEventType.MouseMove;

            if (args.Button == MouseButtons.Left)
                type = SpyEventType.MouseLeftClick;
            else if (args.Button == MouseButtons.Right)
                type = SpyEventType.MouseRightClick;
            else
            {
                if (args.Delta != 0)
                    type = SpyEventType.MouseWheel;
            }

            if (type != SpyEventType.MouseMove || DateTime.Now.Subtract(LastMouseMove)>Config.IntervalMouseMove)
            {
                LastMouseMove = DateTime.Now;

                var se = new SpyEvent(type);
                se.AsDyn().X = args.X;
                se.AsDyn().Y = args.Y;
                RaiseNewEvent(se);
            }
        }

        protected virtual void OnKeyPress(object sender, KeyPressEventArgs args)
        {
            var se = new SpyEvent(SpyEventType.KeyPress).DoDyn(x=>
                {
                    x.UnicodeCategory = Char.GetUnicodeCategory(args.KeyChar).ToString();
                    x.Layout = InputLanguage.CurrentInputLanguage.LayoutName;
                });

            RaiseNewEvent(se);
        }

        private void AppChanged(AppInfo newApp, AppInfo oldApp)
        {
            var se =  new SpyEvent()
                .MergeInto(newApp).DoDyn(newApp, (x,a) =>
                {
                    x.Type = SpyEventType.ActiveAppChanged;
                    x.Pid = a.Pid;
                    x.ProcessName = a.ProcessName;
                    x.MainWindowTitle = a.MainWindowTitle;
                    x.ActiveWindowTitle = a.ActiveWindowTitle;
                    x.FocusWindowTitle = a.FocusWindowTitle;
                    x.PrevPid = oldApp.Pid;
                });
            RaiseNewEvent(se);
        }

        public void ProcessForegroundApp()
        {
            AppInfo app = Desktop.GetForegroundApp();
            if (null == ForegroundApp && app!=null)
                ForegroundApp = app;
            if (!ForegroundApp.Equals(app))
            {
                var oldApp = ForegroundApp;
                ForegroundApp = app;
                AppChanged(app,oldApp);
            }
        }

        protected virtual void OnTimerTick(object sender, EventArgs args)
        {
            ProcessForegroundApp();
        }

        public void Start()
        {
            try
            {
                //if (UserHook != null)
                //    UserHook.Start();
                Timer.Start();
            }
            catch (Exception e)
            {
                logger.Error("{0}", e);
            }
        }

        public void Stop()
        {
            if (UserHook != null)
                UserHook.Stop();
            Timer.Stop();
        }
    }
}
