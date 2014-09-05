using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Reflection;
using FubuMVC.Core;
using FubuMVC.Media.Projections;
using FubuMVC.Swank;
using FubuMVC.Swank.Description;
using FubuMVC.Swank.Specification;
using NUnit.Framework;
using Should;

namespace Tests.Specification
{
    [TestFixture]
    public class TypeGraphFactoryTests
    {
        public TypeGraphFactory CreateFactory(Action<Configuration> configure = null)
        {
            var configuration = new Configuration();
            if (configure != null) configure(configuration);
            return new TypeGraphFactory(
                configuration, 
                new TypeDescriptorCache(), 
                new TypeConvention(configuration), 
                new MemberConvention(), 
                new OptionFactory(configuration, new OptionConvention()));
        }

        public class TypeWithoutComments { }

        [Test]
        public void should_create_type_without_comments()
        {
            var type = CreateFactory().BuildGraph(
                typeof(TypeWithoutComments));

            type.Name.ShouldEqual("TypeWithoutComments");
            type.Comments.ShouldBeNull();
        }

        [Comments("This is a type.")]
        public class TypeWithComments { }

        [Test]
        public void should_create_type_with_comments()
        {
            var type = CreateFactory().BuildGraph(
                typeof (TypeWithComments));

            type.Name.ShouldEqual("TypeWithComments");
            type.Comments.ShouldEqual("This is a type.");
        }

        [Test]
        public void should_override_type()
        {
            var type = CreateFactory(x => x.TypeOverrides.Add((t, d) =>
            {
                d.Name += t.Name;
                d.Comments += t.Name;
            })).BuildGraph(typeof(TypeWithComments));

            type.Name.ShouldEqual("TypeWithCommentsTypeWithComments");
            type.Comments.ShouldEqual("This is a type.TypeWithComments");
        }

        public class TypeWithOverridedMember
        {
            [Comments("This is a member.")]
            public string Member { get; set; }
        }

        [Test]
        public void should_override_type_members()
        {
            var type = CreateFactory(x => x.MemberOverrides.Add((p, m) =>
            {
                m.Name += p.Name;
                m.Comments += p.Name;
            })).BuildGraph(typeof(TypeWithOverridedMember));

            var member = type.Members.Single();
            member.Name.ShouldEqual("MemberMember");
            member.Comments.ShouldEqual("This is a member.Member");
        }

        // Simple types

        [Test]
        [TestCase(typeof(string), "string")]
        [TestCase(typeof(int), "int")]
        [TestCase(typeof(int?), "int")]
        [TestCase(typeof(Guid), "uuid")]
        [TestCase(typeof(TimeSpan), "duration")]
        [TestCase(typeof(DateTime), "dateTime")]
        [TestCase(typeof(Uri), "anyURI")]
        public void should_create_simple_type(Type type, string name)
        {
            should_be_simple_type(CreateFactory().BuildGraph(type), name);
        }

        public class SimpleTypeMember
        {
            public int Member { get; set; }
        }

        [Test]
        public void should_create_simple_type_member()
        {
            should_be_simple_type(CreateFactory().BuildGraph(
                typeof(SimpleTypeMember)).Members.Single().Type, "int");
        }

        public void should_be_simple_type(DataType type, string name)
        {
            type.Name.ShouldEqual(name);
            type.Comments.ShouldBeNull();

            type.IsSimple.ShouldBeTrue();
            type.Options.ShouldBeNull();

            type.IsComplex.ShouldBeFalse();
            type.Members.ShouldBeNull();

            type.IsArray.ShouldBeFalse();
            type.ArrayItem.ShouldBeNull();

            type.IsDictionary.ShouldBeFalse();
            type.DictionaryEntry.ShouldBeNull();
        }

        public enum Options
        {
            Option,
            [Comments("This is an option.")]
            OptionWithComments
        }

        [Test]
        public void should_create_simple_type_string_options(
            [Values(typeof(Options), typeof(Options?))]Type type)
        {
            var dataType = CreateFactory().BuildGraph(type);

            dataType.Name.ShouldEqual("int");
            dataType.IsSimple.ShouldBeTrue();
            dataType.Options.Count.ShouldEqual(2);

            var option = dataType.Options[0];
            option.Name.ShouldEqual("Option");
            option.Value.ShouldEqual("0");
            option.Comments.ShouldBeNull();
            
            option = dataType.Options[1];
            option.Name.ShouldEqual("OptionWithComments");
            option.Value.ShouldEqual("1");
            option.Comments.ShouldEqual("This is an option.");
        }

        [Test]
        public void should_create_simple_type_numeric_options(
            [Values(typeof(Options), typeof(Options?))]Type type)
        {
            var dataType = CreateFactory(x => x.EnumValue = EnumValue.AsString).BuildGraph(type);

            dataType.Name.ShouldEqual("string");
            dataType.IsSimple.ShouldBeTrue();
            dataType.Options.Count.ShouldEqual(2);

            var option = dataType.Options[0];
            option.Name.ShouldEqual("Option");
            option.Value.ShouldEqual("Option");
            option.Comments.ShouldBeNull();

            option = dataType.Options[1];
            option.Name.ShouldEqual("OptionWithComments");
            option.Value.ShouldEqual("OptionWithComments");
            option.Comments.ShouldEqual("This is an option.");
        }

        // Arrays

        [Test]
        [TestCase(typeof(int[]))]
        [TestCase(typeof(int?[]))]
        [TestCase(typeof(IList<int>))]
        [TestCase(typeof(List<int>))]
        public void should_create_array(Type type)
        {
            should_be_array_type(CreateFactory().BuildGraph(type));
        }

        [Comments("This is an array.")]
        public class ListWithComments : List<int> { }

        [Test]
        public void should_create_array_with_comments()
        {
            should_be_array_type(CreateFactory().BuildGraph(typeof(ListWithComments)),
                comments: "This is an array.");
        }

        [ArrayDescription("ArrayName", "This is an array comment.", 
            "ItemName", "This is an item comment.")]
        public class ListWithArrayDescription : List<int> { }

        [Test]
        public void should_create_array_with_array_description()
        {
            should_be_array_type(CreateFactory().BuildGraph(typeof(ListWithArrayDescription)),
                name: "ArrayName", comments: "This is an array comment.",
                itemName: "ItemName", itemComments: "This is an item comment.");
        }

        public class ArrayMember
        {
            public List<int> MemberWithoutComments { get; set; }
            [ArrayDescription("ArrayName", "This is an array comment.", 
                "ItemName", "This is an item comment.")]
            public List<int> MemberWithComments { get; set; }
        }

        [Test]
        public void should_create_array_member_without_description()
        {
            should_be_array_type(CreateFactory().BuildGraph(typeof(ArrayMember))
                .Members.Single(x => x.Name == "MemberWithoutComments").Type);
        }

        [Test]
        public void should_create_array_member_with_description()
        {
            should_be_array_type(CreateFactory().BuildGraph(typeof(ArrayMember))
                .Members.Single(x => x.Name == "ArrayName").Type,
                comments: "This is an array comment.",
                itemName: "ItemName", itemComments: "This is an item comment.");
        }

        public void should_be_array_type(DataType type, string name = null, string comments = null, 
            string itemName = "int", string itemComments = null)
        {
            type.Name.ShouldEqual(name ?? "ArrayOfInt");
            type.Comments.ShouldEqual(comments);

            type.IsArray.ShouldBeTrue();
            type.ArrayItem.ShouldNotBeNull();
            type.ArrayItem.Name.ShouldEqual(itemName);
            type.ArrayItem.Comments.ShouldEqual(itemComments);
            should_be_simple_type(type.ArrayItem.Type, "int");

            type.IsSimple.ShouldBeFalse();
            type.Options.ShouldBeNull();

            type.IsComplex.ShouldBeFalse();
            type.Members.ShouldBeNull();

            type.IsDictionary.ShouldBeFalse();
            type.DictionaryEntry.ShouldBeNull();
        }

        // Dictionaries

        [Test]
        [TestCase(typeof(IDictionary<string, int>))]
        [TestCase(typeof(Dictionary<string, int>))]
        public void should_create_dictionary(Type type)
        {
            should_be_dictionary_type(CreateFactory().BuildGraph(type));
        }

        [Comments("This is a dictionary.")]
        public class DictionaryWithComments : Dictionary<string, int> { }

        [Test]
        public void should_create_dictionary_with_comments()
        {
            should_be_dictionary_type(CreateFactory().BuildGraph(typeof(DictionaryWithComments)),
                comments: "This is a dictionary.");
        }

        [DictionaryDescription("DictionaryName", "This is an dictionary.", 
            "KeyName", "This is a dictionary key.", "This is a dictionary value.")]
        public class DictionaryWithDictionaryComments : Dictionary<string, int> { }

        [Test]
        public void should_create_dictionary_with_dictionary_comments()
        {
            should_be_dictionary_type(CreateFactory().BuildGraph(typeof(DictionaryWithDictionaryComments)),
                name: "DictionaryName",
                comments: "This is an dictionary.",
                keyName: "KeyName",
                keyComments: "This is a dictionary key.", 
                valueComments: "This is a dictionary value.");
        }

        public class DictionaryMember
        {
            public Dictionary<string, int> MemberWithoutComments { get; set; }
            [DictionaryDescription("DictionaryName", "This is a dictionary.",
                "KeyName", "This is a dictionary key.", "This is a dictionary value.")]
            public Dictionary<string, int> MemberWithComments { get; set; }
        }

        [Test]
        public void should_create_dictionary_member_without_comments()
        {
            should_be_dictionary_type(CreateFactory().BuildGraph(typeof(DictionaryMember))
                .Members.Single(x => x.Name == "MemberWithoutComments").Type);
        }

        [Test]
        public void should_create_dictionary_member_with_description()
        {
            should_be_dictionary_type(CreateFactory().BuildGraph(typeof(DictionaryMember))
                .Members.Single(x => x.Name == "DictionaryName").Type,
                comments: "This is a dictionary.",
                keyName: "KeyName",
                keyComments: "This is a dictionary key.", 
                valueComments: "This is a dictionary value.");
        }

        public void should_be_dictionary_type(DataType type, string name = null, string comments = null,
            string keyName = null, string keyComments = null, string valueComments = null)
        {
            type.Name.ShouldEqual(name ?? "DictionaryOfInt");
            type.Comments.ShouldEqual(comments);

            type.IsArray.ShouldBeFalse();
            type.ArrayItem.ShouldBeNull();

            type.IsSimple.ShouldBeFalse();
            type.Options.ShouldBeNull();

            type.IsComplex.ShouldBeFalse();
            type.Members.ShouldBeNull();

            type.IsDictionary.ShouldBeTrue();
            type.DictionaryEntry.ShouldNotBeNull();
            type.DictionaryEntry.KeyName.ShouldEqual(keyName);
            type.DictionaryEntry.KeyComments.ShouldEqual(keyComments);
            should_be_simple_type(type.DictionaryEntry.KeyType, "string");
            type.DictionaryEntry.ValueComments.ShouldEqual(valueComments);
            should_be_simple_type(type.DictionaryEntry.ValueType, "int");
        }

        // Complex types

        public class ComplexType
        {
            public string Member1 { get; set; }
            public string Member2 { get; set; }
        }

        [Test]
        public void should_create_complex_type_and_members()
        {
            var members = should_be_complex_type(CreateFactory().BuildGraph(
                typeof(ComplexType)), 2).Members;

            should_match_member(members[0], "Member1", 
                type: x => should_be_simple_type(x, "string"));
            should_match_member(members[1], "Member2",
                type: x => should_be_simple_type(x, "string"));
        }

        public class ComplexTypeWithMemberComments
        {
            [Comments("This is a member.")]
            public string Member { get; set; }
        }

        [Test]
        public void should_return_complex_type_member_comments()
        {
            var member = should_be_complex_type(CreateFactory().BuildGraph(
                typeof(ComplexTypeWithMemberComments)), 1)
                .Members.Single();

            should_match_member(member, "Member", "This is a member.",
                type: x => should_be_simple_type(x, "string"));
        }

        public class ComplexTypeWithDefaultValue
        {
            [DefaultValue(3.14159)]
            public decimal Member { get; set; }
        }

        [Test]
        public void should_return_complex_type_member_default_value()
        {
            var member = should_be_complex_type(CreateFactory().BuildGraph(
                typeof(ComplexTypeWithDefaultValue)), 1)
                .Members.Single();

            should_match_member(member, "Member",
                defaultValue: "3.14159",
                type: x => should_be_simple_type(x, "decimal"));
        }

        [Test]
        public void should_return_complex_type_member_default_value_with_custom_format()
        {
            var member = should_be_complex_type(CreateFactory(x => x.DefaultValueRealFormat = "0.0").BuildGraph(
                typeof(ComplexTypeWithDefaultValue)), 1)
                .Members.Single();

            should_match_member(member, "Member",
                defaultValue: "3.1",
                type: x => should_be_simple_type(x, "decimal"));
        }

        public class ComplexTypeWithOptionalMember
        {
            [Optional]
            public string OptionalMember { get; set; }
            public int? NullableMember { get; set; }
            public string RequiredMember { get; set; }
            public ComplexTypeChildWithOptionalMember Child { get; set; }
        }

        public class ComplexTypeChildWithOptionalMember
        {
            [Optional]
            public string OptionalMember { get; set; }
            public int? NullableMember { get; set; }
            public string RequiredMember { get; set; }
        }

        public class OptionalMemberPostHandler
        {
            public void Execute(ComplexTypeWithOptionalMember request) { }
        }

        [Test]
        public void should_return_complex_type_optional_member_when_input()
        {
            var action = Behavior.BuildGraph().AddAndGetAction<OptionalMemberPostHandler>();

            var members = should_be_complex_type(CreateFactory().BuildGraph(
                typeof(ComplexTypeWithOptionalMember), action), 4).Members;

            should_match_member(members[0], "OptionalMember",
                required: false,
                optional: true,
                type: x => should_be_simple_type(x, "string"));

            should_match_member(members[1], "NullableMember",
                required: false,
                optional: true,
                type: x => should_be_simple_type(x, "int"));

            should_match_member(members[2], "RequiredMember",
                required: true,
                optional: false,
                type: x => should_be_simple_type(x, "string"));

            should_match_member(members[3].Type.Members[0], "OptionalMember",
                required: false,
                optional: true,
                type: x => should_be_simple_type(x, "string"));

            should_match_member(members[3].Type.Members[1], "NullableMember",
                required: false,
                optional: true,
                type: x => should_be_simple_type(x, "int"));

            should_match_member(members[3].Type.Members[2], "RequiredMember",
                required: true,
                optional: false,
                type: x => should_be_simple_type(x, "string"));
        }

        [Test]
        public void should_not_return_complex_type_optional_member_when_output()
        {
            var members = should_be_complex_type(CreateFactory().BuildGraph(
                typeof(ComplexTypeWithOptionalMember)), 4).Members;

            should_match_member(members[0], "OptionalMember",
                required: false,
                optional: false,
                type: x => should_be_simple_type(x, "string"));

            should_match_member(members[1], "NullableMember",
                required: false,
                optional: false,
                type: x => should_be_simple_type(x, "int"));

            should_match_member(members[2], "RequiredMember",
                required: false,
                optional: false,
                type: x => should_be_simple_type(x, "string"));

            should_match_member(members[3].Type.Members[0], "OptionalMember",
                required: false,
                optional: false,
                type: x => should_be_simple_type(x, "string"));

            should_match_member(members[3].Type.Members[1], "NullableMember",
                required: false,
                optional: false,
                type: x => should_be_simple_type(x, "int"));

            should_match_member(members[3].Type.Members[2], "RequiredMember",
                required: false,
                optional: false,
                type: x => should_be_simple_type(x, "string"));
        }

        public class ProjectionModel
        {
            public Guid Id { get; set; }
            public string UserAgent { get; set; } // Autobound
            public string Name { get; set; }
        }

        public class Projection : Projection<ProjectionModel>
        {
            public Projection() { Value(x => x.Id); }
        }

        [Test]
        public void should_create_complex_type_with_projection_properties()
        {
            var projection = should_be_complex_type(CreateFactory()
                .BuildGraph(typeof(Projection)), 1);

            projection.Members[0].Name.ShouldEqual("Id");
        }

        public class AutoboundModel
        {
            public string UserAgent { get; set; } // Autobound
            public string Name { get; set; }
        }

        [Test]
        public void should_exclude_complex_type_autobound_members()
        {
            should_be_complex_type(CreateFactory()
                .BuildGraph(typeof(Projection)), 1)
                .Members.All(x => x.Name != "UserAgent").ShouldBeTrue();
        }

        public class QuerystringModel
        {
            public string Member { get; set; }
            [QueryString]
            public string QuerystringMember { get; set; }
        }

        public class QuerystringPostHandler
        {
            public void Execute(QuerystringModel request) { } 
        }

        [Test]
        public void should_exclude_complex_type_quertstring_members()
        {
            var action = Behavior.BuildGraph().AddAndGetAction<QuerystringPostHandler>();
            should_be_complex_type(CreateFactory()
                .BuildGraph(typeof(QuerystringModel), action), 1)
                .Members.Single().Name.ShouldEqual("Member");
        }

        public class UrlParameterModel
        {
            public string Member { get; set; }
            public string UrlParameterMember { get; set; }
        }

        public class UrlParameterPostHandler
        {
            public void Execute_UrlParameterMember(UrlParameterModel request) { }
        }

        [Test]
        public void should_exclude_complex_type_url_parameter_members()
        {
            var action = Behavior.BuildGraph().AddAndGetAction<UrlParameterPostHandler>();
            should_be_complex_type(CreateFactory()
                .BuildGraph(typeof(UrlParameterModel), action), 1)
                .Members.Single().Name.ShouldEqual("Member");
        }

        public class CyclicModel
        {
            public string Member { get; set; }
            public CyclicModel CyclicMember { get; set; }
        }

        [Test]
        public void should_exclude_complex_type_cyclic_members()
        {
            should_be_complex_type(CreateFactory()
                .BuildGraph(typeof(CyclicModel)), 1)
                .Members.Single().Name.ShouldEqual("Member");
        }

        public class CyclicArrayModel
        {
            public string Member { get; set; }
            public List<CyclicArrayModel> CyclicMember { get; set; }
        }

        [Test]
        public void should_exclude_complex_type_cyclic_array_members()
        {
            should_be_complex_type(CreateFactory()
                .BuildGraph(typeof(CyclicArrayModel)), 1)
                .Members.Single().Name.ShouldEqual("Member");
        }

        public class CyclicDictionaryModel
        {
            public string Member { get; set; }
            public List<CyclicDictionaryModel> CyclicMember { get; set; }
        }

        [Test]
        public void should_exclude_complex_type_cyclic_dictionary_members()
        {
            should_be_complex_type(CreateFactory()
                .BuildGraph(typeof(CyclicDictionaryModel)), 1)
                .Members.Single().Name.ShouldEqual("Member");
        }

        public class ComplexTypeWithHiddenMember
        {
            [Hide]
            public string HiddenMember { get; set; }
        }

        [Test]
        public void should_exclude_a_member_if_it_is_hidden()
        {
            CreateFactory().BuildGraph(
                typeof(ComplexTypeWithHiddenMember))
                .Members.Any(x => x.Name == "HiddenMember").ShouldBeFalse();
        }

        [Hide]
        public class HiddenType { }

        public class ComplexTypeWithHiddenTypeMember
        {
            public HiddenType HiddenTypeMember { get; set; }
        }

        [Test]
        public void should_exclude_a_member_if_its_type_is_hidden()
        {
            CreateFactory().BuildGraph(
                typeof(ComplexTypeWithHiddenTypeMember))
                .Members.Any(x => x.Name == "HiddenTypeMember").ShouldBeFalse();
        }

        public class ComplexTypeWithDeprecatedMembers
        {
            [Obsolete]
            public string DeprecatedMember { get; set; }

            [Obsolete("DO NOT seek the treasure!")]
            public string DeprecatedMemberWithMessage { get; set; }
        }

        [Test]
        public void should_indicate_if_member_is_deprecated()
        {
            var members = should_be_complex_type(CreateFactory().BuildGraph(
                typeof(ComplexTypeWithDeprecatedMembers)), 2).Members;

            should_match_member(members[0], "DeprecatedMember",
                deprecated: true,
                type: x => should_be_simple_type(x, "string"));
        }

        [Test]
        public void should_indicate_if_member_is_deprecated_with_message()
        {
            var members = should_be_complex_type(CreateFactory().BuildGraph(
                typeof(ComplexTypeWithDeprecatedMembers)), 2).Members;

            should_match_member(members[1], "DeprecatedMemberWithMessage",
                deprecated: true,
                deprecatedMessage: "DO NOT seek the treasure!",
                type: x => should_be_simple_type(x, "string"));
        }

        public void should_match_member(Member member, string name,
            string comments = null, string defaultValue = null,
            bool required = false, bool optional = false,
            bool deprecated = false, string deprecatedMessage = null, 
            Action<DataType> type = null)
        {
            member.Name.ShouldEqual(name);
            member.Comments.ShouldEqual(comments);
            member.DefaultValue.ShouldEqual(defaultValue);
            member.Required.ShouldEqual(required);
            member.Optional.ShouldEqual(optional);
            member.Type.ShouldNotBeNull();
            member.Deprecated.ShouldEqual(deprecated);
            member.DeprecationMessage.ShouldEqual(deprecatedMessage);
            if (type != null) type(member.Type);
        }

        public DataType should_be_complex_type(DataType type, 
            int memberCount, string comments = null)
        {
            type.Name.ShouldEqual(type.Name);
            type.Comments.ShouldEqual(comments);

            type.IsArray.ShouldBeFalse();
            type.ArrayItem.ShouldBeNull();

            type.IsSimple.ShouldBeFalse();
            type.Options.ShouldBeNull();

            type.IsComplex.ShouldBeTrue();
            type.Members.ShouldNotBeNull();
            type.Members.Count.ShouldEqual(memberCount);

            type.IsDictionary.ShouldBeFalse();
            type.DictionaryEntry.ShouldBeNull();

            return type;
        }
    }
}
