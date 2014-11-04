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
    public class BodyDescriptionFactoryTests
    {
        public List<BodyLineItem> BuildDescription(Type type,
            Action<Configuration> configure = null, ActionCall action = null)
        {
            var configuration = new Configuration();
            if (configure != null) configure(configuration);
            return new BodyDescriptionFactory(configuration).Create(new TypeGraphFactory(
                configuration,
                new TypeDescriptorCache(),
                new TypeConvention(configuration),
                new MemberConvention(),
                new OptionFactory(configuration, new OptionConvention())).BuildGraph(type, action));
        }

        public List<BodyLineItem> BuildDescription<T>(
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

            description.ShouldBeIndexed().ShouldTotal(2);

            description[0].ShouldBeComplexType("ComplexTypeWithNoMembers", 0,
                x => x.First().Opening().Comments("Complex type comments").EmptyNamespace());

            description[1].ShouldBeComplexType("ComplexTypeWithNoMembers", 0,
                x => x.Last().Closing());
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

            description.ShouldBeIndexed().ShouldTotal(8);

            description[0].ShouldBeComplexType("ComplexTypeWithSimpleMembers", 0, 
                x => x.First().Opening().EmptyNamespace());

            description[1].ShouldBeSimpleTypeMember("StringMember", 
                "string", 1, "", x => x.IsString());
            description[2].ShouldBeSimpleTypeMember("BooleanMember", 
                "boolean", 1, "false", x => x.IsBoolean());
            description[3].ShouldBeSimpleTypeMember("DateTimeMember", 
                "dateTime", 1, DateTime.Now.ToString("g"), x => x.IsDateTime());
            description[4].ShouldBeSimpleTypeMember("DurationMember", 
                "duration", 1, "0:00:00", x => x.IsDuration());
            description[5].ShouldBeSimpleTypeMember("UuidMember", 
                "uuid", 1, "00000000-0000-0000-0000-000000000000", x => x.IsGuid());
            description[6].ShouldBeSimpleTypeMember("NumericMember", 
                "int", 1, "0", x => x.IsNumeric(), x => x.IsLastMember());

            description[7].ShouldBeComplexType("ComplexTypeWithSimpleMembers", 0, 
                x => x.Last().Closing());
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

            description.ShouldBeIndexed().ShouldTotal(3);

            description[0].ShouldBeComplexType("ComplexTypeWithSimpleOptionMember", 0, 
                x => x.First().Opening().EmptyNamespace());

            description[1].ShouldBeSimpleTypeMember("OptionMember", "int", 1, "0", 
                x => x.IsNumeric()
                    .Options
                        .WithOption("Option", "0")
                        .WithOptionAndComments("OptionWithComments", "1", "This is an option."), 
                x => x.IsLastMember());

            description[2].ShouldBeComplexType("ComplexTypeWithSimpleOptionMember", 0, 
                x => x.Last().Closing());
        }

        [Test]
        public void should_create_complex_type_with_simple_string_option_member()
        {
            var description = BuildDescription<ComplexTypeWithSimpleOptionMember>(
                x => x.EnumFormat = EnumFormat.AsString);

            description.ShouldBeIndexed().ShouldTotal(3);

            description[0].ShouldBeComplexType("ComplexTypeWithSimpleOptionMember", 0, 
                x => x.First().Opening().EmptyNamespace());

            description[1].ShouldBeSimpleTypeMember("OptionMember", "string", 1, "Option",
                x => x.IsString()
                    .Options
                        .WithOption("Option")
                        .WithOptionAndComments("OptionWithComments", "This is an option."),
                x => x.IsLastMember());

            description[2].ShouldBeComplexType("ComplexTypeWithSimpleOptionMember", 0, 
                x => x.Last().Closing());
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

            description.ShouldBeIndexed().ShouldTotal(4);

            description[0].ShouldBeComplexType("ComplexTypeWithOptionalMember", 0, 
                x => x.First().Opening().EmptyNamespace());

            description[1].ShouldBeSimpleTypeMember("OptionalMember", 
                "string", 1, "", x => x.IsString(),
                x => x.Optional());

            description[2].ShouldBeSimpleTypeMember("RequiredMember", 
                "string", 1, "", x => x.IsString(),
                x => x.Required().IsLastMember());

            description[3].ShouldBeComplexType("ComplexTypeWithOptionalMember", 0, 
                x => x.Last().Closing());
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

            description.ShouldBeIndexed().ShouldTotal(3);

            description[0].ShouldBeComplexType("ComplexTypeWithDeprecatedMember", 0, 
                x => x.First().Opening().EmptyNamespace());

            description[1].ShouldBeSimpleTypeMember("DeprecatedMember", 
                "string", 1, "", x => x.IsString(),
                x => x.IsDeprecated("Why u no use different one??").IsLastMember());

            description[2].ShouldBeComplexType("ComplexTypeWithDeprecatedMember", 0, 
                x => x.Last().Closing());
        }

        public class DefaultValueMemberPostHandler
        {
            public void Execute(ComplexTypeWithDefaultValueMember request) { }
        }

        public class ComplexTypeWithDefaultValueMember
        {
            [Optional, DefaultValue("zero")]
            public string DefaultValueMember { get; set; }
        }

        [Test]
        public void should_create_complex_type_with_default_value_members()
        {
            var action = Behavior.BuildGraph().AddAndGetAction<DefaultValueMemberPostHandler>();

            var description = BuildDescription<ComplexTypeWithDefaultValueMember>(action: action);

            description.ShouldBeIndexed().ShouldTotal(3);

            description[0].ShouldBeComplexType("ComplexTypeWithDefaultValueMember", 0, 
                x => x.First().Opening().EmptyNamespace());

            description[1].ShouldBeSimpleTypeMember("DefaultValueMember", 
                "string", 1, "", x => x.IsString(),
                x => x.Default("zero").Optional().IsLastMember());

            description[2].ShouldBeComplexType("ComplexTypeWithDefaultValueMember", 0, 
                x => x.Last().Closing());
        }

        public class ComplexTypeWithSampleValueMember
        {
            [SampleValue("zero")]
            public string SampleValueMember { get; set; }
        }

        [Test]
        public void should_create_complex_type_with_sample_value_members()
        {
            var description = BuildDescription<ComplexTypeWithSampleValueMember>();

            description.ShouldBeIndexed().ShouldTotal(3);

            description[0].ShouldBeComplexType("ComplexTypeWithSampleValueMember", 0, 
                x => x.First().Opening().EmptyNamespace());

            description[1].ShouldBeSimpleTypeMember("SampleValueMember", 
                "string", 1, "zero", x => x.IsString(), x => x.IsLastMember());

            description[2].ShouldBeComplexType("ComplexTypeWithSampleValueMember", 0, 
                x => x.Last().Closing());
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

            description.ShouldBeIndexed().ShouldTotal(5);
            description[0].ShouldBeComplexType(type.Name, 0, x => 
                x.First().Opening().EmptyNamespace());

            description[1].ShouldBeArrayMember("ArrayMember", 1,
                x => x.Opening().LongNamespace(type.Name).ShortNamespace(), 
                x => x.IsLastMember());

            description[2].ShouldBeSimpleType(itemName, 
                "string", 2, "", x => x.IsString());

            description[3].ShouldBeArrayMember("ArrayMember", 1, 
                x => x.Closing(), x => x.IsLastMember());

            description[4].ShouldBeComplexType(type.Name, 0, 
                x => x.Last().Closing());
        }

        public class ComplexTypeWithDictionaryMember
        {
            [DictionaryDescription("Entries", "This is a dictionary.",
                "KeyName", "This is a dictionary key.",
                "This is a dictionary value.")]
            public Dictionary<string, string> DictionaryMember { get; set; }
        }

        [Test]
        public void should_create_complex_type_with_dictionary_members()
        {
            var description = BuildDescription<ComplexTypeWithDictionaryMember>();

            description.ShouldBeIndexed().ShouldTotal(5);

            description[0].ShouldBeComplexType("ComplexTypeWithDictionaryMember", 0, 
                x => x.First().Opening().EmptyNamespace());

            description[1].ShouldBeDictionaryMember("Entries", 1,
                x => x.Opening().LongNamespace("ComplexTypeWithDictionaryMember").ShortNamespace(),
                x => x.Comments("This is a dictionary.").IsLastMember());

            description[2].ShouldBeSimpleTypeDictionaryEntry("KeyName", "string", "string", 2, "",
               x => x.IsString().Comments("This is a dictionary value."),
               x => x.KeyComments("This is a dictionary key."));

            description[3].ShouldBeDictionaryMember("Entries", 1, x => x.Closing(), 
                x => x.IsLastMember());

            description[4].ShouldBeComplexType("ComplexTypeWithDictionaryMember", 0, 
                x => x.Last().Closing());
        }

        // Arrays

        [ArrayDescription("Items", "This is an array", 
            "Item", "This is an array item.")]
        public class ArrayType : List<string> { }

        [Test]
        public void should_create_an_array_with_a_description()
        {
            var description = BuildDescription<ArrayType>();

            description.ShouldBeIndexed().ShouldTotal(3);

            description[0].ShouldBeArray("Items", 0, 
                x => x.Comments("This is an array").First().Opening().EmptyNamespace());

            description[1].ShouldBeSimpleType("Item", "string", 1, "", x => x
                .Comments("This is an array item.").IsString());

            description[2].ShouldBeArray("Items", 0, 
                x => x.Last().Closing());
        }

        public enum ArrayOptions { Option1, Option2 }

        [Test]
        public void should_create_an_array_of_numeric_options()
        {
            var description = BuildDescription<List<ArrayOptions>>();

            description.ShouldBeIndexed().ShouldTotal(3);

            description[0].ShouldBeArray("ArrayOfInt", 0, 
                x => x.First().Opening().EmptyNamespace());

            description[1].ShouldBeSimpleType("int", "int", 1, "0", 
                x => x.IsNumeric()
                    .Options
                        .WithOption("Option1", "0")
                        .WithOption("Option2", "1")); 

            description[2].ShouldBeArray("ArrayOfInt", 0, 
                x => x.Last().Closing());
        }

        [Test]
        public void should_create_an_array_of_string_options()
        {
            var description = BuildDescription<List<ArrayOptions>>(
                x => x.EnumFormat = EnumFormat.AsString);

            description.ShouldBeIndexed().ShouldTotal(3);

            description[0].ShouldBeArray("ArrayOfString", 0,
                x => x.First().Opening().EmptyNamespace());

            description[1].ShouldBeSimpleType("string", "string", 1, "Option1", 
                x => x.IsString()
                    .Options
                        .WithOption("Option1")
                        .WithOption("Option2"));

            description[2].ShouldBeArray("ArrayOfString", 0, x => x.Last().Closing());
        }

        public class ArrayComplexType { public string Member { get; set; } }

        [Test]
        public void should_create_an_array_of_complex_types()
        {
            var description = BuildDescription<List<ArrayComplexType>>();

            description.ShouldBeIndexed().ShouldTotal(5);

            description[0].ShouldBeArray("ArrayOfArrayComplexType", 0,
                x => x.First().Opening().EmptyNamespace());

            description[1].ShouldBeComplexType("ArrayComplexType", 1,
                x => x.Opening().LongNamespace("ArrayOfArrayComplexType").ShortNamespace());

            description[2].ShouldBeSimpleTypeMember("Member", "string", 2, "", 
                x => x.IsString(), x => x.IsLastMember());

            description[3].ShouldBeComplexType("ArrayComplexType", 1, 
                x => x.Closing());

            description[4].ShouldBeArray("ArrayOfArrayComplexType", 0, 
                x => x.Last().Closing());
        }

        [Test]
        public void should_create_an_array_of_arrays()
        {
            var description = BuildDescription<List<List<string>>>();

            description.ShouldBeIndexed().ShouldTotal(5);

            description[0].ShouldBeArray("ArrayOfArrayOfString", 0, 
                x => x.First().Opening().EmptyNamespace());

            description[1].ShouldBeArray("ArrayOfString", 1,
                x => x.Opening().LongNamespace("ArrayOfArrayOfString").ShortNamespace());

            description[2].ShouldBeSimpleType("string", "string", 2, "", x => x.IsString());

            description[3].ShouldBeArray("ArrayOfString", 1, x => x.Closing());

            description[4].ShouldBeArray("ArrayOfArrayOfString", 0, x => x.Last().Closing());
        }

        [Test]
        public void should_create_an_array_of_dictionaries()
        {
            var description = BuildDescription<List<Dictionary<string, int>>>();

            description.ShouldBeIndexed().ShouldTotal(5);

            description[0].ShouldBeArray("ArrayOfDictionaryOfInt", 0, 
                x => x.First().Opening().EmptyNamespace());

            description[1].ShouldBeDictionary("DictionaryOfInt", 1,
                x => x.Opening().LongNamespace("ArrayOfDictionaryOfInt").ShortNamespace());

            description[2].ShouldBeSimpleTypeDictionaryEntry("key", "string", "int", 2, "0",
                x => x.IsNumeric());

            description[3].ShouldBeDictionary("DictionaryOfInt", 1, x => x.Closing());

            description[4].ShouldBeArray("ArrayOfDictionaryOfInt", 0, x => x.Last().Closing());
        }

        // Dictionaries

        [DictionaryDescription("Entries", "This is a dictionary.",
            "KeyName", "This is a dictionary key.", 
            "This is a dictionary value.")]
        public class DictionaryType : Dictionary<string, int> { }

        [Test]
        public void should_create_a_dictionary_with_a_description()
        {
            var description = BuildDescription<DictionaryType>();

            description.ShouldBeIndexed().ShouldTotal(3);

            description[0].ShouldBeDictionary("Entries", 0, x => x
                .Comments("This is a dictionary.").First().Opening().EmptyNamespace());

            description[1].ShouldBeSimpleTypeDictionaryEntry("KeyName", "string", "int", 1, "0",
                x => x.IsNumeric().Comments("This is a dictionary value."), 
                x => x.KeyComments("This is a dictionary key."));

            description[2].ShouldBeDictionary("Entries", 0, x => x.Last().Closing());
        }

        public enum DictionaryKeyOptions { KeyOption1, KeyOption2 }
        public enum DictionaryValueOptions { ValueOption1, ValueOption2 }

        [Test]
        public void should_create_an_dictionary_of_numeric_options()
        {
            var description = BuildDescription<Dictionary<DictionaryKeyOptions, DictionaryValueOptions>>();

            description.ShouldBeIndexed().ShouldTotal(3);

            description[0].ShouldBeDictionary("DictionaryOfInt", 0, 
                x => x.First().Opening().EmptyNamespace());

            description[1].ShouldBeSimpleTypeDictionaryEntry("key", "int", "int", 1, "0",
                x => x.IsNumeric()
                    .Options
                        .WithOption("ValueOption1", "0")
                        .WithOption("ValueOption2", "1"),
                x => x.KeyOptions
                    .WithOption("KeyOption1", "0")
                    .WithOption("KeyOption2", "1"));

            description[2].ShouldBeDictionary("DictionaryOfInt", 0, x => x.Last().Closing());
        }

        [Test]
        public void should_create_an_dictionary_of_string_options()
        {
            var description = BuildDescription<Dictionary<DictionaryKeyOptions, DictionaryValueOptions>>(x => x.EnumFormat = EnumFormat.AsString);

            description.ShouldBeIndexed().ShouldTotal(3);

            description[0].ShouldBeDictionary("DictionaryOfString", 0, 
                x => x.First().Opening().EmptyNamespace());

            description[1].ShouldBeSimpleTypeDictionaryEntry("key", "string", "string", 1, "ValueOption1",
                x => x.IsString()
                    .Options
                        .WithOption("ValueOption1")
                        .WithOption("ValueOption2"),
                x => x.KeyOptions
                    .WithOption("KeyOption1")
                    .WithOption("KeyOption2"));

            description[2].ShouldBeDictionary("DictionaryOfString", 0, x => x.Last().Closing());
        }
        public class DictionaryComplexType { public string Member { get; set; } }

        [Test]
        public void should_create_a_dictionary_of_complex_types()
        {
            var description = BuildDescription<Dictionary<string, DictionaryComplexType>>();

            description.ShouldBeIndexed().ShouldTotal(5);

            description[0].ShouldBeDictionary("DictionaryOfDictionaryComplexType", 0, 
                x => x.First().Opening().EmptyNamespace());

            description[1].ShouldBeOpeningComplexTypeDictionaryEntry("key", "string", 1,
                x => x.LongNamespace("DictionaryOfDictionaryComplexType").ShortNamespace());

            description[2].ShouldBeSimpleTypeMember("Member", "string", 2, "",
                x => x.IsString(), x => x.IsLastMember());

            description[3].ShouldBeClosingComplexTypeDictionaryEntry("key", 1);

            description[4].ShouldBeDictionary("DictionaryOfDictionaryComplexType", 0, x => x.Last().Closing());
        }

        [Test]
        public void should_create_a_dictionary_of_arrays()
        {
            var description = BuildDescription<Dictionary<string, List<int>>>();

            description.ShouldBeIndexed().ShouldTotal(5);

            description[0].ShouldBeDictionary("DictionaryOfArrayOfInt", 0, 
                x => x.First().Opening().EmptyNamespace());

            description[1].ShouldBeOpeningArrayDictionaryEntry("key", "string", 1,
                x => x.LongNamespace("DictionaryOfArrayOfInt").ShortNamespace());

            description[2].ShouldBeSimpleType("int", "int", 2, "0", x => x.IsNumeric());

            description[3].ShouldBeClosingArrayDictionaryEntry("key", 1);

            description[4].ShouldBeDictionary("DictionaryOfArrayOfInt", 0, x => x.Last().Closing());
        }

        [Test]
        public void should_create_a_dictionary_of_dictionaries()
        {
            var description = BuildDescription<Dictionary<string, Dictionary<string, int>>>();

            description.ShouldBeIndexed().ShouldTotal(5);

            description[0].ShouldBeDictionary("DictionaryOfDictionaryOfInt", 0,
                x => x.First().Opening().EmptyNamespace());

            description[1].ShouldBeOpeningDictionaryDictionaryEntry("key", "string", 1,
                x => x.LongNamespace("DictionaryOfDictionaryOfInt").ShortNamespace());

            description[2].ShouldBeSimpleTypeDictionaryEntry("key", "string", "int", 2, "0",
                x => x.IsNumeric());

            description[3].ShouldBeClosingDictionaryDictionaryEntry("key", 1);

            description[4].ShouldBeDictionary("DictionaryOfDictionaryOfInt", 0, x => x.Last().Closing());
        }
    }

    public static class DataDescriptionAssertions
    {
        // Simple type assertions

        public static void ShouldBeSimpleType(this BodyLineItem source,
            string name, string typeName, int level, string sampleValue,
            Action<SimpleTypeDsl> simpleTypeProperties)
        {
            source.ShouldMatchLineItem(CreateSimpleType(name, typeName, 
                level, sampleValue, simpleTypeProperties));
        }

        public static void ShouldBeSimpleTypeMember(this BodyLineItem source,
            string name, string typeName, int level, string sampleValue,
            Action<SimpleTypeDsl> simpleTypeProperties,
            Action<MemberDsl> memberProperties = null)
        {
            var compare = CreateSimpleType(name, typeName, 
                level, sampleValue, simpleTypeProperties);
            compare.IsMember = true;
            if (memberProperties != null) memberProperties(new MemberDsl(compare));
            source.ShouldMatchLineItem(compare);
        }

        public static void ShouldBeSimpleTypeDictionaryEntry(this BodyLineItem source,
            string name, string keyTypeName, string valueTypeName, int level, string sampleValue,
            Action<SimpleTypeDsl> simpleTypeProperties,
            Action<DictionaryKeyDsl> dictionaryEntryProperties = null)
        {
            var compare = CreateSimpleType(name, valueTypeName,
                level, sampleValue, simpleTypeProperties);
            compare.IsDictionaryEntry = true;
            compare.DictionaryKey = new Key { TypeName = keyTypeName };
            if (dictionaryEntryProperties != null) 
                dictionaryEntryProperties(new DictionaryKeyDsl(compare));
            source.ShouldMatchLineItem(compare);
        }

        private static BodyLineItem CreateSimpleType(
            string name, string typeName, int level, string sampleValue,
            Action<SimpleTypeDsl> simpleTypeProperties)
        {
            var simpleType = new BodyLineItem
            {
                Name = name,
                TypeName = typeName,
                IsSimpleType = true,
                SampleValue = sampleValue,
                Whitespace = BodyDescriptionFactory.Whitespace.Repeat(level)
            };
            simpleTypeProperties(new SimpleTypeDsl(simpleType));
            return simpleType;
        }

        public class SimpleTypeDsl
        {
            private readonly BodyLineItem _body;
            public SimpleTypeDsl(BodyLineItem body) { _body = body; }
            public SimpleTypeDsl Comments(string comments) { _body.Comments = comments; return this; }
            public SimpleTypeDsl IsString() { _body.IsString = true; return this; }
            public SimpleTypeDsl IsBoolean() { _body.IsBoolean = true; return this; }
            public SimpleTypeDsl IsNumeric() { _body.IsNumeric = true; return this; }
            public SimpleTypeDsl IsDateTime() { _body.IsDateTime = true; return this; }
            public SimpleTypeDsl IsDuration() { _body.IsDuration = true; return this; }
            public SimpleTypeDsl IsGuid() { _body.IsGuid = true; return this; }

            public OptionDsl Options
            {
                get { return new OptionDsl(_body.Options = _body.Options ?? new List<Option>()); }
            }
        }

        // Array assertions

        public static void ShouldBeArray(
            this BodyLineItem source, string name, int level,
            Action<ArrayDsl> properties)
        {
            source.ShouldMatchLineItem(CreateArray(name, level, properties));
        }

        public static void ShouldBeArrayMember(
            this BodyLineItem source, string name, int level,
            Action<ArrayDsl> arrayProperties,
            Action<MemberDsl> memberProperties = null)
        {
            var compare = CreateArray(name, level, arrayProperties);
            compare.IsMember = true;
            if (memberProperties != null) memberProperties(new MemberDsl(compare));
            source.ShouldMatchLineItem(compare);
        }

        public static void ShouldBeOpeningArrayDictionaryEntry(
            this BodyLineItem source, string name, string keyTypeName, int level,
            Action<ArrayDsl> arrayProperties = null,
            Action<DictionaryKeyDsl> dictionaryKeyProperties = null)
        {
            var compare = CreateArray(name, level, arrayProperties);
            compare.IsOpening = true;
            compare.IsDictionaryEntry = true;
            compare.DictionaryKey = new Key { TypeName = keyTypeName };
            if (dictionaryKeyProperties != null)
                dictionaryKeyProperties(new DictionaryKeyDsl(compare));
            source.ShouldMatchLineItem(compare);
        }

        public static void ShouldBeClosingArrayDictionaryEntry(
            this BodyLineItem source, string name, int level,
            Action<ArrayDsl> arrayProperties = null)
        {
            var compare = CreateArray(name, level, arrayProperties);
            compare.IsClosing = true;
            compare.IsDictionaryEntry = true;
            source.ShouldMatchLineItem(compare);
        }

        private static BodyLineItem CreateArray(
            string name, int level, Action<ArrayDsl> properties)
        {
            var arrayType = new BodyLineItem
            {
                Name = name,
                IsArray = true,
                Whitespace = BodyDescriptionFactory.Whitespace.Repeat(level)
            };
            if (properties != null) properties(new ArrayDsl(arrayType));
            return arrayType;
        }

        public class ArrayDsl
        {
            private readonly BodyLineItem _body;
            public ArrayDsl(BodyLineItem body) { _body = body; }
            public ArrayDsl Comments(string comments) { _body.Comments = comments; return this; }
            public ArrayDsl Opening() { _body.IsOpening = true; return this; }
            public ArrayDsl Closing() { _body.IsClosing = true; return this; }
            public ArrayDsl First() { _body.IsFirst = true; return this; }
            public ArrayDsl Last() { _body.IsLast = true; return this; }

            public ArrayDsl EmptyNamespace()
            {
                _body.LongNamespace = _body.ShortNamespace = new List<string>(); return this;
            }

            public ArrayDsl LongNamespace(params string[] @namespace)
            {
                _body.LongNamespace = @namespace.ToList(); return this;
            }

            public ArrayDsl ShortNamespace(params string[] @namespace)
            {
                _body.ShortNamespace = @namespace.ToList(); return this;
            }
        }

        // Dictionary assertions

        public static void ShouldBeDictionary(
            this BodyLineItem source, string name, int level,
            Action<DictionaryDsl> properties)
        {
            source.ShouldMatchLineItem(CreateDictionary(name, level, properties));
        }

        public static void ShouldBeDictionaryMember(
            this BodyLineItem source, string name, int level,
            Action<DictionaryDsl> dictionaryProperties,
            Action<MemberDsl> memberProperties = null)
        {
            var compare = CreateDictionary(name, level, dictionaryProperties);
            compare.IsMember = true;
            if (memberProperties != null) memberProperties(new MemberDsl(compare));
            source.ShouldMatchLineItem(compare);
        }

        public static void ShouldBeDictionaryDictionaryEntry(
            this BodyLineItem source, string name, string keyTypeName, int level,
            Action<DictionaryDsl> dictionaryProperties,
            Action<DictionaryKeyDsl> dictionaryKeyProperties = null)
        {
            var compare = CreateDictionary(name, level, dictionaryProperties);
            compare.IsDictionaryEntry = true;
            compare.DictionaryKey = new Key { TypeName = keyTypeName };
            if (dictionaryKeyProperties != null)
                dictionaryKeyProperties(new DictionaryKeyDsl(compare));
            source.ShouldMatchLineItem(compare);
        }

        public static void ShouldBeOpeningDictionaryDictionaryEntry(
            this BodyLineItem source, string name, string keyTypeName, int level,
            Action<DictionaryDsl> dictionaryProperties = null,
            Action<DictionaryKeyDsl> dictionaryKeyProperties = null)
        {
            var compare = CreateDictionary(name, level, dictionaryProperties);
            compare.IsOpening = true;
            compare.IsDictionaryEntry = true;
            compare.DictionaryKey = new Key { TypeName = keyTypeName };
            if (dictionaryKeyProperties != null)
                dictionaryKeyProperties(new DictionaryKeyDsl(compare));
            source.ShouldMatchLineItem(compare);
        }

        public static void ShouldBeClosingDictionaryDictionaryEntry(
            this BodyLineItem source, string name, int level,
            Action<DictionaryDsl> dictionaryKeyProperties = null)
        {
            var compare = CreateDictionary(name, level, dictionaryKeyProperties);
            compare.IsClosing = true;
            compare.IsDictionaryEntry = true;
            source.ShouldMatchLineItem(compare);
        }

        private static BodyLineItem CreateDictionary(
            string name, int level, Action<DictionaryDsl> properties)
        {
            var dictionaryType = new BodyLineItem
            {
                Name = name,
                IsDictionary = true,
                Whitespace = BodyDescriptionFactory.Whitespace.Repeat(level)
            };
            if (properties != null) properties(new DictionaryDsl(dictionaryType));
            return dictionaryType;
        }

        public class DictionaryDsl
        {
            private readonly BodyLineItem _body;
            public DictionaryDsl(BodyLineItem body) { _body = body; }
            public DictionaryDsl Comments(string comments) { _body.Comments = comments; return this; }
            public DictionaryDsl Opening() { _body.IsOpening = true; return this; }
            public DictionaryDsl Closing() { _body.IsClosing = true; return this; }
            public DictionaryDsl First() { _body.IsFirst = true; return this; }
            public DictionaryDsl Last() { _body.IsLast = true; return this; }

            public DictionaryDsl EmptyNamespace()
            {
                _body.LongNamespace = _body.ShortNamespace = new List<string>(); return this;
            }

            public DictionaryDsl LongNamespace(params string[] @namespace)
            {
                _body.LongNamespace = @namespace.ToList(); return this;
            }

            public DictionaryDsl ShortNamespace(params string[] @namespace)
            {
                _body.ShortNamespace = @namespace.ToList(); return this;
            }
        }

        // Complex type assertions

        public static void ShouldBeComplexType(this BodyLineItem source,
            string name, int level, Action<ComplexTypeDsl> properties)
        {
            source.ShouldMatchLineItem(CreateComplexType(name, level, properties));
        }

        public static void ShouldBeComplexTypeMember(
            this BodyLineItem source, string name, int level,
            Action<ComplexTypeDsl> complexTypeProperties = null,
            Action<MemberDsl> memberProperties = null)
        {
            var compare = CreateComplexType(name, level, complexTypeProperties);
            compare.IsMember = true;
            if (memberProperties != null) memberProperties(new MemberDsl(compare));
            source.ShouldMatchLineItem(compare);
        }

        public static void ShouldBeOpeningComplexTypeDictionaryEntry(
            this BodyLineItem source, string name, string keyTypeName, int level,
            Action<ComplexTypeDsl> complexTypeProperties = null,
            Action<DictionaryKeyDsl> dictionaryKeyProperties = null)
        {
            var compare = CreateComplexType(name, level, complexTypeProperties);
            compare.IsOpening = true;
            compare.IsDictionaryEntry = true;
            compare.DictionaryKey = new Key { TypeName = keyTypeName };
            if (dictionaryKeyProperties != null)
                dictionaryKeyProperties(new DictionaryKeyDsl(compare));
            source.ShouldMatchLineItem(compare);
        }

        public static void ShouldBeClosingComplexTypeDictionaryEntry(
            this BodyLineItem source, string name, int level,
            Action<ComplexTypeDsl> complexTypeProperties = null)
        {
            var compare = CreateComplexType(name, level, complexTypeProperties);
            compare.IsClosing = true;
            compare.IsDictionaryEntry = true;
            source.ShouldMatchLineItem(compare);
        }

        private static BodyLineItem CreateComplexType(
            string name, int level, Action<ComplexTypeDsl> properties = null)
        {
            var complexType = new BodyLineItem
            {
                Name = name,
                IsComplexType = true,
                Whitespace = BodyDescriptionFactory.Whitespace.Repeat(level)
            };
            if (properties != null) properties(new ComplexTypeDsl(complexType));
            return complexType;
        }

        public class ComplexTypeDsl
        {
            private readonly BodyLineItem _body;
            public ComplexTypeDsl(BodyLineItem body) { _body = body; }
            public ComplexTypeDsl Comments(string comments) { _body.Comments = comments; return this; }
            public ComplexTypeDsl Opening() { _body.IsOpening = true; return this; }
            public ComplexTypeDsl Closing() { _body.IsClosing = true; return this; }
            public ComplexTypeDsl First() { _body.IsFirst = true; return this; }
            public ComplexTypeDsl Last() { _body.IsLast = true; return this; }

            public ComplexTypeDsl EmptyNamespace()
            {
                _body.LongNamespace = _body.ShortNamespace = new List<string>(); return this;
            }

            public ComplexTypeDsl LongNamespace(params string[] @namespace)
            {
                _body.LongNamespace = @namespace.ToList(); return this;
            }

            public ComplexTypeDsl ShortNamespace(params string[] @namespace)
            {
                _body.ShortNamespace = @namespace.ToList(); return this;
            }
        }

        // Common assertion DSL's

        public class DictionaryKeyDsl
        {
            private readonly Key _key;
            public DictionaryKeyDsl(BodyLineItem body) { _key = body.DictionaryKey; }
            public DictionaryKeyDsl KeyComments(string comments) { _key.Comments = comments; return this; }

            public OptionDsl KeyOptions
            {
                get { return new OptionDsl(_key.Options = _key.Options ?? new List<Option>()); }
            }
        }

        public class MemberDsl
        {
            private readonly BodyLineItem _body;
            public MemberDsl(BodyLineItem body) { _body = body; }
            public MemberDsl Comments(string comments) { _body.Comments = comments; return this; }
            public MemberDsl Default(string value) { _body.DefaultValue = value; return this; }
            public MemberDsl Required() { _body.Required = true; return this; }
            public MemberDsl Optional() { _body.Optional = true; return this; }
            public MemberDsl IsLastMember() { _body.IsLastMember = true; return this; }

            public MemberDsl IsDeprecated(string message = null)
            {
                _body.IsDeprecated = true;
                _body.DeprecationMessage = message;
                return this;
            }
        }

        public class OptionDsl
        {
            private readonly List<Option> _options;
            public OptionDsl(List<Option> options) { _options = options; }

            public OptionDsl WithOption(string value)
            {
                return WithOption(new Option { Value = value });
            }

            public OptionDsl WithOption(string name, string value)
            {
                return WithOption(new Option { Name = name, Value = value });
            }

            public OptionDsl WithOptionAndComments(string value, string comments)
            {
                return WithOption(new Option { Value = value, Comments = comments });
            }

            public OptionDsl WithOptionAndComments(string name, string value, string comments)
            {
                return WithOption(new Option { Name = name, Value = value, Comments = comments });
            }

            private OptionDsl WithOption(Option option)
            {
                _options.Add(option); return this;
            }
        }

        // Common assertions

        public static List<BodyLineItem> ShouldBeIndexed(this List<BodyLineItem> source)
        {
            Enumerable.Range(0, source.Count).ForEach(x => source[x].Index.ShouldEqual(x + 1));
            return source;
        }

        private static void ShouldMatchLineItem(this BodyLineItem source, BodyLineItem compare)
        {
            source.Name.ShouldEqual(compare.Name);
            source.LongNamespace.ShouldEqual(compare.LongNamespace);
            source.ShortNamespace.ShouldEqual(compare.ShortNamespace);
            source.Comments.ShouldEqual(compare.Comments);
            source.IsFirst.ShouldEqual(compare.IsFirst);
            source.IsLast.ShouldEqual(compare.IsLast);
            source.TypeName.ShouldEqual(compare.TypeName);
            source.SampleValue.ShouldEqual(compare.SampleValue);
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
                source.DictionaryKey.TypeName.ShouldEqual(compare.DictionaryKey.TypeName);
                source.DictionaryKey.Comments.ShouldEqual(compare.DictionaryKey.Comments);
                source.DictionaryKey.Options.ShouldEqualOptions(compare.DictionaryKey.Options);
            }
        }

        private static void ShouldEqualOptions(this List<Option> source, List<Option> compare)
        {
            if (compare == null) source.ShouldBeNull();
            else
            {
                source.ShouldTotal(compare.Count);
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
