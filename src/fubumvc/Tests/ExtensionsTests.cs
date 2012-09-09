using System.Reflection;
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

        [Test]
        public void should_return_null_when_no_resource_found()
        {
            Assembly.GetExecutingAssembly().FindTextResourceNamed("Tests.EmbeddedResource").ShouldBeNull();
        }

        [Test]
        public void should_return_text_resource()
        {
            Assembly.GetExecutingAssembly().FindTextResourceNamed("Tests.EmbeddedTextResource").ShouldEqual("Some text yo!");
        }

        [Test]
        public void should_return_html_resource()
        {
            Assembly.GetExecutingAssembly().FindTextResourceNamed("Tests.EmbeddedHtmlResource").ShouldEqual("<b>Some html yo!</b>");
        }

        [Test]
        public void should_return_markdown_resource()
        {
            Assembly.GetExecutingAssembly().FindTextResourceNamed("Tests.EmbeddedMarkdownResource").ShouldEqual("<p><strong>Some markdown yo!</strong></p>");
        }
    }
}