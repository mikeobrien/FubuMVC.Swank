using System;
using FubuCore.Reflection;
using FubuMVC.Swank;
using FubuMVC.Swank.Description;
using FubuMVC.Swank.Extensions;
using FubuMVC.Swank.Specification;
using NUnit.Framework;
using Should;

namespace Tests.Specification.SpecificationServiceResourceTests
{
    [TestFixture]
    public class Tests
    {
        private FubuMVC.Swank.Specification.Specification BuildSpec<TNamespace>(Action<Swank> configure = null, System.Type rootType = null)
        {
            var graph = rootType == null ? Behavior.BuildGraph().AddActionsInThisNamespace() : Behavior.BuildGraph().AddActionsInNamespace(rootType);
            var moduleSource = new ModuleSource(new MarkerSource<ModuleDescription>());
            var resourceSource = new ResourceSource(
                new MarkerSource<ResourceDescription>(),
                new ActionSource(graph,
                    Swank.CreateConfig(x => x.AppliesToThisAssembly()
                        .Where(y => y.HandlerType.InNamespace<Tests>()))));
            var configuration = Swank.CreateConfig(x => 
            { if (configure != null) configure(x); x.AppliesToThisAssembly().Where(y => y.HandlerType.InNamespace<TNamespace>()); });
            return new SpecificationService(configuration, new ActionSource(graph, configuration), new TypeDescriptorCache(),
                moduleSource, resourceSource, new EndpointSource(), new MemberSource(), new OptionSource(), new ErrorSource(), new TypeSource()).Generate();
        }

        [Test]
        public void should_set_default_description_when_no_marker_is_defined()
        {
            var spec = BuildSpec<ResourceDescriptions.NoDescription.GetHandler>();

            var resource = spec.Resources[0];

            resource.Name.ShouldBeNull();
            resource.Comments.ShouldBeNull();
        }

        [Test]
        public void should_set_description_when_marker_is_defined()
        {
            var spec = BuildSpec<ResourceDescriptions.Description.GetHandler>();

            var resource = spec.Resources[0];

            resource.Name.ShouldEqual("Some Resource");
            resource.Comments.ShouldEqual("Some comments.");
        }

        [Test]
        public void should_set_description_and_text_embedded_resource_comments_when_marker_is_defined()
        {
            var spec = BuildSpec<ResourceDescriptions.EmbeddedTextComments.GetHandler>();

            var resource = spec.Resources[0];

            resource.Name.ShouldEqual("Some Text Resource");
            resource.Comments.ShouldEqual("<b>Some text comments</b>");
        }

        [Test]
        public void should_set_description_and_markdown_embedded_resource_comments_when_marker_is_defined()
        {
            var spec = BuildSpec<ResourceDescriptions.EmbeddedMarkdownComments.GetHandler>();

            var resource = spec.Resources[0];

            resource.Name.ShouldEqual("Some Markdown Resource");
            resource.Comments.ShouldEqual("<p><strong>Some markdown comments</strong></p>");
        }

        [Test]
        public void should_set_description_when_attribute_is_applied()
        {
            var spec = BuildSpec<AttributeResource.Attribute.Controller>();

            var resource = spec.Resources[0];

            resource.Name.ShouldEqual("Some Resource");
            resource.Comments.ShouldEqual("Some resource description");
        }

        [Test]
        public void should_set_description_and_text_embedded_resource_comments_when_attribute_is_applied()
        {
            var spec = BuildSpec<AttributeResource.EmbeddedTextComments.Controller>();

            var resource = spec.Resources[0];

            resource.Name.ShouldEqual("Some Text Resource");
            resource.Comments.ShouldEqual("<b>This is a resource</b>");
        }

        [Test]
        public void should_set_description_and_markdown_embedded_resource_comments_when_attribute_is_applied()
        {
            var spec = BuildSpec<AttributeResource.EmbeddedMarkdownComments.Controller>();

            var resource = spec.Resources[0];

            resource.Name.ShouldEqual("Some Markdown Resource");
            resource.Comments.ShouldEqual("<p><strong>This is a resource</strong></p>");
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

            spec.Resources.Count.ShouldEqual(1);

            var resource = spec.Resources[0];
            resource.Endpoints.Count.ShouldEqual(4);
            resource.Name.ShouldEqual("Some Resource");
            resource.Endpoints[0].Url.ShouldEqual("/samenamespace");
            resource.Endpoints[1].Url.ShouldEqual("/samenamespace/{Id}");
            resource.Endpoints[2].Url.ShouldEqual("/samenamespace/widget");
            resource.Endpoints[3].Url.ShouldEqual("/samenamespace/widget/{Id}");
        }

        [Test]
        public void should_group_all_actions_in_child_namespaces_into_the_same_resource()
        {
            var spec = BuildSpec<ChildResources.GetHandler>();

            spec.Resources.Count.ShouldEqual(1);

            var resource = spec.Resources[0];
            resource.Endpoints.Count.ShouldEqual(4);
            resource.Name.ShouldEqual("Some Resource");
            resource.Endpoints[0].Url.ShouldEqual("/childresources");
            resource.Endpoints[1].Url.ShouldEqual("/childresources/{Id}");
            resource.Endpoints[2].Url.ShouldEqual("/childresources/widget");
            resource.Endpoints[3].Url.ShouldEqual("/childresources/widget/{Id}");
        }

        [Test]
        public void should_group_actions_into_the_closest_parent_resources()
        {
            var spec = BuildSpec<NestedResources.GetHandler>();

            spec.Resources.Count.ShouldEqual(2);

            var resource = spec.Resources[0];
            resource.Endpoints.Count.ShouldEqual(2);
            resource.Name.ShouldEqual("Another Resource");
            resource.Endpoints[0].Url.ShouldEqual("/nestedresources/widget");
            resource.Endpoints[1].Url.ShouldEqual("/nestedresources/widget/{Id}");

            resource = spec.Resources[1];
            resource.Endpoints.Count.ShouldEqual(2);
            resource.Name.ShouldEqual("Some Resource");
            resource.Endpoints[0].Url.ShouldEqual("/nestedresources");
            resource.Endpoints[1].Url.ShouldEqual("/nestedresources/{Id}");
        }

        [Test]
        public void should_group_orphaned_actions_into_default_resources()
        {
            var spec = BuildSpec<OrphanedResources.GetHandler>(rootType: typeof(OrphanedResources.GetHandler));

            spec.Resources.Count.ShouldEqual(2);

            var resource = spec.Resources[0];
            resource.Endpoints.Count.ShouldEqual(2);
            resource.Name.ShouldEqual("/");
            resource.Endpoints[0].Url.ShouldEqual("/");
            resource.Endpoints[1].Url.ShouldEqual("/{Id}");

            resource = spec.Resources[1];
            resource.Endpoints.Count.ShouldEqual(2);
            resource.Name.ShouldEqual("/widget");
            resource.Endpoints[0].Url.ShouldEqual("/widget");
            resource.Endpoints[1].Url.ShouldEqual("/widget/{Id}");
        }

        [Test]
        public void should_group_orphaned_actions_into_the_specified_default_resource()
        {
            var spec = BuildSpec<OrphanedResources.GetHandler>(
                x => x.WithDefaultResource(y => new ResourceDescription{ Name = y.ParentChain().Route.FirstPatternSegment()}));

            spec.Resources.Count.ShouldEqual(1);

            var resource = spec.Resources[0];
            resource.Endpoints.Count.ShouldEqual(4);
            resource.Name.ShouldEqual("orphanedresources");
            resource.Endpoints[0].Url.ShouldEqual("/orphanedresources");
            resource.Endpoints[1].Url.ShouldEqual("/orphanedresources/{Id}");
            resource.Endpoints[2].Url.ShouldEqual("/orphanedresources/widget");
            resource.Endpoints[3].Url.ShouldEqual("/orphanedresources/widget/{Id}");
        }

        [Test]
        public void should_apply_resource_to_handler()
        {
            var spec = BuildSpec<AppliedToResource.GetHandler>();

            spec.Resources.Count.ShouldEqual(2);

            var resource = spec.Resources[0];
            resource.Endpoints.Count.ShouldEqual(2);
            resource.Name.ShouldEqual("Another Resource");
            resource.Endpoints[0].Url.ShouldEqual("/appliedtoresource/widget");
            resource.Endpoints[1].Url.ShouldEqual("/appliedtoresource/widget/{Id}");

            resource = spec.Resources[1];
            resource.Endpoints.Count.ShouldEqual(2);
            resource.Name.ShouldEqual("Some Resource");
            resource.Endpoints[0].Url.ShouldEqual("/appliedtoresource");
            resource.Endpoints[1].Url.ShouldEqual("/appliedtoresource/{Id}");
        }

        [Test]
        public void should_ignore_orphaned_actions()
        {
            var spec = BuildSpec<OrphanedNestedResources.GetHandler>(x => x
                .OnOrphanedResourceAction(OrphanedActions.Exclude));

            spec.Resources.Count.ShouldEqual(1);

            var resource = spec.Resources[0];
            resource.Endpoints.Count.ShouldEqual(2);
            resource.Name.ShouldEqual("Another Resource");
            resource.Endpoints[0].Url.ShouldEqual("/orphanednestedresources/widget");
            resource.Endpoints[1].Url.ShouldEqual("/orphanednestedresources/widget/{Id}");
        }
    }
}