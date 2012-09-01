using FubuMVC.Core.Registration.Routes;
using NSubstitute;
using NUnit.Framework;
using Should;
using Swank;

namespace Tests
{
    [TestFixture]
    public class ExtensionsTests
    {
        [Test]
        public void should_get_resource_from_pattern()
        {
            var route = Substitute.For<IRouteDefinition>();
            route.Pattern.Returns("{Yada}/books/{Id}/categories/{CategoryId}/classification/{ClassId}");
            route.GetRouteResource().ShouldEqual("books/categories/classification");
        }
    }
}