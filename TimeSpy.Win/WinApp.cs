using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using System.Diagnostics;

namespace TimeSpy
{
    public class WinApp : App
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

       

        private static String GetWindowText(IntPtr hwnd)
        {
            if (hwnd == null)
                return "";
            var buf = new StringBuilder(4096);
            User32.SendMessage(hwnd, 0xD, 4096, buf);
            return buf.ToString();
        }

        private IntPtr GetFocusedWindowHandle(IntPtr hwnd)
        {
            User32.GUITHREADINFO gti;
            User32.GetInfo(hwnd, out gti);
            //var s = string.Format("GUITHREADINFO 0x{0:x} f={1:X} a={2:X} ct={3:X} cp={4:X} mo={5:X}", hwnd, gti.hwndFocus, gti.hwndActive, gti.hwndCaret, gti.hwndCapture, gti.hwndMenuOwner);
            //logger.Info(s);
            return gti.hwndFocus;
        }

        public override void GetAppInfo(AppInfo ai)
        {
            ai.ActiveWindowHandle = User32.GetForegroundWindow();// pr.MainWindowHandle;
            ai.ActiveWindowTitle = GetWindowText(ai.ActiveWindowHandle); //pr.MainWindowTitle;

            User32.RECT rc;
            User32.GetWindowRect(ai.ActiveWindowHandle, out rc);

            var pr = User32.GetWindowProcess(ai.ActiveWindowHandle);
            ai.Pid = pr.Id;
            ai.ProcessName = pr.ProcessName;
            ai.MainWindowHandle = pr.MainWindowHandle;
            ai.MainWindowTitle = pr.MainWindowTitle;
            ai.FocusWindowTitle = "";

            if (ai.ActiveWindowHandle != IntPtr.Zero)
            {
                ai.FocusWindowHandle = GetFocusedWindowHandle(ai.ActiveWindowHandle);
                ai.FocusWindowTitle = GetWindowText(ai.FocusWindowHandle);
            }

            var pp = pr.GetParentProcess();

            if(pp!=null)
            {
                ai.ParentWindowHandle = pp.MainWindowHandle;
                ai.ParentWindowTitle = pp.MainWindowTitle;
            }
            
            bool isMatch = Matches(ai);

            logger.Info("WinApp.GetAppInfo({0},ai:{1},isMatch:{2})", GetType().Name, ai, isMatch);

            if (isMatch)
            {
                ai.App = this;
                base.GetAppInfo(ai);
            }
        }
    }

   

    public class WinDesktop : App, IDesktop
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public WinApp GuiApp;

        public WinDesktop()
        {
            Derived.Add(GuiApp = new WinApp());

            // http://wiki.tcl.tk/14977
            GuiApp.Derived.AddRange(new App[]{
                new DdeBrowserApp("iexplore", "iexplore"),
                new DdeBrowserApp("firefox", "firefox"),
                new DdeBrowserApp("netscape", "netscape"),
                new DdeBrowserApp("mosaic", "mosaic"),
                new DdeBrowserApp("netscp6", "netscp6"),
                new DdeBrowserApp("opera", "opera"),
                //new WinChromeOmniboxApp(),
                new WinUIAutoChromeApp()
            });
        }

        public AppInfo GetForegroundApp()
        {
            AppInfo ai = new AppInfo();
            GetAppInfo(ai);
            return ai;
        }
    }

}
