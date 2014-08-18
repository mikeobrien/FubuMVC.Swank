using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FubuCore.Reflection;
using FubuMVC.Swank;
using FubuMVC.Swank.Description;
using FubuMVC.Swank.Specification;
using NUnit.Framework;
using Should;

namespace Tests.Specification.TypeGraphFactoryTests
{
    [TestFixture]
    public class Tests
    {
        public TypeGraphFactory CreateFactory(Action<Configuration> configure = null)
        {
            var configuration = new Configuration();
            if (configure != null) configure(configuration);
            return new TypeGraphFactory(
                configuration, 
                new TypeDescriptorCache(), 
                new TypeConvention(), 
                new MemberConvention(), 
                new OptionConvention());
        }

        public class ComplexType
        {
            
        }

        [Test]
        public void should_get_complex_type_name()
        {
            CreateFactory().BuildGraph(typeof(ComplexType))
                .Name.ShouldEqual("ComplexType");
        }

        [Test]
        public void should_get_complex_type_comments()
        {
            CreateFactory().BuildGraph(typeof(ComplexType))
                .Comments.ShouldEqual("");
        }
    }
}
