using System;
using FubuMVC.Media.Projections;
using FubuMVC.Swank.Extensions;
using NUnit.Framework;
using Should;
using System.Linq;

namespace Tests.ExtensionTests
{
    [TestFixture]
    public class FubuTests
    {
        public class Model
        {
            public Guid Id { get; set; }
            public int Sort { get; set; }
            public string UserAgent { get; set; }
            public string Name { get; set; }
        }

        public class Projection : Projection<Model>
        {
            public Projection()
            {
                Value(x => x.Id);
                Value(x => x.UserAgent);
            }
        }

        [Test]
        public void should__determine_if_a_type_is_a_projection()
        {
            typeof(Projection).IsProjection().ShouldBeTrue();
            typeof(Model).IsProjection().ShouldBeFalse();
        }

        [Test]
        public void should_enumerate_projection_properties()
        {
            var properties = typeof(Projection).GetProjectionProperties();
            properties.Count.ShouldEqual(2);
            properties.Any(x => x.Name == "Id").ShouldBeTrue();
            properties.Any(x => x.Name == "UserAgent").ShouldBeTrue();
        }
    }
}