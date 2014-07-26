using System;
using System.Net;
using FubuMVC.Swank.Description;
using FubuMVC.Swank.Net;

namespace TestHarness.Administration.Users
{
    public class PostHandler
    {
        [Description("Add User")]
        [StatusCode(HttpStatusCode.MultipleChoices, "Fail Whale", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut leo est, molestie eget laoreet eu, tincidunt sed nibh.")]
        [ResponseComments("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut leo est, molestie eget laoreet eu, tincidunt sed nibh.")]
        [MimeType(HttpDirection.Request, MimeType.ApplicationJson)]
        [MimeType(HttpDirection.Request, MimeType.ApplicationXml)]
        [MimeType(HttpDirection.Response, MimeType.ApplicationXml)]
        [Header(HttpDirection.Request, "content-type", "This is the content type header.", true)]
        [Header(HttpDirection.Request, "accept", "This is the accept header.", false)]
        [Header(HttpDirection.Response, "content-type", "This is the content type header.", true)]
        [Header(HttpDirection.Response, "accept", "This is the accept header.", false)]
        public User Execute(User request)
        {
            return null;
        } 
    }
}