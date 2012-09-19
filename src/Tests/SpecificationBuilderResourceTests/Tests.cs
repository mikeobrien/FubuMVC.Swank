using System;
using System.Collections.Generic;
using System.Reflection;
using FubuCore.Reflection;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Swank;
using FubuMVC.Swank.Description;
using NUnit.Framework;
using Should;
using ActionSource = FubuMVC.Swank.ActionSource;
using Type = System.Type;

namespace Tests.SpecificationBuilderResourceTests
{
    [TestFixture]
    public class Tests
    {
        private BehaviorGraph _graph;
        private IDescriptionSource<ActionCall, ModuleDescription> _moduleSource;
        private IDescriptionSource<ActionCall, EndpointDescription> _endpointSource;
        private IDescriptionSource<PropertyInfo, ParameterDescription> _parameterSource;
        private IDescriptionSource<FieldInfo, OptionDescription> _optionSource;
        private IDescriptionSource<ActionCall, List<ErrorDescription>> _errors;
        private IDescriptionSource<Type, DataTypeDescription> _dataTypes;

        [SetUp]
        public void Setup()
        {
            _graph = Behaviors.BuildGraph().AddActionsInThisNamespace();
            _moduleSource = new ModuleSource(new MarkerSource<ModuleDescription>());
            _endpointSource = new EndpointSource();
            _parameterSource = new ParameterSource();
            _optionSource = new OptionSource();
            _errors = new ErrorSource();
            _dataTypes = new TypeSource();
        }

        private Specification BuildSpec<T>(Action<ConfigurationDsl> configure = null)
        {
            var resourceSource = new ResourceSource(
                new MarkerSource<ResourceDescription>(),
                new ActionSource(_graph,
                    ConfigurationDsl.CreateConfig(x => x.AppliesToThisAssembly().Where(y => y.HandlerType.InNamespace<Tests>()))));
            var configuration = ConfigurationDsl.CreateConfig(x => { if (configure != null) configure(x); x.AppliesToThisAssembly().Where(y => y.HandlerType.InNamespace<T>()); });
            return new SpecificationBuilder(configuration, new ActionSource(_graph, configuration), new TypeDescriptorCache(),
                _moduleSource, resourceSource, _endpointSource, _parameterSource, _optionSource, _errors, _dataTypes).Build();
        }

        [Test]
        public void should_set_default_description_when_no_marker_is_defined()
        {
            var spec = BuildSpec<ResourceDescriptions.NoDescription.GetHandler>();

            var resource = spec.resources[0];

            resource.name.ShouldBeNull();
            resource.comments.ShouldBeNull();
        }

        [Test]
        public void should_set_description_when_marker_is_defined()
        {
            var spec = BuildSpec<ResourceDescriptions.Description.GetHandler>();

            var resource = spec.resources[0];

            resource.name.ShouldEqual("Some Resource");
            resource.comments.ShouldEqual("Some comments.");
        }

        [Test]
        public void should_set_description_and_text_embedded_resource_comments_when_marker_is_defined()
        {
            var spec = BuildSpec<ResourceDescriptions.EmbeddedTextComments.GetHandler>();

            var resource = spec.resources[0];

            resource.name.ShouldEqual("Some Text Resource");
            resource.comments.ShouldEqual("<b>Some text comments</b>");
        }

        [Test]
        public void should_set_description_and_markdown_embedded_resource_comments_when_marker_is_defined()
        {
            var spec = BuildSpec<ResourceDescriptions.EmbeddedMarkdownComments.GetHandler>();

            var resource = spec.resources[0];

            resource.name.ShouldEqual("Some Markdown Resource");
            resource.comments.ShouldEqual("<p><strong>Some markdown comments</strong></p>");
        }

        [Test]
        public void should_set_description_when_attribute_is_applied()
        {
            var spec = BuildSpec<AttributeResource.Attribute.Controller>();

            var resource = spec.resources[0];

            resource.name.ShouldEqual("Some Resource");
            resource.comments.ShouldEqual("Some resource description");
        }

        [Test]
        public void should_set_description_and_text_embedded_resource_comments_when_attribute_is_applied()
        {
            var spec = BuildSpec<AttributeResource.EmbeddedTextComments.Controller>();

            var resource = spec.resources[0];

            resource.name.ShouldEqual("Some Text Resource");
            resource.comments.ShouldEqual("<b>This is a resource</b>");
        }

        [Test]
        public void should_set_description_and_markdown_embedded_resource_comments_when_attribute_is_applied()
        {
            var spec = BuildSpec<AttributeResource.EmbeddedMarkdownComments.Controller>();

            var resource = spec.resources[0];

            resource.name.ShouldEqual("Some Markdown Resource");
            resource.comments.ShouldEqual("<p><strong>This is a resource</strong></p>");
        }

        [Test]
        public void should_throw_an_exception_for_orphaned_actions()
        {
            Assert.Throws<OrphanedResourceActionException>(() => BuildSpec<OrphanedAction.GetHandler>(x => x
                    .OnOrphanedResourceAction(OrphanedActions.Fail)));
        }

        [Test]
        public void should_not_throw_an_exception_when_there_are_no_orphaned_actions()
        {
            Assert.DoesNotThrow(() => BuildSpec<NotOrphanedAction.GetHandler>(x => x
                    .OnOrphanedResourceAction(OrphanedActions.Fail)));
        }

        [Test]
        public void should_group_all_actions_in_the_same_namespace_into_the_same_resource()
        {
            var spec = BuildSpec<SameNamespace.GetHandler>();

            spec.resources.Count.ShouldEqual(1);

            var resource = spec.resources[0];
            resource.endpoints.Count.ShouldEqual(4);
            resource.name.ShouldEqual("Some Resource");
            resource.endpoints[0].url.ShouldEqual("/samenamespace");
            resource.endpoints[1].url.ShouldEqual("/samenamespace/{Id}");
            resource.endpoints[2].url.ShouldEqual("/samenamespace/widget");
            resource.endpoints[3].url.ShouldEqual("/samenamespace/widget/{Id}");
        }

        [Test]
        public void should_group_all_actions_in_child_namespaces_into_the_same_resource()
        {
            var spec = BuildSpec<ChildResources.GetHandler>();

            spec.resources.Count.ShouldEqual(1);

            var resource = spec.resources[0];
            resource.endpoints.Count.ShouldEqual(4);
            resource.name.ShouldEqual("Some Resource");
            resource.endpoints[0].url.ShouldEqual("/childresources");
            resource.endpoints[1].url.ShouldEqual("/childresources/{Id}");
            resource.endpoints[2].url.ShouldEqual("/childresources/widget");
            resource.endpoints[3].url.ShouldEqual("/childresources/widget/{Id}");
        }

        [Test]
        public void should_group_actions_into_the_closest_parent_resources()
        {
            var spec = BuildSpec<NestedResources.GetHandler>();

            spec.resources.Count.ShouldEqual(2);

            var resource = spec.resources[0];
            resource.endpoints.Count.ShouldEqual(2);
            resource.name.ShouldEqual("Another Resource");
            resource.endpoints[0].url.ShouldEqual("/nestedresources/widget");
            resource.endpoints[1].url.ShouldEqual("/nestedresources/widget/{Id}");

            resource = spec.resources[1];
            resource.endpoints.Count.ShouldEqual(2);
            resource.name.ShouldEqual("Some Resource");
            resource.endpoints[0].url.ShouldEqual("/nestedresources");
            resource.endpoints[1].url.ShouldEqual("/nestedresources/{Id}");
        }

        [Test]
        public void should_group_orphaned_actions_into_default_resources()
        {
            var spec = BuildSpec<OrphanedResources.GetHandler>();

            spec.resources.Count.ShouldEqual(2);

            var resource = spec.resources[0];
            resource.endpoints.Count.ShouldEqual(2);
            resource.name.ShouldEqual("orphanedresources");
            resource.endpoints[0].url.ShouldEqual("/orphanedresources");
            resource.endpoints[1].url.ShouldEqual("/orphanedresources/{Id}");

            resource = spec.resources[1];
            resource.endpoints.Count.ShouldEqual(2);
            resource.name.ShouldEqual("orphanedresources/widget");
            resource.endpoints[0].url.ShouldEqual("/orphanedresources/widget");
            resource.endpoints[1].url.ShouldEqual("/orphanedresources/widget/{Id}");
        }

        [Test]
        public void should_group_orphaned_actions_into_the_specified_default_resource()
        {
            var spec = BuildSpec<OrphanedResources.GetHandler>(
                x => x.WithDefaultResource(y => new ResourceDescription{ Name = y.ParentChain().Route.FirstPatternSegment()}));

            spec.resources.Count.ShouldEqual(1);

            var resource = spec.resources[0];
            resource.endpoints.Count.ShouldEqual(4);
            resource.name.ShouldEqual("orphanedresources");
            resource.endpoints[0].url.ShouldEqual("/orphanedresources");
            resource.endpoints[1].url.ShouldEqual("/orphanedresources/{Id}");
            resource.endpoints[2].url.ShouldEqual("/orphanedresources/widget");
            resource.endpoints[3].url.ShouldEqual("/orphanedresources/widget/{Id}");
        }

        [Test]
        public void should_apply_resource_to_handler()
        {
            var spec = BuildSpec<AppliedToResource.GetHandler>();

            spec.resources.Count.ShouldEqual(2);

            var resource = spec.resources[0];
            resource.endpoints.Count.ShouldEqual(2);
            resource.name.ShouldEqual("Another Resource");
            resource.endpoints[0].url.ShouldEqual("/appliedtoresource/widget");
            resource.endpoints[1].url.ShouldEqual("/appliedtoresource/widget/{Id}");

            resource = spec.resources[1];
            resource.endpoints.Count.ShouldEqual(2);
            resource.name.ShouldEqual("Some Resource");
            resource.endpoints[0].url.ShouldEqual("/appliedtoresource");
            resource.endpoints[1].url.ShouldEqual("/appliedtoresource/{Id}");
        }

        [Test]
        public void should_ignore_orphaned_actions()
        {
            var spec = BuildSpec<OrphanedNestedResources.GetHandler>(x => x
                .OnOrphanedResourceAction(OrphanedActions.Exclude));

            spec.resources.Count.ShouldEqual(1);

            var resource = spec.resources[0];
            resource.endpoints.Count.ShouldEqual(2);
            resource.name.ShouldEqual("Another Resource");
            resource.endpoints[0].url.ShouldEqual("/orphanednestedresources/widget");
            resource.endpoints[1].url.ShouldEqual("/orphanednestedresources/widget/{Id}");
        }
    }
}