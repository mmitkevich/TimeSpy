#define TESTS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TimeSpy
{
    public class AppFactory
    {
        public static IDesktop MakeDesktop()
        {
            return new WinDesktop();
        }

        public static TimeSpyViewModel MakeModel()
        {
            var spy = new Spy(new WinUserActivityHook(), new WinDesktop());
            var model = new TimeSpyViewModel(new TimeAggregator(spy));
            spy.Start();
            return model;
        }
    }


    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            NLog.Targets.TraceTarget target = new NLog.Targets.TraceTarget();
            target.Layout = "${time} ${message}";
            NLog.Config.SimpleConfigurator.ConfigureForTargetLogging(target);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TimeSpyForm(AppFactory.MakeModel()));
        }
    }
}
