using System;
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
        public MemberDescription GetDescription(string property)
        {
            return new MemberConvention().GetDescription(typeof(Model).GetProperty(property));
        }

        [Hide]
        public class HiddenType { }

        public class Model
        {
            public string NoDescription { get; set; }

            [XmlElement("NewName")]
            public string CustomXmlElementName { get; set; }

            [DataMember(Name = "NewName")]
            public string CustomDataMemberName { get; set; }

            [Comments("This is a comment.")]
            public string WithComments { get; set; }

            [DefaultValue("This is a default.")]
            public string WithDefaultValue { get; set; }

            [Optional]
            public string Optional { get; set; }

            [XmlArrayItem("Item")]
            public string WithArrayItemName { get; set; }

            [ArrayComments]
            public string WithEmptyArrayComments { get; set; }

            [ArrayComments("This is an array comment.", "This is an item comment.")]
            public string WithArrayComments { get; set; }

            [DictionaryDescription]
            public string WithEmptyDictionaryDescription { get; set; }

            [DictionaryDescription("DictionaryName", "This is a dictionary comment.", "KeyName",
                "This is a key comment.", "This is a value comment.")]
            public string WithDictionaryDescription { get; set; }

            [XmlIgnoreAttribute]
            public string XmlIgnored { get; set; }

            [Hide]
            public string Hidden { get; set; }

            public HiddenType HiddenType { get; set; }

            [FubuMVC.Swank.Description.DescriptionAttribute("NewName", "This is a comment.")]
            public string WithDescription { get; set; }

            [Obsolete]
            public string Deprecated { get; set; }

            [Obsolete("DO NOT seek the treasure!")]
            public string DeprecatedWithMessage { get; set; }
        }

        [Test]
        public void should_return_visible_if_not_specified()
        {
            GetDescription("NoDescription").Hidden.ShouldBeFalse();
        }

        [Test]
        public void should_return_hidden_if_specified(
            [Values("XmlIgnored", "Hidden", "HiddenType")] string property)
        {
            GetDescription(property).Hidden.ShouldBeTrue();
        }

        [Test]
        public void should_return_default_name()
        {
            GetDescription("NoDescription").Name.ShouldEqual("NoDescription");
        }

        [Test]
        public void should_return_custom_name(
            [Values("CustomXmlElementName", "CustomDataMemberName", "WithDescription")] string property)
        {
            GetDescription(property).Name.ShouldEqual("NewName");
        }

        [Test]
        public void should_return_null_comments_if_not_specified()
        {
            GetDescription("NoDescription").Comments.ShouldBeNull();
        }

        [Test]
        public void should_return_comments_if_specified(
            [Values("WithComments", "WithDescription")] string property)
        {
            GetDescription(property).Comments.ShouldEqual("This is a comment.");
        }

        [Test]
        public void should_return_null_default_value_if_not_specified()
        {
            GetDescription("NoDescription").DefaultValue.ShouldBeNull();
        }

        [Test]
        public void should_return_default_value_if_specified()
        {
            GetDescription("WithDefaultValue").DefaultValue.ShouldEqual("This is a default.");
        }

        [Test]
        public void should_return_required_if_not_specified()
        {
            GetDescription("NoDescription").Optional.ShouldBeFalse();
        }

        [Test]
        public void should_return_optional_if_specified()
        {
            GetDescription("Optional").Optional.ShouldBeTrue();
        }

        [Test]
        public void should_return_not_deprecated()
        {
            var description = GetDescription("NoDescription");
            description.Deprecated.ShouldBeFalse();
            description.DeprecationMessage.ShouldBeNull();
        }

        [Test]
        public void should_return_deprecated_without_message()
        {
            var description = GetDescription("Deprecated");
            description.Deprecated.ShouldBeTrue();
            description.DeprecationMessage.ShouldBeNull();
        }

        [Test]
        public void should_return_deprecated_with_message()
        {
            var description = GetDescription("DeprecatedWithMessage");
            description.Deprecated.ShouldBeTrue();
            description.DeprecationMessage.ShouldEqual("DO NOT seek the treasure!");
        }

        [Test]
        public void should_return_null_array_comments_if_not_specified(
            [Values("NoDescription", "WithEmptyArrayComments")] string property)
        {
            GetDescription(property).Comments.ShouldBeNull();
        }

        [Test]
        public void should_return_array_comments_if_specified()
        {
            GetDescription("WithArrayComments").Comments.ShouldEqual("This is an array comment.");
        }

        [Test]
        public void should_return_null_array_item_name_if_not_specified()
        {
            GetDescription("NoDescription").ArrayItem.Name.ShouldBeNull();
        }

        [Test]
        public void should_return_array_item_name_if_specified()
        {
            GetDescription("WithArrayItemName").ArrayItem.Name.ShouldEqual("Item");
        }

        [Test]
        public void should_return_null_array_item_comments_if_not_specified(
            [Values("NoDescription", "WithEmptyArrayComments")] string property)
        {
            GetDescription(property).ArrayItem.Comments.ShouldBeNull();
        }

        [Test]
        public void should_return_array_item_comments_if_specified()
        {
            GetDescription("WithArrayComments").ArrayItem.Comments.ShouldEqual("This is an item comment.");
        }

        [Test]
        public void should_return_null_dictionary_name_if_not_specified(
            [Values("NoDescription", "WithEmptyDictionaryDescription")] string property)
        {
            GetDescription(property).Name.ShouldEqual(property);
        }

        [Test]
        public void should_return_dictionary_name_if_specified()
        {
            GetDescription("WithDictionaryDescription")
                .Name.ShouldEqual("DictionaryName");
        }

        [Test]
        public void should_return_null_dictionary_comments_if_not_specified(
            [Values("NoDescription", "WithEmptyDictionaryDescription")] string property)
        {
            GetDescription(property).Comments.ShouldBeNull();
        }

        [Test]
        public void should_return_dictionary_comments_if_specified()
        {
            GetDescription("WithDictionaryDescription")
                .Comments.ShouldEqual("This is a dictionary comment.");
        }

        [Test]
        public void should_return_null_dictionary_key_name_if_not_specified(
            [Values("NoDescription", "WithEmptyDictionaryDescription")] string property)
        {
            GetDescription(property).DictionaryEntry.KeyName.ShouldBeNull();
        }

        [Test]
        public void should_return_dictionary_key_name_if_specified()
        {
            GetDescription("WithDictionaryDescription").DictionaryEntry
                .KeyName.ShouldEqual("KeyName");
        }

        [Test]
        public void should_return_null_dictionary_key_comments_if_not_specified(
            [Values("NoDescription", "WithEmptyDictionaryDescription")] string property)
        {
            GetDescription(property).DictionaryEntry.KeyComments.ShouldBeNull();
        }

        [Test]
        public void should_return_dictionary_key_comments_if_specified()
        {
            GetDescription("WithDictionaryDescription").DictionaryEntry.KeyComments.ShouldEqual("This is a key comment.");
        }

        [Test]
        public void should_return_null_dictionary_value_comments_if_not_specified(
            [Values("NoDescription", "WithEmptyDictionaryDescription")] string property)
        {
            GetDescription(property).DictionaryEntry.ValueComments.ShouldBeNull();
        }

        [Test]
        public void should_return_dictionary_value_comments_if_specified()
        {
            GetDescription("WithDictionaryDescription").DictionaryEntry.ValueComments.ShouldEqual("This is a value comment.");
        }
    }
}