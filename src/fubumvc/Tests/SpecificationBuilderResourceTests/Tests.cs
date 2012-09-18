using System;
using System.Collections.Generic;
using System.Reflection;
using FubuCore.Reflection;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using NUnit.Framework;
using Should;
using Swank;
using Swank.Description;
using Swank.Models;
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

        private Specification BuildSpec<T>(Action<ConfigurationDsl> configure = null, ResourceSourceConfig resourceSourceConfig = null)
        {
            var resourceSource = new ResourceSource(
                new MarkerSource<ResourceDescription>(),
                new Swank.ActionSource(_graph,
                    ConfigurationDsl.CreateConfig(x => x.AppliesToThisAssembly().Where(y => y.HandlerType.InNamespace<Tests>()))),
                    resourceSourceConfig ?? new ResourceSourceConfig());
            var configuration = ConfigurationDsl.CreateConfig(x => { if (configure != null) configure(x); x.AppliesToThisAssembly().Where(y => y.HandlerType.InNamespace<T>()); });
            return new SpecificationBuilder(configuration, new Swank.ActionSource(_graph, configuration), new TypeDescriptorCache(),
                _moduleSource, resourceSource, _endpointSource, _parameterSource, _optionSource, _errors, _dataTypes).Build();
        }

        [Test]
        public void should_set_description_when_one_is_specified()
        {
            var spec = BuildSpec<ResourceDescriptions.Description.GetHandler>();

            var resource = spec.resources[0];

            resource.name.ShouldEqual("Some Resource");
            resource.comments.ShouldEqual("Some comments.");
        }

        [Test]
        public void should_set_description_and_text_embedded_resource_comments_when_specified()
        {
            var spec = BuildSpec<ResourceDescriptions.EmbeddedTextComments.GetHandler>();

            var resource = spec.resources[0];

            resource.name.ShouldEqual("Some Text Resource");
            resource.comments.ShouldEqual("<b>Some text comments</b>");
        }

        [Test]
        public void should_set_description_and_markdown_embedded_resource_comments_when_specified()
        {
            var spec = BuildSpec<ResourceDescriptions.EmbeddedMarkdownComments.GetHandler>();

            var resource = spec.resources[0];

            resource.name.ShouldEqual("Some Markdown Resource");
            resource.comments.ShouldEqual("<p><strong>Some markdown comments</strong></p>");
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
        public void should_group_actions_in_a_child_namespace_into_the_resource()
        {
            var spec = BuildSpec<NestedResources.GetHandler>();

            spec.resources.Count.ShouldEqual(1);

            var resource = spec.resources[0];
            resource.endpoints.Count.ShouldEqual(4);
            resource.name.ShouldEqual("Some Resource");
            resource.endpoints[0].url.ShouldEqual("/nestedresources");
            resource.endpoints[1].url.ShouldEqual("/nestedresources/{Id}");
            resource.endpoints[2].url.ShouldEqual("/nestedresources/widget");
            resource.endpoints[3].url.ShouldEqual("/nestedresources/widget/{Id}");
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
        public void should_override_resource_grouping()
        {
            var spec = BuildSpec<OrphanedResources.GetHandler>(
                resourceSourceConfig: new ResourceSourceConfig().GroupBy(z => z.ParentChain().Route.FirstPatternSegment()));

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
            // this is with the generic param where two resources are in the same namespace
            var spec = BuildSpec<ResourceDescriptions.EmbeddedMarkdownComments.GetHandler>();

            var resource = spec.resources[0];
        }

        [Test]
        public void should_automatically_add_orphaned_actions_to_default_resource()
        {
            
        }

        [Test]
        public void should_automatically_add_orphaned_actions_to_specified_default_resource()
        {
            
        }

        [Test]
        public void should_ignore_orphaned_actions()
        {
            
        }

        //[Test]
        //public void should_automatically_add_orphaned_actions_to_default_resource()
        //{
        //    var configuration = ConfigurationDsl.CreateConfig(x => x
        //        .AppliesToThisAssembly()
        //        .Where(ActionFilter)
        //        .OnOrphanedModuleAction(OrphanedActions.UseDefault)
        //        .OnOrphanedResourceAction(OrphanedActions.UseDefault));
        //    var specBuilder = new SpecificationBuilder(configuration, new Swank.ActionSource(_graph, configuration), new TypeDescriptorCache(),
        //        _moduleSource, _resourceSource, _endpointSource, _parameterSource, _optionSource, _errors, _dataTypes);

        //    var spec = specBuilder.Build();

        //    spec.modules.Count.ShouldEqual(3);
        //    spec.resources.Count.ShouldEqual(2);

        //    var resources = spec.modules[0].resources;
        //    resources.Count.ShouldEqual(3);
        //    var resource = resources[0];
        //    resource.name.ShouldEqual(AdminAccountResource.Name);
        //    resource.comments.ShouldEqual(AccountResourceComments);
        //    resource = resources[1];
        //    resource.name.ShouldEqual(AdminAddressResource.Name);
        //    resource.comments.ShouldEqual(AdminAddressResource.Comments);
        //    resource = resources[2];
        //    resource.name.ShouldEqual(AdminUserResource.Name);
        //    resource.comments.ShouldEqual(AdminUserResource.Comments);

        //    resources = spec.modules[1].resources;
        //    resources.Count.ShouldEqual(1);
        //    resource = resources[0];
        //    resource.name.ShouldEqual(BatchCellResource.Name);
        //    resource.comments.ShouldEqual(BatchCellsResourceComments);

        //    resources = spec.modules[2].resources;
        //    resources.Count.ShouldEqual(1);
        //    resource = resources[0];
        //    resource.name.ShouldEqual("batches/schedules");
        //    resource.comments.ShouldBeNull();

        //    resources = spec.resources;
        //    resources.Count.ShouldEqual(2);
        //    resource = resources[0];
        //    resource.name.ShouldEqual("templates");
        //    resource.comments.ShouldBeNull();
        //    resource = resources[1];
        //    resource.name.ShouldEqual("templates/file");
        //    resource.comments.ShouldBeNull();
        //}

        //[Test]
        //public void should_automatically_add_orphaned_actions_to_specified_default_resource()
        //{
        //    var configuration = ConfigurationDsl.CreateConfig(x => x
        //        .AppliesToThisAssembly()
        //        .Where(ActionFilter)
        //        .OnOrphanedModuleAction(OrphanedActions.UseDefault)
        //        .OnOrphanedResourceAction(OrphanedActions.UseDefault)
        //        .WithDefaultResource(y => new ResourceDescription { Name = y.ParentChain().Route.FirstPatternSegment() }));
        //    var specBuilder = new SpecificationBuilder(configuration, new Swank.ActionSource(_graph, configuration), new TypeDescriptorCache(),
        //        _moduleSource, _resourceSource, _endpointSource, _parameterSource, _optionSource, _errors, _dataTypes);

        //    var spec = specBuilder.Build();

        //    spec.modules.Count.ShouldEqual(3);
        //    spec.resources.Count.ShouldEqual(1);

        //    var resources = spec.modules[0].resources;
        //    resources.Count.ShouldEqual(3);
        //    var resource = resources[0];
        //    resource.name.ShouldEqual(AdminAccountResource.Name);
        //    resource.comments.ShouldEqual(AccountResourceComments);
        //    resource = resources[1];
        //    resource.name.ShouldEqual(AdminAddressResource.Name);
        //    resource.comments.ShouldEqual(AdminAddressResource.Comments);
        //    resource = resources[2];
        //    resource.name.ShouldEqual(AdminUserResource.Name);
        //    resource.comments.ShouldEqual(AdminUserResource.Comments);

        //    resources = spec.modules[1].resources;
        //    resources.Count.ShouldEqual(1);
        //    resource = resources[0];
        //    resource.name.ShouldEqual(BatchCellResource.Name);
        //    resource.comments.ShouldEqual(BatchCellsResourceComments);

        //    resources = spec.modules[2].resources;
        //    resources.Count.ShouldEqual(1);
        //    resource = resources[0];
        //    resource.name.ShouldEqual("batches");
        //    resource.comments.ShouldBeNull();

        //    resources = spec.resources;
        //    resource = resources[0];
        //    resource.name.ShouldEqual("templates");
        //    resource.comments.ShouldBeNull();
        //}

        //[Test]
        //public void should_ignore_orphaned_actions()
        //{
        //    var configuration = ConfigurationDsl.CreateConfig(x => x
        //        .AppliesToThisAssembly()
        //        .Where(ActionFilter)
        //        .OnOrphanedModuleAction(OrphanedActions.UseDefault)
        //        .OnOrphanedResourceAction(OrphanedActions.Exclude)
        //        .WithDefaultResource(y => new ResourceDescription { Name = y.ParentChain().Route.FirstPatternSegment() }));
        //    var specBuilder = new SpecificationBuilder(configuration, new Swank.ActionSource(_graph, configuration), new TypeDescriptorCache(),
        //        _moduleSource, _resourceSource, _endpointSource, _parameterSource, _optionSource, _errors, _dataTypes);

        //    var spec = specBuilder.Build();

        //    spec.modules.Count.ShouldEqual(2);
        //    spec.resources.Count.ShouldEqual(0);

        //    var resources = spec.modules[0].resources;
        //    resources.Count.ShouldEqual(3);
        //    var resource = resources[0];
        //    resource.name.ShouldEqual(AdminAccountResource.Name);
        //    resource.comments.ShouldEqual(AccountResourceComments);
        //    resource = resources[1];
        //    resource.name.ShouldEqual(AdminAddressResource.Name);
        //    resource.comments.ShouldEqual(AdminAddressResource.Comments);
        //    resource = resources[2];
        //    resource.name.ShouldEqual(AdminUserResource.Name);
        //    resource.comments.ShouldEqual(AdminUserResource.Comments);

        //    resources = spec.modules[1].resources;
        //    resources.Count.ShouldEqual(1);
        //    resource = resources[0];
        //    resource.name.ShouldEqual(BatchCellResource.Name);
        //    resource.comments.ShouldEqual(BatchCellsResourceComments);
        //}
    }
}