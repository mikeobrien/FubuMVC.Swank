using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuMVC.Swank.Description;
using NUnit.Framework;
using Should;

namespace Tests.Description.MarkerConventionTests
{
    [TestFixture]
    public class Tests
    {
        private IList<FubuMVC.Swank.Description.Description> _descriptions;

        [SetUp]
        public void Setup()
        {
            _descriptions = new MarkerConvention<FubuMVC.Swank.Description.Description>().GetDescriptions(Assembly.GetExecutingAssembly())
                .Where(x => x.GetType().InNamespace<Tests>()).ToList();
        }

        [Test]
        public void should_use_marker_description_over_embedded_description()
        {
            var marker = _descriptions.First(x => x.GetType() == typeof(MarkerCommentsPriority.Description));

            marker.Name.ShouldEqual("Some Description");
            marker.Comments.ShouldEqual("Some comments.");

            Assembly.GetExecutingAssembly().FindTextResourceNamed<MarkerCommentsPriority.Description>()
                .ShouldEqual("<p><strong>This is a marker</strong></p>");
        }

        [Test]
        public void should_set_description_to_default_when_none_is_specified()
        {
            var marker = _descriptions.First(x => x.GetType() == typeof(MarkerDescriptions.NoDescription.Description));

            marker.Name.ShouldBeNull();
            marker.Comments.ShouldBeNull();
        }

        [Test]
        public void should_set_description_when_one_is_specified()
        {
            var marker = _descriptions.First(x => x.GetType() == typeof(MarkerDescriptions.Description.Description));

            marker.Name.ShouldEqual("Some Description");
            marker.Comments.ShouldEqual("Some comments.");
        }

        [Test]
        public void should_set_description_and_text_embedded_resource_comments_when_specified()
        {
            var marker = _descriptions.First(x => x.GetType() == typeof(MarkerDescriptions.EmbeddedTextComments.Description));

            marker.Name.ShouldEqual("Some Text Description");
            marker.Comments.ShouldEqual("<b>Some text comments</b>");
        }

        [Test]
        public void should_set_description_and_markdown_embedded_resource_comments_when_specified()
        {
            var marker = _descriptions.First(x => x.GetType() == typeof(MarkerDescriptions.EmbeddedMarkdownComments.Description));

            marker.Name.ShouldEqual("Some Markdown Description");
            marker.Comments.ShouldEqual("<p><strong>Some markdown comments</strong></p>");
        }

        [Test]
        public void should_be_ordered_by_descending_namespace_and_ascending_name()
        {
            _descriptions[0].ShouldBeType<MarkerOrder.ZeeLastMarker.FirstDescription>();
            _descriptions[1].ShouldBeType<MarkerOrder.ZeeLastMarker.LastDescription>();
            _descriptions[2].ShouldBeType<MarkerOrder.AFirstMarker.FirstDescription>();
            _descriptions[3].ShouldBeType<MarkerOrder.AFirstMarker.LastDescription>();
        }
    }
}