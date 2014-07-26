using System;
using System.Collections.Generic;
using FubuMVC.Swank.Description;

namespace TestHarness.Administration.Users
{
    public class User
    {
        public enum UserType
        {
            [Description("Administrator", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut leo est, molestie eget laoreet eu, tincidunt sed nibh.")]
            Admin,
            [Description("General User", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut leo est, molestie eget laoreet eu, tincidunt sed nibh.")]
            User,
            [Description("Guest User", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut leo est, molestie eget laoreet eu, tincidunt sed nibh.")]
            Guest
        }

        public Guid UserId { get; set; }
        [Comments("In volutpat tortor quis mauris blandit non viverra tellus mollis. Nam ac fermentum augue.")]
        public string Name { get; set; }
        [DefaultValue(UserType.Guest)]
        [Comments("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut leo est, molestie eget laoreet eu, tincidunt sed nibh.")]
        public UserType? Type { get; set; }
        [Comments("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut leo est, molestie eget laoreet eu, tincidunt sed nibh.")]
        public List<Address> Addresses { get; set; }
    }
}