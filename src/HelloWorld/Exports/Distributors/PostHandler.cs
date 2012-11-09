using System;
using System.Net;
using FubuMVC.Swank.Description;

namespace HelloWorld.Exports.Distributors
{
    public class PostHandler
    {
        [Description("Add Distributor")]
        [StatusCodeDescription(HttpStatusCode.MultipleChoices, "Fail Whale", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut leo est, molestie eget laoreet eu, tincidunt sed nibh.")]
        [ResponseComments("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut leo est, molestie eget laoreet eu, tincidunt sed nibh.")]
        public Distributor Execute(Distributor request)
        {
            return null;
        } 
    }
}