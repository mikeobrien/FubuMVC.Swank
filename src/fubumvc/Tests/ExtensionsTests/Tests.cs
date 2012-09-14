using System.Collections.Generic;
using System.Reflection;
using FubuMVC.Core.Registration.Routes;
using NSubstitute;
using NUnit.Framework;
using Should;
using Swank;

namespace Tests.ExtensionsTests
{
    [TestFixture]
    public class Tests
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
            Assembly.GetExecutingAssembly().FindTextResourceNamed("Tests.ExtensionsTests.EmbeddedResource").ShouldBeNull();
        }

        [Test]
        public void should_return_text_resource()
        {
            Assembly.GetExecutingAssembly().FindTextResourceNamed("Tests.ExtensionsTests.EmbeddedTextResource").ShouldEqual("Some text yo!");
        }

        [Test]
        public void should_return_html_resource()
        {
            Assembly.GetExecutingAssembly().FindTextResourceNamed("Tests.ExtensionsTests.EmbeddedHtmlResource").ShouldEqual("<b>Some html yo!</b>");
        }

        [Test]
        public void should_return_markdown_resource()
        {
            Assembly.GetExecutingAssembly().FindTextResourceNamed("Tests.ExtensionsTests.EmbeddedMarkdownResource").ShouldEqual("<p><strong>Some markdown yo!</strong></p>");
        }

        private class Widgets : List<string> { }
        private class Widgets<T> : List<string> { }

        [Test]
        public void should_determine_if_an_type_is_a_list_type()
        {
            typeof(List<string>).IsListType().ShouldBeTrue();
            typeof(IList<string>).IsListType().ShouldBeTrue();
            typeof(Widgets).IsListType().ShouldBeFalse();
            typeof(Widgets<string>).IsListType().ShouldBeFalse();
        }

        [Test]
        public void should_determine_if_an_type_implements_a_list_type()
        {
            typeof(List<string>).ImplementsListType().ShouldBeTrue();
            typeof(IList<string>).ImplementsListType().ShouldBeFalse();
            typeof(Widgets).ImplementsListType().ShouldBeTrue();
            typeof(Widgets<string>).ImplementsListType().ShouldBeTrue();
        }

        [Test]
        public void should_determine_if_an_type_inherits_from_a_list()
        {
            typeof(List<string>).InheritsFromListType().ShouldBeFalse();
            typeof(IList<string>).InheritsFromListType().ShouldBeFalse();
            typeof(Widgets).InheritsFromListType().ShouldBeTrue();
            typeof(Widgets<string>).InheritsFromListType().ShouldBeTrue();
        }

        [Test]
        public void should_determine_if_a_type_is_a_list()
        {
            typeof(List<string>).IsList().ShouldBeTrue();
            typeof(IList<string>).IsList().ShouldBeTrue();
            typeof(Widgets).IsList().ShouldBeTrue();
            typeof(Widgets<string>).IsList().ShouldBeTrue();
        }

        [Test]
        public void should_return_types_list_interface()
        {
            typeof(List<string>).GetListInterface().ShouldEqual(typeof(IList<string>));
            typeof(IList<string>).GetListInterface().ShouldEqual(typeof(IList<string>));
            typeof(Widgets).GetListInterface().ShouldEqual(typeof(IList<string>));
            typeof(Widgets<string>).GetListInterface().ShouldEqual(typeof(IList<string>));
        }

        [Test]
        public void should_return_list_element_type()
        {
            Extensions.GetListElementType(typeof(List<string>)).ShouldEqual(typeof(string));
            Extensions.GetListElementType(typeof(IList<string>)).ShouldEqual(typeof(string));
            Extensions.GetListElementType(typeof(Widgets)).ShouldEqual(typeof(string));
            Extensions.GetListElementType(typeof(Widgets<string>)).ShouldEqual(typeof(string));
            Extensions.GetListElementType(typeof(string[])).ShouldEqual(typeof(string));
            Extensions.GetListElementType(typeof(string)).ShouldBeNull();
        }
    }
}