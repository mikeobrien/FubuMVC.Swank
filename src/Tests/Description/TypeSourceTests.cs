using System.Collections.Generic;
using System.Xml.Serialization;
using FubuMVC.Swank.Description;
using NUnit.Framework;
using Should;

namespace Tests.Description
{
    [TestFixture]
    public class TypeSourceTests
    {
        public class SomeType { }

        [Test]
        public void should_return_default_description_of_datatype()
        {
            var type = typeof(SomeType);
            var description = new TypeSource().GetDescription(type);
            description.Type.ShouldEqual(type);
            description.Name.ShouldEqual("SomeType");
            description.Comments.ShouldBeNull();
        }

        [Test]
        public void should_return_default_description_of_list_datatype()
        {
            var description = new TypeSource().GetDescription(typeof(List<SomeType>));
            description.Type.ShouldEqual(typeof(SomeType));
            description.Name.ShouldEqual("ArrayOfSomeType");
            description.Comments.ShouldBeNull();
        }

        [Comments("This is a type with comments.")]
        public class SomeTypeWithComments { }

        [Test]
        public void should_return_attribute_description_of_datatype()
        {
            var type = typeof(SomeTypeWithComments);
            var description = new TypeSource().GetDescription(type);
            description.Type.ShouldEqual(type);
            description.Name.ShouldEqual("SomeTypeWithComments");
            description.Comments.ShouldEqual("This is a type with comments.");
        }

        [XmlType("SomeType")]
        public class SomeTypeWithXmlName { }

        [Test]
        public void should_return_attribute_description_of_datatype_and_xml_type_attribute()
        {
            var type = typeof(SomeTypeWithXmlName);
            var description = new TypeSource().GetDescription(type);
            description.Type.ShouldEqual(type);
            description.Name.ShouldEqual("SomeType");
            description.Comments.ShouldBeNull();
        }

        [Comments("These are some types.")]
        public class SomeTypes : List<SomeType> { }

        [Test]
        public void should_return_attribute_description_of_inherited_list_datatype()
        {
            var description = new TypeSource().GetDescription(typeof(SomeTypes));
            description.Type.ShouldEqual(typeof(SomeType));
            description.Name.ShouldEqual("ArrayOfSomeType");
            description.Comments.ShouldEqual("These are some types.");
        }

        [Comments("These are some moar types."), XmlType("SomeTypes")]
        public class SomeMoarTypes : List<SomeType> { }

        [Test]
        public void should_return_attribute_description_of_inherited_list_datatype_with_xml_type_attribute()
        {
            var description = new TypeSource().GetDescription(typeof(SomeMoarTypes));
            description.Type.ShouldEqual(typeof(SomeType));
            description.Name.ShouldEqual("SomeTypes");
            description.Comments.ShouldEqual("These are some moar types.");
        }
    }
}