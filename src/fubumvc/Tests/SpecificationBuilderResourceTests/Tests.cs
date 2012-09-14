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
using Tests.SpecificationBuilderResourceTests.Administration;
using Tests.SpecificationBuilderResourceTests.Administration.Users;
using Tests.SpecificationBuilderResourceTests.Batches.Cells;
using Tests.SpecificationBuilderResourceTests.Batches.Schedules;
using Tests.SpecificationBuilderResourceTests.Templates;

namespace Tests.SpecificationBuilderResourceTests
{
    [TestFixture]
    public class Tests
    {
        public const string AccountResourceComments = "<p><strong>These are accounts yo!</strong></p>";
        public const string BatchCellsResourceComments = "<b>These are batch cells yo!</b>";

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
        private IDescriptionSource<Type, DataTypeDescription> _dataTypes;

        private static readonly Func<ActionCall, bool> ActionFilter =
            x => x.HandlerType.Namespace.StartsWith(typeof(Tests).Namespace);

        [SetUp]
        public void Setup()
        {
            _graph = Behaviors.BuildGraph()
                .AddAction<TemplateGetAllHandler>("/templates", HttpVerbs.Get)
                .AddAction<TemplateFileGetAllHandler>("/templates/files", HttpVerbs.Get)
                .AddAction<AdminAccountGetAllHandler>("/admin", HttpVerbs.Get)
                .AddAction<AdminUserGetAllHandler>("/admin/users", HttpVerbs.Get)
                .AddAction<AdminAddressGetAllHandler>("/admin/users/addresses", HttpVerbs.Get)
                .AddAction<BatchCellGetAllHandler>("/batches/cells", HttpVerbs.Get)
                .AddAction<BatchScheduleGetAllHandler>("/batches/schedules", HttpVerbs.Get);
            _moduleSource = new ModuleSource(new MarkerSource<ModuleDescription>());
            _resourceSource = new ResourceSource(
                new MarkerSource<ResourceDescription>(),
                new Swank.ActionSource(_graph, ConfigurationDsl.CreateConfig(x => x.AppliesToThisAssembly().Where(ActionFilter))), new ResourceSourceConfig());
            _endpointSource = new EndpointSource();
            _parameterSource = new ParameterSource();
            _optionSource = new OptionSource();
            _errors = new ErrorSource();
            _dataTypes = new DataTypeSource();
        }

        [Test]
        public void should_automatically_add_orphaned_actions_to_default_resource()
        {
            var configuration = ConfigurationDsl.CreateConfig(x => x
                .AppliesToThisAssembly()
                .Where(ActionFilter)
                .OnOrphanedModuleAction(OrphanedActions.UseDefault)
                .OnOrphanedResourceAction(OrphanedActions.UseDefault));
            var specBuilder = new SpecificationBuilder(configuration, new Swank.ActionSource(_graph, configuration), new TypeDescriptorCache(),
                _moduleSource, _resourceSource, _endpointSource, _parameterSource, _optionSource, _errors, _dataTypes);

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
            resource.comments.ShouldEqual(AccountResourceComments);
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
            resource.comments.ShouldEqual(BatchCellsResourceComments);

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
                .Where(ActionFilter)
                .OnOrphanedModuleAction(OrphanedActions.UseDefault)
                .OnOrphanedResourceAction(OrphanedActions.UseDefault)
                .WithDefaultResource(y => new ResourceDescription { Name = y.ParentChain().Route.FirstPatternSegment() }));
            var specBuilder = new SpecificationBuilder(configuration, new Swank.ActionSource(_graph, configuration), new TypeDescriptorCache(),
                _moduleSource, _resourceSource, _endpointSource, _parameterSource, _optionSource, _errors, _dataTypes);

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
            resource.comments.ShouldEqual(AccountResourceComments);
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
            resource.comments.ShouldEqual(BatchCellsResourceComments);

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
                .Where(ActionFilter)
                .OnOrphanedModuleAction(OrphanedActions.UseDefault)
                .OnOrphanedResourceAction(OrphanedActions.Exclude)
                .WithDefaultResource(y => new ResourceDescription { Name = y.ParentChain().Route.FirstPatternSegment() }));
            var specBuilder = new SpecificationBuilder(configuration, new Swank.ActionSource(_graph, configuration), new TypeDescriptorCache(),
                _moduleSource, _resourceSource, _endpointSource, _parameterSource, _optionSource, _errors, _dataTypes);

            var spec = specBuilder.Build();

            spec.modules[0].resources.Count.ShouldEqual(0);

            var resources = spec.modules[1].resources;
            resources.Count.ShouldEqual(3);
            var resource = resources[0];
            resource.name.ShouldEqual(AdminAccountResource.Name);
            resource.comments.ShouldEqual(AccountResourceComments);
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
            resource.comments.ShouldEqual(BatchCellsResourceComments);

            spec.modules[3].resources.Count.ShouldEqual(0);
        }

        [Test]
        public void should_throw_an_exception_for_orphaned_actions()
        {
            var configuration = ConfigurationDsl.CreateConfig(x => x
                .AppliesToThisAssembly()
                .Where(ActionFilter)
                .OnOrphanedModuleAction(OrphanedActions.UseDefault)
                .OnOrphanedResourceAction(OrphanedActions.Fail));
            var specBuilder = new SpecificationBuilder(configuration, new Swank.ActionSource(_graph, configuration), new TypeDescriptorCache(),
                _moduleSource, _resourceSource, _endpointSource, _parameterSource, _optionSource, _errors, _dataTypes);

            Assert.Throws<OrphanedResourceActionException>(() => specBuilder.Build());
        }

        [Test]
        public void should_not_throw_an_exception_when_there_are_no_orphaned_actions()
        {
            var configuration = ConfigurationDsl.CreateConfig(x => x
                .AppliesToThisAssembly()
                .Where(ActionFilter)
                .OnOrphanedModuleAction(OrphanedActions.UseDefault)
                .OnOrphanedResourceAction(OrphanedActions.Fail)
                .Where(y => y.HandlerType.Namespace != typeof(TemplateGetAllHandler).Namespace &&
                            y.HandlerType.Namespace != typeof(BatchScheduleGetAllHandler).Namespace));
            var specBuilder = new SpecificationBuilder(configuration, new Swank.ActionSource(_graph, configuration), new TypeDescriptorCache(),
                _moduleSource, _resourceSource, _endpointSource, _parameterSource, _optionSource, _errors, _dataTypes);

            Assert.DoesNotThrow(() => specBuilder.Build());
        }
    }
}