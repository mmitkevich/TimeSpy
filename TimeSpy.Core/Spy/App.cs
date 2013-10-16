using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Diagnostics;
using NLog;
using System.Windows.Forms;
using System.Drawing;

namespace TimeSpy
{
    public abstract class App
    {
        public String ProcessNamePattern = "";

        public List<App> Derived = new List<App>();

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public virtual void RaiseError(Exception e)
        {
            logger.Fatal(e);
         //   throw e;
        }

        public virtual bool Matches(AppInfo ai)
        {
            if (ProcessNamePattern.Length == 0)
                return true;

            if (ai.ProcessName == null || ai.ProcessName.Length == 0)
                return false;

            return ai.ProcessName.IndexOf(ProcessNamePattern) > -1;
        }

        public virtual void GetAppInfo(AppInfo ai)
        {
            foreach (var app in Derived)
                app.GetAppInfo(ai);
        }
    }

    public class AppInfo:Detailed
    {
        // template who filled this AppInfo
        public App App;

        // process id and name
        public String ProcessName;
        public int Pid;

        // process' main window 
        public IntPtr MainWindowHandle;
        public String MainWindowTitle;

        // foreground (active) window
        public IntPtr ActiveWindowHandle;
        public String ActiveWindowTitle;
        public Rectangle ActiveWindowRect;

        // keyboard focus handling window
        public IntPtr FocusWindowHandle;
        public String FocusWindowTitle;
        // parent process main window
        public IntPtr ParentWindowHandle;
        public String ParentWindowTitle;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("pid={0}:'{1}',mainwnd=0x{2:X}:'{3}'", Pid, ProcessName, (int)MainWindowHandle, MainWindowTitle);
            if(ActiveWindowHandle!=IntPtr.Zero && ActiveWindowHandle!=MainWindowHandle)
                sb.AppendFormat(",active=0x{0:X}:'{1}'",(int)ActiveWindowHandle,ActiveWindowTitle);
            if(FocusWindowHandle!=IntPtr.Zero && FocusWindowHandle!=ActiveWindowHandle)
                sb.AppendFormat(",focused=0x{0:X}:'{1}'", (int)FocusWindowHandle, FocusWindowTitle);

            sb.AppendFormat(",{0}", base.ToString());
            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            AppInfo ai = obj as AppInfo;
            if (ai == null)
                return object.ReferenceEquals(this, obj);
            return Pid == ai.Pid;
        }
    }

    public interface IDesktop
    {
        AppInfo GetForegroundApp();
    }

    public interface IUserActivityHook
    {
        event MouseEventHandler OnMouseActivity;
        event KeyEventHandler KeyDown;
        event KeyPressEventHandler KeyPress;
        event KeyEventHandler KeyUp;

        void Start();
        void Stop();
    }

}
