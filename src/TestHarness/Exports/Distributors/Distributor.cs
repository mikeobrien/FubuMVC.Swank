using System;
using System.Collections.Generic;
using FubuMVC.Swank.Description;

namespace TestHarness.Exports.Distributors
{
    public class Distributor
    {
        public enum DistributorType
        {
            [Description("Intensive", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut leo est, molestie eget laoreet eu, tincidunt sed nibh.")]
            Intensive,
            [Description("Selective", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut leo est, molestie eget laoreet eu, tincidunt sed nibh.")]
            Selective,
            [Description("Exclusive", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut leo est, molestie eget laoreet eu, tincidunt sed nibh.")]
            Exclusive
        }

        [Comments("In volutpat tortor quis mauris blandit non viverra tellus mollis. Nam ac fermentum augue.")]
        public Guid DistributorId { get; set; }
        [Comments("In volutpat tortor quis mauris blandit non viverra tellus mollis. Nam ac fermentum augue.")]
        public string Name { get; set; }
        [DefaultValue(DistributorType.Exclusive)]
        [Comments("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut leo est, molestie eget laoreet eu, tincidunt sed nibh.")]
        public DistributorType Type { get; set; }
        [Comments("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut leo est, molestie eget laoreet eu, tincidunt sed nibh.")]
        public List<Address> Addresses { get; set; }
        [Comments("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut leo est, molestie eget laoreet eu, tincidunt sed nibh.")]
        public Dictionary<string, string> LocationDescriptions { get; set; }
        [Comments("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut leo est, molestie eget laoreet eu, tincidunt sed nibh.")]
        public Dictionary<string, Address> Locations { get; set; }
        [Comments("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut leo est, molestie eget laoreet eu, tincidunt sed nibh.")]
        public Dictionary<string, DistributorType> LocationTypes { get; set; }
    }
}