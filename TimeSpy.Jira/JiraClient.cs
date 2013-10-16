using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TimeSpy
{
    class JiraIssue:DetailedNode<string,JiraIssue>
    {
   
    }

    static class JiraExtensions
    {
        public static IEnumerable<JiraIssue> ToJiraIssues(this JObject obj)
        {
            return obj["issues"].Select(j => ToJiraIssue(j));
        }

        public static JiraIssue ToJiraIssue(this JToken j)
        {
            JObject obj = (JObject)j;
            if (obj == null)
                throw new InvalidOperationException(string.Format("not json object {0}",j));
            var issue = new JiraIssue();
            foreach(var prop in obj.Properties())
            {
                switch (prop.Type)
                {
                    case JTokenType.Array:
                        continue;
                    case JTokenType.Object:
                        continue;
                    default:
                        issue[prop.Name] = prop.Value.ToString();
                        break;
                }
            }
            return issue;
        }
    }

    public class JiraClient
    {
        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static string ApiVersion = "2";
        public HttpClient HttpClient;
        public Uri JiraUri;
        public JiraClient(string baseUrl)
        {
            JiraUri = new Uri(baseUrl);
            HttpClient = new HttpClient();
        }


        public Uri GetUri(string path,string query = null)
        {
            var ub = new UriBuilder(JiraUri)
            {
                Path = string.Format("/rest/api/{0}/{1}",ApiVersion,path)
            };
        
            if (query != null)
                ub.Query = query;

            return ub.Uri;            
        }

        public Task<JObject> SendRequestAsync(Uri uri)
        {
            logger.Info("SearchIssuesAsync({0})", uri);

            return HttpClient.GetAsync(uri)
            .Then(response => 
                response.EnsureSuccessStatusCode()
                .Content.ReadAsStringAsync()
            ).Then(str => JObject.Parse(str));
        }
    }
}
