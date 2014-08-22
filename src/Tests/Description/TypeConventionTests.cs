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
    public class TypeConventionTests
    {
        public TypeDescription GetDescription(Type type)
        {
            return new TypeConvention().GetDescription(type);
        }

        public TypeDescription GetDescription<T>()
        {
            return GetDescription(typeof(T));
        }

        public class SomeType { }

        [Test]
        public void should_return_default_description_of_datatype()
        {
            var description = GetDescription<SomeType>();
            description.Name.ShouldEqual("SomeType");
            description.Comments.ShouldBeNull();
        }

        [Test]
        public void should_return_default_description_of_list_datatype()
        {
            var description = GetDescription<List<SomeType>>();
            description.Name.ShouldEqual("ArrayOfSomeType");
            description.Comments.ShouldBeNull();
        }

        [Comments("This is a type with comments.")]
        public class SomeTypeWithComments { }

        [Test]
        public void should_return_attribute_description_of_datatype()
        {
            var description = GetDescription<SomeTypeWithComments>();
            description.Name.ShouldEqual("SomeTypeWithComments");
            description.Comments.ShouldEqual("This is a type with comments.");
        }

        [XmlType("SomeType")]
        public class SomeTypeWithXmlName { }

        [Test]
        public void should_return_attribute_description_of_datatype_and_xml_type_attribute()
        {
            var description = GetDescription<SomeTypeWithXmlName>();
            description.Name.ShouldEqual("SomeType");
            description.Comments.ShouldBeNull();
        }

        [XmlRoot("SomeRoot")]
        public class SomeTypeWithXmlRootName { }

        [Test]
        public void should_return_attribute_description_of_datatype_and_xml_root_attribute()
        {
            var description = GetDescription<SomeTypeWithXmlRootName>();
            description.Name.ShouldEqual("SomeRoot");
            description.Comments.ShouldBeNull();
        }

        [DataContract(Name = "SomeType")]
        public class SomeTypeWithDataContractName { }

        [Test]
        public void should_return_data_contract_attribute_name()
        {
            var description = GetDescription<SomeTypeWithDataContractName>();
            description.Name.ShouldEqual("SomeType");
            description.Comments.ShouldBeNull();
        }

        [Comments("These are some types.")]
        public class SomeTypes : List<SomeType> { }

        [Test]
        public void should_return_attribute_description_of_inherited_list_datatype()
        {
            var description = GetDescription<SomeTypes>();
            description.Name.ShouldEqual("ArrayOfSomeType");
            description.Comments.ShouldEqual("These are some types.");
        }

        [Comments("These are some moar types."), XmlType("SomeTypes")]
        public class SomeMoarTypes : List<SomeType> { }

        [CollectionDataContract(Name = "SomeTypes")]
        public class SomeCollectionWithDataContractName : List<SomeType> { }

        [Test]
        public void should_return_attribute_description_of_inherited_list_datatype_with_xml_type_attribute()
        {
            var description = GetDescription<SomeMoarTypes>();
            description.Name.ShouldEqual("SomeTypes");
            description.Comments.ShouldEqual("These are some moar types.");
        }

        [Test]
        public void should_return_name_of_inherited_list_datatype_with_collection_data_contract_attribute()
        {
            var description = GetDescription<SomeCollectionWithDataContractName>();
            description.Name.ShouldEqual("SomeTypes");
            description.Comments.ShouldBeNull();
        }

        [Test]
        public void should_initial_cap_list_primitive_type_name()
        {
            var description = GetDescription<List<Int64>>();
            description.Name.ShouldEqual("ArrayOfLong");
        }

        public class WithNoArrayComments : List<int> { }

        [ArrayComments]
        public class WithEmptyArrayComments : List<int> { }

        [ArrayComments("This is a comment.", "This is an item comment.")]
        public class WithArrayComments : List<int> { }

        [Test]
        public void should_return_null_array_comments_if_not_specified(
            [Values(typeof(WithNoArrayComments), typeof(WithEmptyArrayComments))] Type type)
        {
            GetDescription(type).Comments.ShouldBeNull();
        }

        [Test]
        public void should_return_array_comments_if_specified()
        {
            GetDescription<WithArrayComments>().Comments.ShouldEqual("This is a comment.");
        }

        [Test]
        public void should_return_null_array_item_comments_if_not_specified(
            [Values(typeof(WithNoArrayComments), typeof(WithEmptyArrayComments))] Type type)
        {
            GetDescription(type).ArrayItem.Comments.ShouldBeNull();
        }

        [Test]
        public void should_return_array_item_comments_if_specified()
        {
            GetDescription<WithArrayComments>().ArrayItem.Comments.ShouldEqual("This is an item comment.");
        }

        public class WithNoDictionaryComments : Dictionary<string, int> { }

        [DictionaryComments]
        public class WithEmptyDictionaryComments : Dictionary<string, int> { }

        [DictionaryComments("This is a comment.", "This is a key comment.", "This is a value comment.")]
        public class WithDictionaryComments : Dictionary<string, int> { }

        [Test]
        public void should_return_null_dictionary_comments_if_not_specified(
            [Values(typeof(WithNoDictionaryComments), typeof(WithEmptyDictionaryComments))] Type type)
        {
            GetDescription(type).Comments.ShouldBeNull();
        }

        [Test]
        public void should_return_dictionary_comments_if_specified()
        {
            GetDescription<WithDictionaryComments>().Comments.ShouldEqual("This is a comment.");
        }

        [Test]
        public void should_return_null_dictionary_key_comments_if_not_specified(
            [Values(typeof(WithNoDictionaryComments), typeof(WithEmptyDictionaryComments))] Type type)
        {
            GetDescription(type).DictionaryEntry.KeyComments.ShouldBeNull();
        }

        [Test]
        public void should_return_dictionary_key_comments_if_specified()
        {
            GetDescription<WithDictionaryComments>().DictionaryEntry.KeyComments.ShouldEqual("This is a key comment.");
        }

        [Test]
        public void should_return_null_dictionary_value_comments_if_not_specified(
            [Values(typeof(WithNoDictionaryComments), typeof(WithEmptyDictionaryComments))] Type type)
        {
            GetDescription(type).DictionaryEntry.ValueComments.ShouldBeNull();
        }

        [Test]
        public void should_return_dictionary_value_comments_if_specified()
        {
            GetDescription<WithDictionaryComments>().DictionaryEntry.ValueComments.ShouldEqual("This is a value comment.");
        }
    }
}