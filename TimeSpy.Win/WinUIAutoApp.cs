using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Automation;
using System.Diagnostics;
using NLog;
namespace TimeSpy
{
    public class WinUIAutoChromeApp:App
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public WinUIAutoChromeApp()
        {
            ProcessNamePattern = "chrome";
        }

        public override void GetAppInfo(AppInfo ai)
        {
            bool isMatch= Matches(ai);

            if (isMatch)
            {
                string url = "";
                if (GetChromeUrl(ai.MainWindowHandle, out url))
                {
                    ai.App = this;
                    ai.Set("Url", url);
                }
                logger.Info("WinUIAutoChromeApp.GetAppInfo({0},ai:{1},isMatch:{2})", GetType().Name, ai, isMatch);
            }
           
        }

        public static bool GetChromeUrl(IntPtr hwnd, out string url)
        {
            url = "<error hwnd=null>";

            if (hwnd == IntPtr.Zero)
                return false;
            
            AutomationElement element = AutomationElement.FromHandle(hwnd);
            
            url = "<error element==null>";
            if (element == null)
                return false;

            url = FirstValue(element, 1);

            //AutomationElement edit = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit));
            
            //url = "edit";
            //if (edit == null)
            //    return false;
            //url = ((ValuePattern)edit.GetCurrentPattern(ValuePattern.Pattern)).Current.Value as string;
            return true;
        }
        
        private static string ToString(int[] ids)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var id in ids)
                sb.AppendFormat(" {0}", id);
            return sb.ToString();
        }

        private static string FirstValue(AutomationElement parent, int n)
        {
            var children = parent.FindAll(TreeScope.Children, Condition.TrueCondition);
            int r = 0;
            foreach (AutomationElement e in children)
            {
                object vp;
                if (e.TryGetCachedPattern(ValuePattern.Pattern, out vp))
                {
                    var s = ((ValuePattern)vp).Current.Value;
                    logger.Info("n={0} r={1} id={2} v={3} ", n, r, ToString(e.GetRuntimeId()), s);
                    return s;
                }
                else
                {
                    logger.Info("n={0} r={1} id={2} novalue", n, r, ToString(e.GetRuntimeId()));
                }
                if (n > 0)
                {
                    var s = FirstValue(e, n - 1);
                    if (null != s)
                        return s;
                }
                r++;
            }
            return null;
        }
    }
}
