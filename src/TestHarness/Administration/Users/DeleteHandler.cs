using System;
using System.Net;
using FubuMVC.Swank.Description;

namespace TestHarness.Administration.Users
{
    public class DeleteUserRequest
    {
        public Guid UserId { get; set; }
    }

    public class DeleteHandler
    {
        [Description("Delete User")]
        [StatusCodeDescription(HttpStatusCode.MultipleChoices, "Fail Whale", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut leo est, molestie eget laoreet eu, tincidunt sed nibh.")]
        [ResponseComments("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut leo est, molestie eget laoreet eu, tincidunt sed nibh.")]
        public void Execute_UserId(DeleteUserRequest request)
        {
        } 
    }
}