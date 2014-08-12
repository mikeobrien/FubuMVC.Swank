using System;
using System.Net;
using FubuMVC.Swank.Description;

namespace TestHarness.Administration.Users
{
    public class GetUserRequest
    {
        public Guid UserId { get; set; }
        public Sort Sort { get; set; }
    }

    public enum Sort { Ascending, Descending }

    public class AllGetHandler
    {
        [Description("Get User")]
        [HeaderDescription(HttpHeaderType.Request, "content-type", "This is the content type header.", true)]
        [HeaderDescription(HttpHeaderType.Request, "accept", "This is the accept header.", false)]
        [StatusCodeDescription(HttpStatusCode.MultipleChoices, "Fail Whale", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut leo est, molestie eget laoreet eu, tincidunt sed nibh.")]
        [ResponseComments("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut leo est, molestie eget laoreet eu, tincidunt sed nibh.")]
        public User Execute_UserId_Sort(GetUserRequest request)
        {
            return null;
        } 
    }
}