using System.Collections.Generic;
using System.Reflection;
using FubuCore.Reflection;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using NUnit.Framework;
using Should;
using Swank;
using Swank.Description;
using Tests.Administration.Users;
using Tests.Batches.Cells;
using Tests.Batches.Schedules;
using Tests.Templates;

namespace Tests
{
    [TestFixture]
    public class SpecificationBuilderResourceTests
    {
        private static readonly BatchCellResource BatchCellResource = new BatchCellResource();
        private static readonly AdminAccountResource AdminAccountResource = new AdminAccountResource();
        private static readonly AdminAddressResource AdminAddressResource = new AdminAddressResource();
        private static readonly AdminUserResource AdminUserResource = new AdminUserResource();

        private BehaviorGraph _graph;
        private IDescriptionSource<ActionCall, ModuleDescription> _moduleSource;
        private IDescriptionSource<ActionCall, ResourceDescription> _resourceSource;
        private IDescriptionSource<ActionCall, EndpointDescription> _endpointSource;
        private IDescriptionSource<PropertyInfo, ParameterDescription> _parameterSource;
        private IDescriptionSource<FieldInfo, OptionDescription> _optionSource;
        private IDescriptionSource<ActionCall, List<ErrorDescription>> _errors;

        [SetUp]
        public void Setup()
        {
            _graph = TestBehaviorGraph.Build();
            _moduleSource = new ModuleSource(new MarkerSource<ModuleDescription>());
            _resourceSource = new ResourceSource(
                new MarkerSource<ResourceDescription>(),
                new Swank.ActionSource(_graph, ConfigurationDsl.CreateConfig(x => x.AppliesToThisAssembly())), new ResourceSourceConfig());
            _endpointSource = new EndpointSource();
            _parameterSource = new ParameterSource();
            _optionSource = new OptionSource();
            _errors = new ErrorSource();
        }

        [Test]
        public void should_automatically_add_orphaned_actions_to_default_resource()
        {
            var configuration = ConfigurationDsl.CreateConfig(x => x
                .AppliesToThisAssembly()
                .OnOrphanedModuleAction(OrphanedActions.UseDefault)
                .OnOrphanedResourceAction(OrphanedActions.UseDefault));
            var specBuilder = new SpecificationBuilder(configuration, new Swank.ActionSource(_graph, configuration), new TypeDescriptorCache(),
                _moduleSource, _resourceSource, _endpointSource, _parameterSource, _optionSource, _errors);

            var spec = specBuilder.Build();

            var resources = spec.modules[0].resources;
            resources.Count.ShouldEqual(2);
            var resource = resources[0];
            resource.name.ShouldEqual("templates");
            resource.comments.ShouldBeNull();
            resource = resources[1];
            resource.name.ShouldEqual("templates/files");
            resource.comments.ShouldBeNull();

            resources = spec.modules[1].resources;
            resources.Count.ShouldEqual(3);
            resource = resources[0];
            resource.name.ShouldEqual(AdminAccountResource.Name);
            resource.comments.ShouldEqual(AdminAccountResource.Comments);
            resource = resources[1];
            resource.name.ShouldEqual(AdminAddressResource.Name);
            resource.comments.ShouldEqual(AdminAddressResource.Comments);
            resource = resources[2];
            resource.name.ShouldEqual(AdminUserResource.Name);
            resource.comments.ShouldEqual(AdminUserResource.Comments);

            resources = spec.modules[2].resources;
            resources.Count.ShouldEqual(1);
            resource = resources[0];
            resource.name.ShouldEqual(BatchCellResource.Name);
            resource.comments.ShouldEqual(BatchCellResource.Comments);

            resources = spec.modules[3].resources;
            resources.Count.ShouldEqual(1);
            resource = resources[0];
            resource.name.ShouldEqual("batches/schedules");
            resource.comments.ShouldBeNull();
        }

        [Test]
        public void should_automatically_add_orphaned_actions_to_specified_default_resource()
        {
            var configuration = ConfigurationDsl.CreateConfig(x => x
                .AppliesToThisAssembly()
                .OnOrphanedModuleAction(OrphanedActions.UseDefault)
                .OnOrphanedResourceAction(OrphanedActions.UseDefault)
                .WithDefaultResource(y => new ResourceDescription { Name = y.ParentChain().Route.FirstPatternSegment() }));
            var specBuilder = new SpecificationBuilder(configuration, new Swank.ActionSource(_graph, configuration), new TypeDescriptorCache(),
                _moduleSource, _resourceSource, _endpointSource, _parameterSource, _optionSource, _errors);

            var spec = specBuilder.Build();

            var resources = spec.modules[0].resources;
            resources.Count.ShouldEqual(1);
            var resource = resources[0];
            resource.name.ShouldEqual("templates");
            resource.comments.ShouldBeNull();

            resources = spec.modules[1].resources;
            resources.Count.ShouldEqual(3);
            resource = resources[0];
            resource.name.ShouldEqual(AdminAccountResource.Name);
            resource.comments.ShouldEqual(AdminAccountResource.Comments);
            resource = resources[1];
            resource.name.ShouldEqual(AdminAddressResource.Name);
            resource.comments.ShouldEqual(AdminAddressResource.Comments);
            resource = resources[2];
            resource.name.ShouldEqual(AdminUserResource.Name);
            resource.comments.ShouldEqual(AdminUserResource.Comments);

            resources = spec.modules[2].resources;
            resources.Count.ShouldEqual(1);
            resource = resources[0];
            resource.name.ShouldEqual(BatchCellResource.Name);
            resource.comments.ShouldEqual(BatchCellResource.Comments);

            resources = spec.modules[3].resources;
            resources.Count.ShouldEqual(1);
            resource = resources[0];
            resource.name.ShouldEqual("batches");
            resource.comments.ShouldBeNull();
        }

        [Test]
        public void should_ignore_orphaned_actions()
        {
            var configuration = ConfigurationDsl.CreateConfig(x => x
                .AppliesToThisAssembly()
                .OnOrphanedModuleAction(OrphanedActions.UseDefault)
                .OnOrphanedResourceAction(OrphanedActions.Exclude)
                .WithDefaultResource(y => new ResourceDescription { Name = y.ParentChain().Route.FirstPatternSegment() }));
            var specBuilder = new SpecificationBuilder(configuration, new Swank.ActionSource(_graph, configuration), new TypeDescriptorCache(),
                _moduleSource, _resourceSource, _endpointSource, _parameterSource, _optionSource, _errors);

            var spec = specBuilder.Build();

            spec.modules[0].resources.Count.ShouldEqual(0);

            var resources = spec.modules[1].resources;
            resources.Count.ShouldEqual(3);
            var resource = resources[0];
            resource.name.ShouldEqual(AdminAccountResource.Name);
            resource.comments.ShouldEqual(AdminAccountResource.Comments);
            resource = resources[1];
            resource.name.ShouldEqual(AdminAddressResource.Name);
            resource.comments.ShouldEqual(AdminAddressResource.Comments);
            resource = resources[2];
            resource.name.ShouldEqual(AdminUserResource.Name);
            resource.comments.ShouldEqual(AdminUserResource.Comments);

            resources = spec.modules[2].resources;
            resources.Count.ShouldEqual(1);
            resource = resources[0];
            resource.name.ShouldEqual(BatchCellResource.Name);
            resource.comments.ShouldEqual(BatchCellResource.Comments);

            spec.modules[3].resources.Count.ShouldEqual(0);
        }

        [Test]
        public void should_throw_an_exception_for_orphaned_actions()
        {
            var configuration = ConfigurationDsl.CreateConfig(x => x
                .AppliesToThisAssembly()
                .OnOrphanedModuleAction(OrphanedActions.UseDefault)
                .OnOrphanedResourceAction(OrphanedActions.Fail));
            var specBuilder = new SpecificationBuilder(configuration, new Swank.ActionSource(_graph, configuration), new TypeDescriptorCache(),
                _moduleSource, _resourceSource, _endpointSource, _parameterSource, _optionSource, _errors);

            Assert.Throws<OrphanedResourceActionException>(() => specBuilder.Build());
        }

        [Test]
        public void should_not_throw_an_exception_when_there_are_no_orphaned_actions()
        {
            var configuration = ConfigurationDsl.CreateConfig(x => x
                .AppliesToThisAssembly()
                .OnOrphanedModuleAction(OrphanedActions.UseDefault)
                .OnOrphanedResourceAction(OrphanedActions.Fail)
                .Where(y => y.HandlerType.Namespace != typeof(TemplateRequest).Namespace &&
                            y.HandlerType.Namespace != typeof(BatchScheduleRequest).Namespace));
            var specBuilder = new SpecificationBuilder(configuration, new Swank.ActionSource(_graph, configuration), new TypeDescriptorCache(),
                _moduleSource, _resourceSource, _endpointSource, _parameterSource, _optionSource, _errors);

            Assert.DoesNotThrow(() => specBuilder.Build());
        }
    }
}