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
using Tests.SpecificationBuilderModuleTests.Administration;
using Tests.SpecificationBuilderModuleTests.Batches;
using Tests.SpecificationBuilderModuleTests.Batches.Schedules;
using Tests.SpecificationBuilderModuleTests.Templates;

namespace Tests.SpecificationBuilderModuleTests
{
    [TestFixture]
    public class Tests
    {
        public const string SchedulesModuleComments = "<p><strong>These are schedules yo!</strong></p>";
        public const string BatchesModuleComments = "<b>These are batches yo!</b>";

        private static readonly AdministrationModule AdministrationModule = new AdministrationModule();
        private static readonly BatchesModule BatchesModule = new BatchesModule();
        private static readonly SchedulesModule SchedulesModule = new SchedulesModule();

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
            _graph = Behaviors.BuildGraph().AddActionsInThisNamespace();
            _moduleSource = new ModuleSource(new MarkerSource<ModuleDescription>());
            _resourceSource = new ResourceSource(
                new MarkerSource<ResourceDescription>(),
                new Swank.ActionSource(_graph, ConfigurationDsl.CreateConfig(x => x.AppliesToThisAssembly())), new ResourceSourceConfig());
            _endpointSource = new EndpointSource();
            _parameterSource = new ParameterSource();
            _optionSource = new OptionSource();
            _errors = new ErrorSource();
            _dataTypes = new DataTypeSource();
        }

        [Test]
        public void should_return_actions_at_root_when_only_one_module_with_no_name()
        {
            var configuration = ConfigurationDsl.CreateConfig(x => x
                .AppliesToThisAssembly()
                .OnOrphanedModuleAction(OrphanedActions.UseDefault)
                .Where(y => y.HandlerType.Namespace == typeof(TemplatePutHandler).Namespace));
            var specBuilder = new SpecificationBuilder(configuration, new Swank.ActionSource(_graph, configuration), new TypeDescriptorCache(),
                _moduleSource, _resourceSource, _endpointSource, _parameterSource, _optionSource, _errors, _dataTypes);

            var spec = specBuilder.Build();

            spec.modules.Count.ShouldEqual(0);
            spec.resources.Count.ShouldEqual(1);
            spec.resources[0].endpoints.Count.ShouldEqual(1);
        }

        [Test]
        public void should_automatically_add_orphaned_actions_to_empty_default_module()
        {
            var configuration = ConfigurationDsl.CreateConfig(x => x
                .AppliesToThisAssembly()
                .Where(ActionFilter)
                .OnOrphanedModuleAction(OrphanedActions.UseDefault));
            var specBuilder = new SpecificationBuilder(configuration, new Swank.ActionSource(_graph, configuration), new TypeDescriptorCache(),
                _moduleSource, _resourceSource, _endpointSource, _parameterSource, _optionSource, _errors, _dataTypes);

            var spec = specBuilder.Build();

            spec.modules.Count.ShouldEqual(4);
            spec.resources.Count.ShouldEqual(0);

            var module = spec.modules[0];
            module.name.ShouldBeNull();
            module.comments.ShouldBeNull();
            module.resources.Count.ShouldEqual(1);

            module = spec.modules[1];
            module.name.ShouldEqual(AdministrationModule.Name);
            module.comments.ShouldEqual(AdministrationModule.Comments);
            module.resources.Count.ShouldEqual(3);

            module = spec.modules[2];
            module.name.ShouldEqual(BatchesModule.Name);
            module.comments.ShouldEqual(BatchesModuleComments);
            module.resources.Count.ShouldEqual(1);

            module = spec.modules[3];
            module.name.ShouldEqual(SchedulesModule.Name);
            module.comments.ShouldEqual(SchedulesModuleComments);
            module.resources.Count.ShouldEqual(1);
        }

        [Test]
        public void should_automatically_add_orphaned_actions_to_specified_default_module()
        {
            var configuration = ConfigurationDsl.CreateConfig(x => x
                .AppliesToThisAssembly()
                .Where(ActionFilter)
                .OnOrphanedModuleAction(OrphanedActions.UseDefault)
                .WithDefaultModule(y => new ModuleDescription { Name = "API", Comments = "This is the API yo!" }));
            var specBuilder = new SpecificationBuilder(configuration, new Swank.ActionSource(_graph, configuration), new TypeDescriptorCache(),
                _moduleSource, _resourceSource, _endpointSource, _parameterSource, _optionSource, _errors, _dataTypes);

            var spec = specBuilder.Build();

            spec.modules.Count.ShouldEqual(4);
            spec.resources.Count.ShouldEqual(0);

            var module = spec.modules[0];
            module.name.ShouldEqual(AdministrationModule.Name);
            module.comments.ShouldEqual(AdministrationModule.Comments);
            module.resources.Count.ShouldEqual(3);

            module = spec.modules[1];
            module.name.ShouldEqual("API");
            module.comments.ShouldEqual("This is the API yo!");
            module.resources.Count.ShouldEqual(1);

            module = spec.modules[2];
            module.name.ShouldEqual(BatchesModule.Name);
            module.comments.ShouldEqual(BatchesModuleComments);
            module.resources.Count.ShouldEqual(1);

            module = spec.modules[3];
            module.name.ShouldEqual(SchedulesModule.Name);
            module.comments.ShouldEqual(SchedulesModuleComments);
            module.resources.Count.ShouldEqual(1);
        }

        [Test]
        public void should_ignore_orphaned_actions()
        {
            var configuration = ConfigurationDsl.CreateConfig(x => x
                .AppliesToThisAssembly()
                .Where(ActionFilter)
                .OnOrphanedModuleAction(OrphanedActions.Exclude));
            var specBuilder = new SpecificationBuilder(configuration, new Swank.ActionSource(_graph, configuration), new TypeDescriptorCache(),
                _moduleSource, _resourceSource, _endpointSource, _parameterSource, _optionSource, _errors, _dataTypes);

            var spec = specBuilder.Build();

            spec.modules.Count.ShouldEqual(3);
            spec.resources.Count.ShouldEqual(0);

            var module = spec.modules[0];
            module.name.ShouldEqual(AdministrationModule.Name);
            module.comments.ShouldEqual(AdministrationModule.Comments);
            module.resources.Count.ShouldEqual(3);

            module = spec.modules[1];
            module.name.ShouldEqual(BatchesModule.Name);
            module.comments.ShouldEqual(BatchesModuleComments);
            module.resources.Count.ShouldEqual(1);

            module = spec.modules[2];
            module.name.ShouldEqual(SchedulesModule.Name);
            module.comments.ShouldEqual(SchedulesModuleComments);
            module.resources.Count.ShouldEqual(1);
        }

        [Test]
        public void should_throw_an_exception_for_orphaned_actions()
        {
            var configuration = ConfigurationDsl.CreateConfig(x => x
                .AppliesToThisAssembly()
                .Where(ActionFilter)
                .OnOrphanedModuleAction(OrphanedActions.Fail));
            var specBuilder = new SpecificationBuilder(configuration, new Swank.ActionSource(_graph, configuration), new TypeDescriptorCache(),
                _moduleSource, _resourceSource, _endpointSource, _parameterSource, _optionSource, _errors, _dataTypes);

            Assert.Throws<OrphanedModuleActionException>(() => specBuilder.Build());
        }

        [Test]
        public void should_not_throw_an_exception_when_there_are_no_orphaned_actions()
        {
            var configuration = ConfigurationDsl.CreateConfig(x => x
                .AppliesToThisAssembly()
                .Where(ActionFilter)
                .OnOrphanedModuleAction(OrphanedActions.Fail)
                .Where(y => y.HandlerType.Namespace != typeof(TemplatePutHandler).Namespace));
            var specBuilder = new SpecificationBuilder(configuration, new Swank.ActionSource(_graph, configuration), new TypeDescriptorCache(),
                _moduleSource, _resourceSource, _endpointSource, _parameterSource, _optionSource, _errors, _dataTypes);

            Assert.DoesNotThrow(() => specBuilder.Build());
        }
    }
}