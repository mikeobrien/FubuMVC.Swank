using System;
using System.Net;
using FubuMVC.Swank.Description;

namespace HelloWorld.Exports.Distributors
{
    public class DeleteDistributorRequest
    {
        [Comments("In volutpat tortor quis mauris blandit non viverra tellus mollis. Nam ac fermentum augue.")]
        public Guid DistributorId { get; set; }
    }

    public class DeleteHandler
    {
        [Description("Delete Distributor")]
        [ErrorDescription(HttpStatusCode.MultipleChoices, "Fail Whale", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut leo est, molestie eget laoreet eu, tincidunt sed nibh.")]
        [ResponseComments("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut leo est, molestie eget laoreet eu, tincidunt sed nibh.")]
        public void Execute_DistributorId(DeleteDistributorRequest request)
        {
        } 
    }
}