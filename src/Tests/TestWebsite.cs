using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using FubuCore;
using Microsoft.Web.Administration;

namespace Tests
{
    public class TestWebsite
    {
        private const int Port = 34523;
        private string _siteName;

        public void Create()
        {
            using (var manager = new ServerManager())
            {
                _siteName = "FubuMVC.Swank_" + Guid.NewGuid().ToString("N");
                manager.Sites.Add(_siteName, 
                                  "http", "*:{0}:".ToFormat(Port), Path.GetFullPath(Environment.CurrentDirectory + @"\..\..\..\TestHarness"));
                manager.CommitChanges(); 
            } 
        }

        public void Remove()
        {
            using (var manager = new ServerManager())
            {
                manager.Sites.Remove(manager.Sites[_siteName]);
                manager.CommitChanges();
                manager.Dispose();
            }
        }

        public string DownloadString(string url)
        {
            try
            {
                return new WebClient().DownloadString("http://localhost:{0}/{1}".ToFormat(Port, url));
            }
            catch (WebException exception)
            {
                var response = (HttpWebResponse) exception.Response;
                Debug.WriteLine("{0}: {1}", response.StatusCode, response.StatusDescription);
                Debug.WriteLine(new StreamReader(response.GetResponseStream()).ReadToEnd());
                throw;
            }
        }
    }
}