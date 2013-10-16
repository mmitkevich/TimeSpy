using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeSpy
{
    class JiraClientTests
    {
        public static void Test()
        {
            JiraClient jc = new JiraClient("http://bugs.lulzex.com");
            var issues = jc.SendRequestAsync(jc.GetUri("search")).Result;
            foreach (var issue in issues)
            {
                Console.WriteLine(issue);
            }
        }
    }
}
