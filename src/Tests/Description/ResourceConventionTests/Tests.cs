﻿using System.Reflection;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Swank;
using FubuMVC.Swank.Description;
using FubuMVC.Swank.Specification;
using NUnit.Framework;
using Should;

namespace Tests.Description.ResourceConventionTests
{
    [TestFixture]
    public class Tests
    {
        private IDescriptionConvention<BehaviorChain, ResourceDescription> _resourceConvention;
        private BehaviorGraph _graph;

        [SetUp]
        public void Setup()
        {
            _graph = Behavior.BuildGraph().AddActionsInThisNamespace();
            _resourceConvention = new ResourceConvention(
                new MarkerConvention<ResourceDescription>(),
                new BehaviorSource(_graph, Swank.CreateConfig(x => x.AppliesToThisAssembly()
                    .Where(y => y.FirstCall().HandlerType.InNamespace<Tests>()))));
        }

        [Test]
        public void should_use_attribute_description_over_embedded_description()
        {
            var action = _graph.GetAction<ResourceCommentsPriority.GetHandler>();
            var resource = _resourceConvention.GetDescription(action.ParentChain());

            resource.Name.ShouldEqual("Some Description");
            resource.Comments.ShouldEqual("Some comments.");

            Assembly.GetExecutingAssembly().FindTextResourceNamed<ResourceCommentsPriority.Resource>()
                .ShouldEqual("<p><strong>This is a resource</strong></p>");
        }

        [Test]
        public void should_not_find_resource_description_when_none_is_specified()
        {
            var action = _graph.GetAction<OrphanedAction.GetHandler>().ParentChain();
            _resourceConvention.GetDescription(action).ShouldBeNull();
        }

        [Test]
        public void should_set_default_description_when_no_marker_is_defined()
        {
            var resource = _resourceConvention.GetDescription(
                _graph.GetAction<ResourceDescriptions.NoDescription.GetHandler>().ParentChain());

            resource.Name.ShouldBeNull();
            resource.Comments.ShouldBeNull();
        }

        [Test]
        public void should_find_resource_marker_description()
        {
            var resource = _resourceConvention.GetDescription(
                _graph.GetAction<ResourceDescriptions.Description.GetHandler>().ParentChain());

            resource.Name.ShouldEqual("Some Resource");
            resource.Comments.ShouldEqual("Some comments.");
        }

        [Test]
        public void should_find_resource_marker_description_and_text_embedded_resource_comments()
        {
            var resource = _resourceConvention.GetDescription(
                _graph.GetAction<ResourceDescriptions.EmbeddedTextComments.GetHandler>().ParentChain());

            resource.Name.ShouldEqual("Some Text Resource");
            resource.Comments.ShouldEqual("<b>Some text comments</b>");
        }

        [Test]
        public void should_find_resource_marker_description_and_markdown_embedded_comments()
        {
            var resource = _resourceConvention.GetDescription(
                _graph.GetAction<ResourceDescriptions.EmbeddedMarkdownComments.GetHandler>().ParentChain());

            resource.Name.ShouldEqual("Some Markdown Resource");
            resource.Comments.ShouldEqual("<p><strong>Some markdown comments</strong></p>");
        }

        [Test]
        public void should_find_resource_attribute_description()
        {
            var resource = _resourceConvention.GetDescription(_graph.GetAction<AttributeResource.Controller>().ParentChain());
            resource.ShouldNotBeNull();
            resource.Name.ShouldEqual("Some Resource");
            resource.Comments.ShouldEqual("Some resource description");
        }

        [Test]
        public void should_find_resource_attribute_markdown_description()
        {
            var resource = _resourceConvention.GetDescription(_graph.GetAction<AttributeResource.EmbeddedMarkdownController>().ParentChain());
            resource.ShouldNotBeNull();
            resource.Name.ShouldEqual("Some Markdown Resource");
            resource.Comments.ShouldEqual("<p><strong>This is a resource</strong></p>");
        }

        [Test]
        public void should_find_resource_attribute_text_description()
        {
            var resource = _resourceConvention.GetDescription(_graph.GetAction<AttributeResource.EmbeddedTextController>().ParentChain());
            resource.ShouldNotBeNull();
            resource.Name.ShouldEqual("Some Text Resource");
            resource.Comments.ShouldEqual("<b>This is a resource<b/>");
        }

        [Test]
        public void should_find_resource_description_when_an_applies_to_type_is_specified()
        {
            var resource = _resourceConvention.GetDescription(_graph.GetAction<AppliedToResource.WidgetGetHandler>().ParentChain());
            resource.ShouldNotBeNull();
            resource.Name.ShouldEqual("Another Resource");
            resource.Comments.ShouldBeNull();
        }

        [Test]
        public void should_find_resource_description_when_an_applies_to_type_is_specified_for_another_type_in_the_group()
        {
            var resource = _resourceConvention.GetDescription(_graph.GetAction<AppliedToResource.WidgetPutHandler>().ParentChain());
            resource.ShouldNotBeNull();
            resource.Name.ShouldEqual("Another Resource");
            resource.Comments.ShouldBeNull();
        }

        [Test]
        public void should_find_parent_resource_description()
        {
            var resource = _resourceConvention.GetDescription(_graph.GetAction<ChildResources.Widget.GetHandler>().ParentChain());
            resource.ShouldNotBeNull();
            resource.Name.ShouldEqual("Some Resource");
            resource.Comments.ShouldBeNull();
        }

        [Test]
        public void should_find_closest_parent_resource_description()
        {
            var resource = _resourceConvention.GetDescription(_graph.GetAction<NestedResources.Widget.GetHandler>().ParentChain());
            resource.ShouldNotBeNull();
            resource.Name.ShouldEqual("Another Resource");
            resource.Comments.ShouldBeNull();
        }
    }
}