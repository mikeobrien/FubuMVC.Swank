using System;
using FubuMVC.Swank.Description;

namespace TestHarness.Exports.Distributors
{
    [Comments("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut leo est, molestie eget laoreet eu, tincidunt sed nibh.")]
    public class Address
    {
        public enum AddressType
        {
            [Comments("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut leo est, molestie eget laoreet eu, tincidunt sed nibh.")]
            Office,
            [Comments("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut leo est, molestie eget laoreet eu, tincidunt sed nibh.")]
            Warehouse,
            [Comments("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut leo est, molestie eget laoreet eu, tincidunt sed nibh.")]
            Sales
        }

        public Guid Id { get; set; }
        [Required]
        [Comments("In volutpat tortor quis mauris blandit non viverra tellus mollis. Nam ac fermentum augue.")]
        public string Street { get; set; }
        [Required]
        [Comments("In volutpat tortor quis mauris blandit non viverra tellus mollis. Nam ac fermentum augue.")]
        public string City { get; set; }
        [Required]
        [Comments("In volutpat tortor quis mauris blandit non viverra tellus mollis. Nam ac fermentum augue.")]
        public string State { get; set; }
        [Required]
        [Comments("In volutpat tortor quis mauris blandit non viverra tellus mollis. Nam ac fermentum augue.")]
        public string Zip { get; set; }
        [DefaultValue(AddressType.Office)]
        [Comments("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut leo est, molestie eget laoreet eu, tincidunt sed nibh.")]
        public AddressType Type { get; set; }
    }
}