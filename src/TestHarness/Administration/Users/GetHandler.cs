using System;
using System.Net;
using FubuMVC.Swank.Description;

namespace TestHarness.Administration.Users
{
    public class GetUserRequest
    {
        public Guid UserId { get; set; }
    }

    public class AllGetHandler
    {
        [Description("Get User")]
        [ErrorDescription(HttpStatusCode.MultipleChoices, "Fail Whale", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut leo est, molestie eget laoreet eu, tincidunt sed nibh.")]
        [ResponseComments("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut leo est, molestie eget laoreet eu, tincidunt sed nibh.")]
        public User Execute_UserId(GetUserRequest request)
        {
            return null;
        } 
    }
}