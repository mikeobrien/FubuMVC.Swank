using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using FubuMVC.Swank.Description;
using NUnit.Framework;
using Should;

namespace Tests.Description
{
    [TestFixture]
    public class MemberConventionTests
    {
        private MemberConvention _memberConvention;

        [SetUp]
        public void Setup()
        {
            _memberConvention = new MemberConvention();
        }

        public class Item {}
        public class Request
        {
            [Comments("This is the id.")]
            public Guid Id { get; set; }
            public Guid? NullableId { get; set; }
            [Required(false)]
            public string Sort { get; set; }
            [XmlElement("R2D2")]
            public int C3P0 { get; set; }
            public List<Item> ListOfComplexTypes { get; set; }
            public List<int> ListOfSimpleTypes { get; set; }
            [XmlArrayItem("Item")]
            public List<Item> ListOfComplexTypesWithCustomItemName { get; set; }
            [XmlArrayItem("Item")]
            public List<int> ListOfSimpleTypesWithCustomItemName { get; set; }
            [DataMember(Name = "Tatooine")]
            public int HanSolo { get; set; }
        }

        [Test]
        public void should_return_default_description_of_member()
        {
            var description = _memberConvention.GetDescription(typeof(Request).GetProperty("Sort"));
            description.Name.ShouldEqual("Sort");
            description.Comments.ShouldBeNull();
        }

        [Test]
        public void should_return_attribute_description_of_member()
        {
            var description = _memberConvention.GetDescription(typeof(Request).GetProperty("Id"));
            description.Name.ShouldEqual("Id");
            description.Comments.ShouldEqual("This is the id.");
        }

        [Test]
        public void should_indicate_if_the_member_is_set_to_be_required()
        {
            _memberConvention.GetDescription(typeof(Request).GetProperty("Id")).Required.ShouldBeTrue();
            _memberConvention.GetDescription(typeof(Request).GetProperty("Sort")).Required.ShouldBeFalse();
            _memberConvention.GetDescription(typeof(Request).GetProperty("NullableId")).Required.ShouldBeFalse();
        }

        [Test]
        public void should_return_name_specified_in_xml_element()
        {
            _memberConvention.GetDescription(typeof(Request).GetProperty("C3P0")).Name.ShouldEqual("R2D2");
        }

        [Test]
        public void should_return_name_specified_in_data_member()
        {
            _memberConvention.GetDescription(typeof(Request).GetProperty("HanSolo")).Name.ShouldEqual("Tatooine");
        }

        [Test]
        public void should_return_property_type()
        {
            _memberConvention.GetDescription(typeof(Request).GetProperty("C3P0")).Type.ShouldEqual(typeof(int));
        }

        [Test]
        public void should_return_list_element_simple_type()
        {
            _memberConvention.GetDescription(typeof(Request).GetProperty("ListOfSimpleTypes")).Type.ShouldEqual(typeof(int));
        }

        [Test]
        public void should_return_list_element_complex_type()
        {
            _memberConvention.GetDescription(typeof(Request).GetProperty("ListOfComplexTypes")).Type.ShouldEqual(typeof(Item));
        }

        [Test]
        public void should_return_list_element_simple_type_custom_item_name()
        {
            _memberConvention.GetDescription(typeof(Request).GetProperty("ListOfSimpleTypes")).ArrayItemName.ShouldBeNull();
            _memberConvention.GetDescription(typeof(Request).GetProperty("ListOfSimpleTypesWithCustomItemName")).ArrayItemName.ShouldEqual("Item");
        }

        [Test]
        public void should_return_list_element_complex_type_custom_item_name()
        {
            _memberConvention.GetDescription(typeof(Request).GetProperty("ListOfComplexTypes")).ArrayItemName.ShouldBeNull();
            _memberConvention.GetDescription(typeof(Request).GetProperty("ListOfComplexTypesWithCustomItemName")).ArrayItemName.ShouldEqual("Item");
        }
    }
}