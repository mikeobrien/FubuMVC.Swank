using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using FubuCore.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Media.Projections;
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
        private readonly DataDescriptionFactory _factory = 
            new DataDescriptionFactory();

        [Test]
        public void should_create_complex_type()
        {
            var description = _factory.Create(
                CreateCompleType(
                "ComplexType", 
                "Complex type comments"));

            should_be_complex_type(description[0],
                "ComplexType", "Complex type comments", true, 1);

            should_be_complex_type(description[0],
                "ComplexType", null, false, 1);
        }

        private void should_be_complex_type(
            DataDescription data,
            string name,
            string comments,
            bool opening,
            int level)
        {
            data.Name.ShouldEqual(name);
            data.Comments.ShouldEqual(comments);
            data.TypeName.ShouldBeNull();
            data.DefaultValue.ShouldBeNull();
            data.Required.ShouldBeNull();
            data.Optional.ShouldBeNull();
            data.Whitespace.ShouldEqual(DataDescriptionFactory.Whitespace.Repeat(level));
            data.IsDeprecated.ShouldBeNull();
            data.DeprecationMessage.ShouldBeNull();

            if (opening)
            {
                data.IsOpening.Value.ShouldBeTrue();
                data.IsClosing.ShouldBeNull();              
            }
            else
            {
                data.IsOpening.ShouldBeNull();  
                data.IsClosing.Value.ShouldBeTrue();
            }

            data.IsMember.ShouldBeNull(); 
            data.IsLastMember.ShouldBeNull();

            should_not_be_a_simple_type(data); 

            data.IsComplexType.Value.ShouldBeTrue();

            data.IsArray.ShouldBeNull();

            should_not_be_a_dictionary_type(data); 
        }

        private void should_not_be_a_simple_type(DataDescription data)
        {
            data.IsSimpleType.ShouldBeNull();
            data.IsString.ShouldBeNull();
            data.IsBoolean.ShouldBeNull();
            data.IsNumeric.ShouldBeNull();
            data.IsDateTime.ShouldBeNull();
            data.IsDuration.ShouldBeNull();
            data.IsGuid.ShouldBeNull();
            data.Options.ShouldBeNull();
        }

        private void should_not_be_a_dictionary_type(DataDescription data)
        {
            data.IsDictionary.ShouldBeNull();
            data.IsDictionaryEntry.ShouldBeNull();
            data.DictionaryKey.ShouldBeNull();
        }

        private DataType CreateCompleType(
            string name, 
            string comments = null, 
            params Func<Member>[] members)
        {
            return new DataType
            {
                Name = name,
                Comments = comments,
                IsComplex = true,
                Members = members.Select(x => x()).ToList()
            };
        }

        private Member CreateMember(
            string name, 
            DataType type,
            string comments = null, 
            string defaultValue = null,
            bool optional = false,
            bool required = false)
        {
            return new Member
            {
                Name = name,
                Comments = comments,
                DefaultValue = defaultValue,
                Optional = optional,
                Required = required,
                Type = type
            };
        }
    }
}
