using System.Collections.Generic;
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

        private class Widgets : List<string> {}

        [Test]
        public void should_determine_if_an_object_is_a_list()
        {
            typeof(List<string>).IsList().ShouldBeTrue();
            typeof(IList<string>).IsList().ShouldBeTrue();
            typeof(Widgets).IsList().ShouldBeTrue();
        }

        [Test]
        public void should_return_list_element_type()
        {
            typeof(List<string>).GetElementTypeOrDefault().ShouldEqual(typeof(string));
            typeof(IList<string>).GetElementTypeOrDefault().ShouldEqual(typeof(string));
            typeof(Widgets).GetElementTypeOrDefault().ShouldEqual(typeof(string));
            typeof(string[]).GetElementTypeOrDefault().ShouldEqual(typeof(string));
            typeof(string).GetElementTypeOrDefault().ShouldEqual(typeof(string));
        }
    }
}