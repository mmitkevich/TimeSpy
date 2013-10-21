using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NLog;

namespace TimeSpy
{
    class Tests
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            NLog.Targets.TraceTarget target = new NLog.Targets.TraceTarget();
            target.Layout = "${message}";
            NLog.Config.SimpleConfigurator.ConfigureForTargetLogging(target);

           //AppSpyTest();
            JsonNetTest.Test();
        }

        public static void AppSpyTest()
        {
            var desk = new WinDesktop();
            while (true)
            {
                var ai = desk.GetForegroundApp();
                logger.Info("{0}", ai);
                Thread.Sleep(1000);
            }
        }

    }
}
