using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using FubuMVC.Swank.Description;
using NUnit.Framework;
using Should;

namespace Tests.Description
{
    [TestFixture]
    public class MemberSourceTests
    {
        private MemberSource _memberSource;

        [SetUp]
        public void Setup()
        {
            _memberSource = new MemberSource();
        }

        public class Item {}
        public class Request
        {
            [Comments("This is the id.")]
            public Guid Id { get; set; }
            [Required]
            public string Sort { get; set; }
            [XmlElement("R2D2")]
            public int C3P0 { get; set; }
            public List<Item> Items { get; set; }
        }

        [Test]
        public void should_return_default_description_of_member()
        {
            var description = _memberSource.GetDescription(typeof(Request).GetProperty("Sort"));
            description.Name.ShouldEqual("Sort");
            description.Comments.ShouldBeNull();
        }

        [Test]
        public void should_return_attribute_description_of_member()
        {
            var description = _memberSource.GetDescription(typeof(Request).GetProperty("Id"));
            description.Name.ShouldEqual("Id");
            description.Comments.ShouldEqual("This is the id.");
        }

        [Test]
        public void should_indicate_if_the_member_is_required()
        {
            _memberSource.GetDescription(typeof(Request).GetProperty("Id")).Required.ShouldBeFalse();
            _memberSource.GetDescription(typeof(Request).GetProperty("Sort")).Required.ShouldBeTrue();
        }

        [Test]
        public void should_return_name_specified_in_xml_element()
        {
            _memberSource.GetDescription(typeof(Request).GetProperty("C3P0")).Name.ShouldEqual("R2D2");
        }

        [Test]
        public void should_return_property_type()
        {
            _memberSource.GetDescription(typeof(Request).GetProperty("C3P0")).Type.ShouldEqual(typeof(int));
        }

        [Test]
        public void should_return_list_element_type()
        {
            _memberSource.GetDescription(typeof(Request).GetProperty("Items")).Type.ShouldEqual(typeof(Item));
        }
    }
}