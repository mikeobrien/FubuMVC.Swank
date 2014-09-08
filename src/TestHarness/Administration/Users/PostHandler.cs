using System;
using System.Net;
using FubuMVC.Swank.Description;

namespace TestHarness.Administration.Users
{
    public class PostHandler
    {
        [Description("Add User")]
        [StatusCode(HttpStatusCode.MultipleChoices, "Fail Whale", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut leo est, molestie eget laoreet eu, tincidunt sed nibh.")]
        [ResponseComments("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut leo est, molestie eget laoreet eu, tincidunt sed nibh.")]
        public User Execute(User request)
        {
            return null;
        } 
    }
}