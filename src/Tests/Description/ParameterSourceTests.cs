using System;
using FubuMVC.Swank.Description;
using NUnit.Framework;
using Should;

namespace Tests.Description
{
    [TestFixture]
    public class ParameterSourceTests
    {
        private ParameterSource _parameterSource;

        [SetUp]
        public void Setup()
        {
            _parameterSource = new ParameterSource();
        }

        public class Request
        {
            [Comments("This is the id.")]
            public Guid Id { get; set; }
            public string Sort { get; set; }
        }

        [Test]
        public void should_return_default_description_of_parameter()
        {
            var description = _parameterSource.GetDescription(typeof(Request).GetProperty("Sort"));
            description.Name.ShouldEqual("Sort");
            description.Comments.ShouldBeNull();
        }

        [Test]
        public void should_return_attribute_description_of_parameter()
        {
            var description = _parameterSource.GetDescription(typeof(Request).GetProperty("Id"));
            description.Name.ShouldEqual("Id");
            description.Comments.ShouldEqual("This is the id.");
        }
    }
}