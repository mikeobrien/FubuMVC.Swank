using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Swank;
using FubuMVC.Swank.Description;
using FubuMVC.Swank.Extensions;
using FubuMVC.Swank.Specification;
using NUnit.Framework;
using Should;

namespace Tests.Specification
{
    [TestFixture]
    public class DataDescriptionFactoryTests
    {
        public List<DataDescription> BuildDescription(Type type,
            Action<Configuration> configure = null, ActionCall action = null)
        {
            var configuration = new Configuration();
            if (configure != null) configure(configuration);
            return new DataDescriptionFactory(configuration).Create(new TypeGraphFactory(
                configuration,
                new TypeDescriptorCache(),
                new TypeConvention(configuration),
                new MemberConvention(),
                new OptionFactory(configuration, new OptionConvention())).BuildGraph(type, action));
        }

        public List<DataDescription> BuildDescription<T>(
            Action<Configuration> configure = null, ActionCall action = null)
        {
            return BuildDescription(typeof(T), configure, action);
        }

        // Complex types

        [Comments("Complex type comments")]
        public class ComplexTypeWithNoMembers { }

        [Test]
        public void should_create_complex_type()
        {
            var description = BuildDescription<ComplexTypeWithNoMembers>();

            description.Count.ShouldEqual(2);

            description[0].ShouldBeComplexType("ComplexTypeWithNoMembers", 1,
                x => x.Opening().Comments("Complex type comments"));
            
            description[1].ShouldBeComplexType("ComplexTypeWithNoMembers", 1, x => x.Closing());
        }

        public class ComplexTypeWithSimpleMembers
        {
            public string StringMember { get; set; }
            public bool BooleanMember { get; set; }
            public DateTime DateTimeMember { get; set; }
            public TimeSpan DurationMember { get; set; }
            public Guid UuidMember { get; set; }
            public int NumericMember { get; set; }
        }

        [Test]
        public void should_create_complex_type_with_simple_type_members()
        {
            var description = BuildDescription<ComplexTypeWithSimpleMembers>();

            description.Count.ShouldEqual(8);

            description[0].ShouldBeComplexType("ComplexTypeWithSimpleMembers", 1, x => x.Opening());

            description[1].ShouldBeSimpleTypeMember("StringMember", "string", 2, "", x => x.IsString());
            description[2].ShouldBeSimpleTypeMember("BooleanMember", "boolean", 2, "false", x => x.IsBoolean());
            description[3].ShouldBeSimpleTypeMember("DateTimeMember", "dateTime", 2, DateTime.Now.ToString("g"), x => x.IsDateTime());
            description[4].ShouldBeSimpleTypeMember("DurationMember", "duration", 2, "0:00:00", x => x.IsDuration());
            description[5].ShouldBeSimpleTypeMember("UuidMember", "uuid", 2, "00000000-0000-0000-0000-000000000000", x => x.IsGuid());
            description[6].ShouldBeSimpleTypeMember("NumericMember", "int", 2, "0", x => x.IsNumeric(), x => x.IsLastMember());

            description[7].ShouldBeComplexType("ComplexTypeWithSimpleMembers", 1, x => x.Closing());
        }

        public enum Options
        {
            Option, 
            [Comments("This is an option.")]
            OptionWithComments
        }

        public class ComplexTypeWithSimpleOptionMember
        {
            public Options OptionMember { get; set; }
        }

        [Test]
        public void should_create_complex_type_with_simple_numeric_option_member()
        {
            var description = BuildDescription<ComplexTypeWithSimpleOptionMember>();

            description.Count.ShouldEqual(3);

            description[0].ShouldBeComplexType("ComplexTypeWithSimpleOptionMember", 1, x => x.Opening());

            description[1].ShouldBeSimpleTypeMember("OptionMember", "int", 2, "0", 
                x => x.IsNumeric()
                    .WithOption("Option", "0")
                    .WithOptionAndComments("OptionWithComments", "1", "This is an option."), 
                x => x.IsLastMember());

            description[2].ShouldBeComplexType("ComplexTypeWithSimpleOptionMember", 1, x => x.Closing());
        }

        [Test]
        public void should_create_complex_type_with_simple_string_option_member()
        {
            var description = BuildDescription<ComplexTypeWithSimpleOptionMember>(
                x => x.EnumValue = EnumValue.AsString);

            description.Count.ShouldEqual(3);

            description[0].ShouldBeComplexType("ComplexTypeWithSimpleOptionMember", 1, x => x.Opening());

            description[1].ShouldBeSimpleTypeMember("OptionMember", "string", 2, "Option",
                x => x.IsString()
                    .WithOption("Option")
                    .WithOptionAndComments("OptionWithComments", "This is an option."),
                x => x.IsLastMember());

            description[2].ShouldBeComplexType("ComplexTypeWithSimpleOptionMember", 1, x => x.Closing());
        }

        public class ComplexTypeWithOptionalMember
        {
            [Optional]
            public string OptionalMember { get; set; }
            public string RequiredMember { get; set; }
        }

        public class OptionalMemberPostHandler
        {
            public void Execute(ComplexTypeWithOptionalMember request) { }
        }

        [Test]
        public void should_create_complex_type_with_optional_members()
        {
            var action = Behavior.BuildGraph().AddAndGetAction<OptionalMemberPostHandler>();

            var description = BuildDescription<ComplexTypeWithOptionalMember>(action: action);

            description.Count.ShouldEqual(4);

            description[0].ShouldBeComplexType("ComplexTypeWithOptionalMember", 1, x => x.Opening());

            description[1].ShouldBeSimpleTypeMember("OptionalMember", "string", 2, "", x => x.IsString(),
                x => x.Optional());

            description[2].ShouldBeSimpleTypeMember("RequiredMember", "string", 2, "", x => x.IsString(),
                x => x.Required().IsLastMember());

            description[3].ShouldBeComplexType("ComplexTypeWithOptionalMember", 1, x => x.Closing());
        }

        public class ComplexTypeWithDeprecatedMember
        {
            [Obsolete("Why u no use different one??")]
            public string DeprecatedMember { get; set; }
        }

        [Test]
        public void should_create_complex_type_with_deprecated_members()
        {
            var description = BuildDescription<ComplexTypeWithDeprecatedMember>();

            description.Count.ShouldEqual(3);

            description[0].ShouldBeComplexType("ComplexTypeWithDeprecatedMember", 1, x => x.Opening());

            description[1].ShouldBeSimpleTypeMember("DeprecatedMember", "string", 2, "", x => x.IsString(),
                x => x.IsDeprecated("Why u no use different one??").IsLastMember());

            description[2].ShouldBeComplexType("ComplexTypeWithDeprecatedMember", 1, x => x.Closing());
        }

        public class ComplexTypeWithArrayMembers
        {
            public List<string> ArrayMember { get; set; }
        }

        public class ComplexTypeWithArrayMembersWithCustomItemName
        {
            [XmlArrayItem("Item")]
            public List<string> ArrayMember { get; set; }
        }

        [Test]
        [TestCase(typeof(ComplexTypeWithArrayMembers), "string")]
        [TestCase(typeof(ComplexTypeWithArrayMembersWithCustomItemName), "Item")]
        public void should_create_complex_type_with_array_members(
            Type type, string itemName)
        {
            var description = BuildDescription(type);

            description.Count.ShouldEqual(5);
            description[0].ShouldBeComplexType(type.Name, 1, x => x.Opening());

            description[1].ShouldBeArrayTypeMember("ArrayMember", 2, true, x => x.IsLastMember());

            description[2].ShouldBeSimpleType(itemName, "string", 3, "", x => x.IsString());

            description[3].ShouldBeArrayTypeMember("ArrayMember", 2, false, x => x.IsLastMember());

            description[4].ShouldBeComplexType(type.Name, 1, x => x.Closing());
        }

        public class ComplexTypeWithDictionaryMember
        {
            [DictionaryComments("This is a dictionary", 
                "This is the key.", "This is the value.")]
            public Dictionary<string, string> DictionaryMember { get; set; }
        }

        [Test]
        public void should_create_complex_type_with_dictionary_members()
        {
            var description = BuildDescription<ComplexTypeWithDictionaryMember>();

            description.Count.ShouldEqual(5);

            description[0].ShouldBeComplexType("ComplexTypeWithDictionaryMember", 1, x => x.Opening());

            description[1].ShouldBeDictionaryTypeMember("DictionaryMember", 2, true,
                x => x.Comments("This is a dictionary").IsLastMember());

            //should_be_a_dictionary_entry(description[2], itemName, SimpleType.String, 3, "");

            description[3].ShouldBeDictionaryTypeMember("DictionaryMember", 2, false, x => x.IsLastMember());

            description[4].ShouldBeComplexType("", 1, x => x.Closing());
        }

        // Arrays

        // Dictionaries
    }

    public static class DataDescriptionAssertions
    {
        public class ComplexTypeDsl
        {
            private readonly DataDescription _data;
            public ComplexTypeDsl(DataDescription data) { _data = data; }
            public ComplexTypeDsl Comments(string comments) { _data.Comments = comments; return this; }
            public ComplexTypeDsl Opening() { _data.IsOpening = true; return this; }
            public ComplexTypeDsl Closing() { _data.IsClosing = true; return this; }
        }

        public static void ShouldBeComplexType(this DataDescription source, 
            string name, int level, Action<ComplexTypeDsl> properties)
        {
            var compare = new DataDescription
            {
                Name = name, 
                IsComplexType = true,
                Whitespace = DataDescriptionFactory.Whitespace.Repeat(level)
            };
            properties(new ComplexTypeDsl(compare));
            source.ShouldMatchData(compare);
        }

        public class SimpleTypeDsl
        {
            private readonly DataDescription _data;
            public SimpleTypeDsl(DataDescription data) { _data = data; }
            public SimpleTypeDsl IsString() { _data.IsString = true; return this; }
            public SimpleTypeDsl IsBoolean() { _data.IsBoolean = true; return this; }
            public SimpleTypeDsl IsNumeric() { _data.IsNumeric = true; return this; }
            public SimpleTypeDsl IsDateTime() { _data.IsDateTime = true; return this; }
            public SimpleTypeDsl IsDuration() { _data.IsDuration = true; return this; }
            public SimpleTypeDsl IsGuid() { _data.IsGuid = true; return this; }

            public SimpleTypeDsl WithOption(string value)
            {
                WithOption(new Option { Value = value }); return this;
            }

            public SimpleTypeDsl WithOption(string name, string value)
            {
                WithOption(new Option { Name = name, Value = value }); return this;
            }

            public SimpleTypeDsl WithOptionAndComments(string value, string comments)
            {
                WithOption(new Option { Value = value, Comments = comments }); return this;
            }

            public SimpleTypeDsl WithOptionAndComments(string name, string value, string comments)
            {
                WithOption(new Option { Name = name, Value = value, Comments = comments }); return this;
            }

            private SimpleTypeDsl WithOption(Option option)
            {
                if (_data.Options == null) _data.Options = new List<Option>();
                _data.Options.Add(option); return this;
            }
        }

        public class MemberDsl
        {
            private readonly DataDescription _data;
            public MemberDsl(DataDescription data) { _data = data; }
            public MemberDsl Comments(string comments) { _data.Comments = comments; return this; }
            public MemberDsl Required() { _data.Required = true; return this; }
            public MemberDsl Optional() { _data.Optional = true; return this; }
            public MemberDsl IsLastMember() { _data.IsLastMember = true; return this; }

            public MemberDsl IsDeprecated(string message = null)
            {
                _data.IsDeprecated = true;
                _data.DeprecationMessage = message;
                return this;
            }
        }

        public static void ShouldBeSimpleType(this DataDescription source,
            string name, string typeName, int level, string defaultValue,
            Action<SimpleTypeDsl> simpleTypeProperties)
        {
            var compare = new DataDescription
            {
                Name = name,
                TypeName = typeName,
                IsSimpleType = true,
                DefaultValue = defaultValue,
                Whitespace = DataDescriptionFactory.Whitespace.Repeat(level)
            };
            simpleTypeProperties(new SimpleTypeDsl(compare));
            source.ShouldMatchData(compare);
        }

        public static void ShouldBeSimpleTypeMember(this DataDescription source,
            string name, string typeName, int level, string defaultValue,
            Action<SimpleTypeDsl> simpleTypeProperties,
            Action<MemberDsl> memberProperties = null)
        {
            var compare = new DataDescription
            {
                Name = name,
                TypeName = typeName,
                IsMember = true,
                IsSimpleType = true,
                DefaultValue = defaultValue,
                Whitespace = DataDescriptionFactory.Whitespace.Repeat(level)
            };
            simpleTypeProperties(new SimpleTypeDsl(compare));
            if (memberProperties != null) memberProperties(new MemberDsl(compare));
            source.ShouldMatchData(compare);
        }

        public static void ShouldBeArrayTypeMember(
            this DataDescription source, string name, int level, bool opening,
            Action<MemberDsl> memberProperties = null)
        {
            var compare = new DataDescription
            {
                Name = name,
                IsMember = true,
                IsArray = true,
                Whitespace = DataDescriptionFactory.Whitespace.Repeat(level)
            };
            if (opening) compare.IsOpening = true;
            else compare.IsClosing = true;
            if (memberProperties != null) memberProperties(new MemberDsl(compare));
            source.ShouldMatchData(compare);
        }

        public static void ShouldBeDictionaryTypeMember(
            this DataDescription source, string name, int level, bool opening,
            Action<MemberDsl> memberProperties = null)
        {
            var compare = new DataDescription
            {
                Name = name,
                IsMember = true,
                IsDictionary = true,
                Whitespace = DataDescriptionFactory.Whitespace.Repeat(level)
            };
            if (opening) compare.IsOpening = true;
            else compare.IsClosing = true;
            if (memberProperties != null) memberProperties(new MemberDsl(compare));
            source.ShouldMatchData(compare);
        }

        public static void ShouldBeDictionaryEntry(
            this DataDescription source, string name, int level, bool opening,
            Action<MemberDsl> memberProperties = null)
        {
            var compare = new DataDescription
            {
                Name = name,
                IsMember = true,
                IsDictionary = true,
                Whitespace = DataDescriptionFactory.Whitespace.Repeat(level)
            };
            if (opening) compare.IsOpening = true;
            else compare.IsClosing = true;
            if (memberProperties != null) memberProperties(new MemberDsl(compare));
            source.ShouldMatchData(compare);
        }

        private static void ShouldMatchData(this DataDescription source, DataDescription compare)
        {
            source.Name.ShouldEqual(compare.Name);
            source.Comments.ShouldEqual(compare.Comments);
            source.TypeName.ShouldEqual(compare.TypeName);
            source.DefaultValue.ShouldEqual(compare.DefaultValue);
            source.Required.ShouldEqual(compare.Required);
            source.Optional.ShouldEqual(compare.Optional);
            source.Whitespace.ShouldEqual(compare.Whitespace);
            source.IsDeprecated.ShouldEqual(compare.IsDeprecated);
            source.DeprecationMessage.ShouldEqual(compare.DeprecationMessage);

            source.IsOpening.ShouldEqual(compare.IsOpening);
            source.IsClosing.ShouldEqual(compare.IsClosing);

            source.IsMember.ShouldEqual(compare.IsMember);
            source.IsLastMember.ShouldEqual(compare.IsLastMember);

            source.IsSimpleType.ShouldEqual(compare.IsSimpleType);
            source.IsString.ShouldEqual(compare.IsString);
            source.IsBoolean.ShouldEqual(compare.IsBoolean);
            source.IsNumeric.ShouldEqual(compare.IsNumeric);
            source.IsDateTime.ShouldEqual(compare.IsDateTime);
            source.IsDuration.ShouldEqual(compare.IsDuration);
            source.IsGuid.ShouldEqual(compare.IsGuid);
            compare.Options.ShouldEqualOptions(source.Options);

            source.IsComplexType.ShouldEqual(compare.IsComplexType);

            source.IsArray.ShouldEqual(compare.IsArray);

            source.IsDictionary.ShouldEqual(compare.IsDictionary);
            source.IsDictionaryEntry.ShouldEqual(compare.IsDictionaryEntry);

            if (compare.DictionaryKey == null) source.DictionaryKey.ShouldBeNull();
            else
            {
                source.DictionaryKey.Type.ShouldEqual(compare.DictionaryKey.Type);
                source.DictionaryKey.Comments.ShouldEqual(compare.DictionaryKey.Comments);
                source.DictionaryKey.Options.ShouldEqualOptions(compare.Options);
            }
        }

        private static void ShouldEqualOptions(this List<Option> source, List<Option> compare)
        {
            if (compare == null) source.ShouldBeNull();
            else
            {
                source.Count.ShouldEqual(compare.Count);
                foreach (var option in source.Zip(compare,
                    (s, c) => new { Source = s, Compare = c }))
                {
                    option.Source.Name.ShouldEqual(option.Compare.Name);
                    option.Source.Comments.ShouldEqual(option.Compare.Comments);
                    option.Source.Value.ShouldEqual(option.Compare.Value);
                }
            }
        }
    }
}
