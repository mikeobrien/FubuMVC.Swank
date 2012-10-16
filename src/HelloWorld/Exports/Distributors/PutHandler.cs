using System;
using System.Net;
using FubuMVC.Swank.Description;

namespace HelloWorld.Exports.Distributors
{
    public class PutHandler
    {
        [Description("Update Distributor")]
        [ErrorDescription(HttpStatusCode.MultipleChoices, "Fail Whale", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut leo est, molestie eget laoreet eu, tincidunt sed nibh.")]
        [ResponseComments("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut leo est, molestie eget laoreet eu, tincidunt sed nibh.")]
        public void Execute_DistributorId(Distributor request)
        {
        } 
    }
}