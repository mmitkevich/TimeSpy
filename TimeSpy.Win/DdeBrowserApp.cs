using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using System.Diagnostics;
using NDde.Client;

namespace TimeSpy
{
    class DdeBrowserApp : App,IDisposable
    {
        public String DdeServer;

        public String DdeTopic = "WWW_GetWindowInfo";
        public String DdeRequest = "0xFFFFFFFF";

        public int DdeTimeout = 1000;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        private DdeClient _dde;

        public DdeBrowserApp(string server, string procname)
        {
            DdeServer = server;
            ProcessNamePattern = procname;
        }

        public override void GetAppInfo(AppInfo ai)
        {
            if (Matches(ai))
                try
                {
                    if(_dde==null)
                    {
                        var ctx = new NDde.Advanced.DdeContext();
                        ctx.Encoding = Encoding.Default;
                        _dde = new DdeClient(DdeServer, DdeTopic, ctx);
                    }

                    if (!_dde.IsConnected)
                    {
                        _dde.Connect();
                    }
                
                    var str = _dde.Request(DdeRequest, DdeTimeout);
                

                    var items = str.Split(',');

                    char quote = '\"';

                    var url = items.Length > 0 ? items[0].Trim(quote) : String.Empty;

                    ai.Set("Url",url);
                    ai.Set("WindowCaption", items.Length > 1 ? items[1].Trim(quote) : String.Empty);
                    ai.App = this;
                    logger.Debug("DdeBrowserApp.GetAppInfo({0}:{1}:{2}) <<", ai.Pid, ai.ProcessName, ai.MainWindowTitle);
                }
                catch (Exception e)
                {
                    RaiseError(e);
                    logger.Error("Dde conversation to {0} failed {1}", DdeServer, e);
                }
        }

        public void Dispose()
        {
            if (_dde != null)
            {
                try
                {
                    if (_dde.IsConnected)
                        _dde.Disconnect();
                    _dde.Dispose();
                }
                catch (Exception e)
                {
                    logger.Error("{0}", e);
                }
            }
        }

        public override string ToString()
        {
            return base.ToString() + string.Format(",DdeServer={0}", DdeServer);
        }
    }
}
