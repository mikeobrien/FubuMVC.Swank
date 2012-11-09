using System.Net;
using FubuMVC.Swank.Description;

namespace TestHarness.Exports.Distributors
{
    public class PutHandler
    {
        [Description("Update Distributor")]
        [StatusCodeDescription(HttpStatusCode.MultipleChoices, "Fail Whale", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut leo est, molestie eget laoreet eu, tincidunt sed nibh.")]
        [ResponseComments("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut leo est, molestie eget laoreet eu, tincidunt sed nibh.")]
        public void Execute_DistributorId(Distributor request)
        {
        } 
    }
}