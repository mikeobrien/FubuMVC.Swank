using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using FubuCore;
using Microsoft.Web.Administration;

namespace Tests
{
    public class Website
    {
        private string _siteName;
        private int _port;

        public void Create(string name, string path, int port)
        {
            _port = port;
            using (var manager = new ServerManager())
            {
                _siteName = name + "_" + Guid.NewGuid().ToString("N");
                manager.Sites.Add(_siteName, "http", "*:{0}:".ToFormat(port), path);
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

        public string DownloadString(string url = "")
        {
            try
            {
                return new WebClient().DownloadString("http://localhost:{0}/{1}".ToFormat(_port, url));
            }
            catch (WebException exception)
            {
                var response = (HttpWebResponse) exception.Response;
                Console.WriteLine("{0}: {1}", response.StatusCode, response.StatusDescription);
                Console.WriteLine(new StreamReader(response.GetResponseStream()).ReadToEnd());
                throw;
            }
        }
    }
}