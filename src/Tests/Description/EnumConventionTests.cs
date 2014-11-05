using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using FubuMVC.Swank.Description;
using NUnit.Framework;
using Should;

namespace Tests.Description
{
    [TestFixture]
    public class EnumConventionTests 
    {
        public FubuMVC.Swank.Description.Description GetDescription(Type type)
        {
            return new EnumConvention().GetDescription(type);
        }

        public FubuMVC.Swank.Description.Description GetDescription<T>()
        {
            return GetDescription(typeof(T));
        }

        public enum SomeEnum { }

        [Test]
        public void should_return_default_description_of_enum()
        {
            var description = GetDescription<SomeEnum>();
            description.Name.ShouldEqual("SomeEnum");
            description.Comments.ShouldBeNull();
        }

        [Comments("This is an enum with comments.")]
        public enum SomeEnumWithComments { }

        [Test]
        public void should_return_attribute_description_of_enum()
        {
            var description = GetDescription<SomeEnumWithComments>();
            description.Name.ShouldEqual("SomeEnumWithComments");
            description.Comments.ShouldEqual("This is an enum with comments.");
        }

        [XmlType("SomeEnum")]
        public enum SomeEnumWithXmlName { }

        [Test]
        public void should_return_attribute_description_of_enum_and_xml_type_attribute()
        {
            var description = GetDescription<SomeEnumWithXmlName>();
            description.Name.ShouldEqual("SomeEnum");
            description.Comments.ShouldBeNull();
        }

        [XmlRoot("SomeRoot")]
        public enum SomeEnumWithXmlRootName { }

        [Test]
        public void should_return_attribute_description_of_enum_and_xml_root_attribute()
        {
            var description = GetDescription<SomeEnumWithXmlRootName>();
            description.Name.ShouldEqual("SomeRoot");
            description.Comments.ShouldBeNull();
        }

        [DataContract(Name = "SomeEnum")]
        public enum SomeEnumWithDataContractName { }

        [Test]
        public void should_return_data_contract_attribute_name()
        {
            var description = GetDescription<SomeEnumWithDataContractName>();
            description.Name.ShouldEqual("SomeEnum");
            description.Comments.ShouldBeNull();
        }
    }
}