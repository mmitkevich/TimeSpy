using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using NLog;

namespace TimeSpy
{

    public class WinChromeOmniboxApp : App
    {
        private Logger logger = LogManager.GetCurrentClassLogger();

        public WinChromeOmniboxApp()
        {
            ProcessNamePattern = "chrome";
        }

        public override void GetAppInfo(AppInfo ai)
        {
            bool isMatch=Matches(ai);

            if (isMatch)
            {

                IntPtr mainWnd = User32.FindWindowEx(IntPtr.Zero, IntPtr.Zero, "Chrome_WidgetWin_1", ai.MainWindowTitle);
                IntPtr addrBar = User32.FindWindowEx(mainWnd, IntPtr.Zero, "Chrome_OmniboxView", null);


                if (addrBar != IntPtr.Zero)
                {
                    var WM_GETTEXT = 0xD;
                    var urlsb = new StringBuilder(1024);
                    User32.SendMessage(addrBar, WM_GETTEXT, 1024, urlsb);
                    ai.Set("Url", urlsb.ToString());
                    ai.App = this;
                }
            }
            logger.Info("WinChromeOmniboxApp.GetAppInfo({0},ai:{1},isMatch:{2})", GetType().Name, ai, isMatch);
        }
    }
}
