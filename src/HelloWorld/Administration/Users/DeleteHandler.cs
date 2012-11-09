using System;
using System.Net;
using FubuMVC.Swank.Description;

namespace HelloWorld.Administration.Users
{
    public class DeleteUserRequest
    {
        public Guid UserId { get; set; }
    }

    public class DeleteHandler
    {
        [Description("Delete User")]
        [StatusCodeDescription(HttpStatusCode.Unauthorized, "Fail Whale", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut leo est, molestie eget laoreet eu, tincidunt sed nibh.")]
        [ResponseComments("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut leo est, molestie eget laoreet eu, tincidunt sed nibh.")]
        public void Execute_UserId(DeleteUserRequest request)
        {
        } 
    }
}