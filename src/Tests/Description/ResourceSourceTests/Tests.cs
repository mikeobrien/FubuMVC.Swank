using System;
using System.Reflection;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Swank;
using FubuMVC.Swank.Description;
using NUnit.Framework;
using Should;
using ActionSource = FubuMVC.Swank.ActionSource;

namespace Tests.Description.ResourceSourceTests
{
    [TestFixture]
    public class Tests
    {
        private IDescriptionSource<ActionCall, ResourceDescription> _resourceSource;
        private BehaviorGraph _graph;

        [SetUp]
        public void Setup()
        {
            _graph = Behaviors.BuildGraph().AddActionsInThisNamespace();
            _resourceSource = new ResourceSource(
                new MarkerSource<ResourceDescription>(),
                new ActionSource(_graph, ConfigurationDsl.CreateConfig(x => x.AppliesToThisAssembly()
                    .Where(y => y.HandlerType.InNamespace<Tests>()))));
        }

        [Test]
        public void should_use_attribute_description_over_embedded_description()
        {
            var action = _graph.GetAction<ResourceCommentsPriority.GetHandler>();
            var resource = _resourceSource.GetDescription(action);

            resource.Name.ShouldEqual("Some Description");
            resource.Comments.ShouldEqual("Some comments.");

            Assembly.GetExecutingAssembly().FindTextResourceNamed<ResourceCommentsPriority.Resource>()
                .ShouldEqual("<p><strong>This is a resource</strong></p>");
        }

        [Test]
        public void should_not_find_resource_description_when_none_is_specified()
        {
            var action = _graph.GetAction<OrphanedAction.GetHandler>();
            _resourceSource.GetDescription(action).ShouldBeNull();
        }

        [Test]
        public void should_set_default_description_when_no_marker_is_defined()
        {
            var resource = _resourceSource.GetDescription(
                _graph.GetAction<ResourceDescriptions.NoDescription.GetHandler>());

            resource.Name.ShouldBeNull();
            resource.Comments.ShouldBeNull();
        }

        [Test]
        public void should_find_resource_marker_description()
        {
            var resource = _resourceSource.GetDescription(
                _graph.GetAction<ResourceDescriptions.Description.GetHandler>());

            resource.Name.ShouldEqual("Some Resource");
            resource.Comments.ShouldEqual("Some comments.");
        }

        [Test]
        public void should_find_resource_marker_description_and_text_embedded_resource_comments()
        {
            var resource = _resourceSource.GetDescription(
                _graph.GetAction<ResourceDescriptions.EmbeddedTextComments.GetHandler>());

            resource.Name.ShouldEqual("Some Text Resource");
            resource.Comments.ShouldEqual("<b>Some text comments</b>");
        }

        [Test]
        public void should_find_resource_marker_description_and_markdown_embedded_comments()
        {
            var resource = _resourceSource.GetDescription(
                _graph.GetAction<ResourceDescriptions.EmbeddedMarkdownComments.GetHandler>());

            resource.Name.ShouldEqual("Some Markdown Resource");
            resource.Comments.ShouldEqual("<p><strong>Some markdown comments</strong></p>");
        }

        [Test]
        public void should_find_resource_attribute_description()
        {
            var resource = _resourceSource.GetDescription(_graph.GetAction<AttributeResource.Controller>());
            resource.ShouldNotBeNull();
            resource.Name.ShouldEqual("Some Resource");
            resource.Comments.ShouldEqual("Some resource description");
        }

        [Test]
        public void should_find_resource_attribute_markdown_description()
        {
            var resource = _resourceSource.GetDescription(_graph.GetAction<AttributeResource.EmbeddedMarkdownController>());
            resource.ShouldNotBeNull();
            resource.Name.ShouldEqual("Some Markdown Resource");
            resource.Comments.ShouldEqual("<p><strong>This is a resource</strong></p>");
        }

        [Test]
        public void should_find_resource_attribute_text_description()
        {
            var resource = _resourceSource.GetDescription(_graph.GetAction<AttributeResource.EmbeddedTextController>());
            resource.ShouldNotBeNull();
            resource.Name.ShouldEqual("Some Text Resource");
            resource.Comments.ShouldEqual("<b>This is a resource<b/>");
        }

        [Test]
        public void should_find_resource_description_when_an_applies_to_type_is_specified()
        {
            var resource = _resourceSource.GetDescription(_graph.GetAction<AppliedToResource.WidgetGetHandler>());
            resource.ShouldNotBeNull();
            resource.Name.ShouldEqual("Another Resource");
            resource.Comments.ShouldBeNull();
        }

        [Test]
        public void should_find_resource_description_when_an_applies_to_type_is_specified_for_another_type_in_the_group()
        {
            var resource = _resourceSource.GetDescription(_graph.GetAction<AppliedToResource.WidgetPutHandler>());
            resource.ShouldNotBeNull();
            resource.Name.ShouldEqual("Another Resource");
            resource.Comments.ShouldBeNull();
        }

        [Test]
        public void should_find_parent_resource_description()
        {
            var resource = _resourceSource.GetDescription(_graph.GetAction<ChildResources.Widget.GetHandler>());
            resource.ShouldNotBeNull();
            resource.Name.ShouldEqual("Some Resource");
            resource.Comments.ShouldBeNull();
        }

        [Test]
        public void should_find_closest_parent_resource_description()
        {
            var resource = _resourceSource.GetDescription(_graph.GetAction<NestedResources.Widget.GetHandler>());
            resource.ShouldNotBeNull();
            resource.Name.ShouldEqual("Another Resource");
            resource.Comments.ShouldBeNull();
        }
    }
}