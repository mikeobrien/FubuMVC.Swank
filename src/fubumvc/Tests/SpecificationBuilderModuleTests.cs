using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using NUnit.Framework;
using Should;
using Swank;
using Swank.Description;
using Tests.Administration;
using Tests.Batches;
using Tests.Batches.Schedules;
using Tests.Templates;

namespace Tests
{
    [TestFixture]
    public class SpecificationBuilderModuleTests
    {
        private static readonly AdministrationModule AdministrationModule = new AdministrationModule();
        private static readonly BatchesModule BatchesModule = new BatchesModule();
        private static readonly SchedulesModule SchedulesModule = new SchedulesModule();

        private BehaviorGraph _graph;
        private IDescriptionSource<ActionCall, ModuleDescription> _moduleSource;
        private IDescriptionSource<ActionCall, ResourceDescription> _resourceSource;
        private IDescriptionSource<ActionCall, EndpointDescription> _endpointSource;

        [SetUp]
        public void Setup()
        {
            _graph = TestBehaviorGraph.Build();
            _moduleSource = new ModuleSource(new MarkerSource<ModuleDescription>());
            _resourceSource = new ResourceSource(
                new MarkerSource<ResourceDescription>(),
                new Swank.ActionSource(_graph, ConfigurationDsl.CreateConfig(x => x.AppliesToThisAssembly())), new ResourceSourceConfig());
            _endpointSource = new EndpointSource();
        }

        [Test]
        public void should_return_actions_at_root_when_only_one_module_with_no_name()
        {
            var configuration = ConfigurationDsl.CreateConfig(x => x
                .AppliesToThisAssembly()
                .OnOrphanedModuleAction(OrphanedActions.UseDefault)
                .Where(y => y.HandlerType.Namespace == typeof(TemplateRequest).Namespace));
            var specBuilder = new SpecificationBuilder(configuration, new Swank.ActionSource(_graph, configuration), _moduleSource, _resourceSource, _endpointSource);

            var spec = specBuilder.Build();

            spec.modules.Count.ShouldEqual(0);
            spec.resources.Count.ShouldEqual(2);
        }

        [Test]
        public void should_automatically_add_orphaned_actions_to_empty_default_module()
        {
            var configuration = ConfigurationDsl.CreateConfig(x => x
                .AppliesToThisAssembly()
                .OnOrphanedModuleAction(OrphanedActions.UseDefault));
            var specBuilder = new SpecificationBuilder(configuration, new Swank.ActionSource(_graph, configuration), _moduleSource, _resourceSource, _endpointSource);

            var spec = specBuilder.Build();

            spec.modules.Count.ShouldEqual(4);
            spec.resources.Count.ShouldEqual(0);

            spec.modules[0].name.ShouldBeNull();
            spec.modules[0].comments.ShouldBeNull();

            spec.modules[1].name.ShouldEqual(AdministrationModule.Name);
            spec.modules[1].comments.ShouldEqual(AdministrationModule.Comments);

            spec.modules[2].name.ShouldEqual(BatchesModule.Name);
            spec.modules[2].comments.ShouldEqual(BatchesModule.ExpectedComments);

            spec.modules[3].name.ShouldEqual(SchedulesModule.Name);
            spec.modules[3].comments.ShouldEqual(SchedulesModule.ExpectedComments);
        }

        [Test]
        public void should_automatically_add_orphaned_actions_to_specified_default_module()
        {
            var configuration = ConfigurationDsl.CreateConfig(x => x
                .AppliesToThisAssembly()
                .OnOrphanedModuleAction(OrphanedActions.UseDefault)
                .WithDefaultModule(y => new ModuleDescription { Name = "API", Comments = "This is the API yo!" }));
            var specBuilder = new SpecificationBuilder(configuration, new Swank.ActionSource(_graph, configuration), _moduleSource, _resourceSource, _endpointSource);

            var spec = specBuilder.Build();

            spec.modules.Count.ShouldEqual(4);
            spec.resources.Count.ShouldEqual(0);

            spec.modules[0].name.ShouldEqual(AdministrationModule.Name);
            spec.modules[0].comments.ShouldEqual(AdministrationModule.Comments);

            spec.modules[1].name.ShouldEqual("API");
            spec.modules[1].comments.ShouldEqual("This is the API yo!");

            spec.modules[2].name.ShouldEqual(BatchesModule.Name);
            spec.modules[2].comments.ShouldEqual(BatchesModule.ExpectedComments);

            spec.modules[3].name.ShouldEqual(SchedulesModule.Name);
            spec.modules[3].comments.ShouldEqual(SchedulesModule.ExpectedComments);
        }

        [Test]
        public void should_ignore_orphaned_actions()
        {
            var configuration = ConfigurationDsl.CreateConfig(x => x
                .AppliesToThisAssembly()
                .OnOrphanedModuleAction(OrphanedActions.Exclude));
            var specBuilder = new SpecificationBuilder(configuration, new Swank.ActionSource(_graph, configuration), _moduleSource, _resourceSource, _endpointSource);

            var spec = specBuilder.Build();

            spec.modules.Count.ShouldEqual(3);
            spec.resources.Count.ShouldEqual(0);

            spec.modules[0].name.ShouldEqual(AdministrationModule.Name);
            spec.modules[0].comments.ShouldEqual(AdministrationModule.Comments);

            spec.modules[1].name.ShouldEqual(BatchesModule.Name);
            spec.modules[1].comments.ShouldEqual(BatchesModule.ExpectedComments);

            spec.modules[2].name.ShouldEqual(SchedulesModule.Name);
            spec.modules[2].comments.ShouldEqual(SchedulesModule.ExpectedComments);
        }

        [Test]
        public void should_throw_an_exception_for_orphaned_actions()
        {
            var configuration = ConfigurationDsl.CreateConfig(x => x
                .AppliesToThisAssembly()
                .OnOrphanedModuleAction(OrphanedActions.Fail));
            var specBuilder = new SpecificationBuilder(configuration, new Swank.ActionSource(_graph, configuration), _moduleSource, _resourceSource, _endpointSource);

            Assert.Throws<OrphanedModuleActionException>(() => specBuilder.Build());
        }

        [Test]
        public void should_not_throw_an_exception_when_there_are_no_orphaned_actions()
        {
            var configuration = ConfigurationDsl.CreateConfig(x => x
                .AppliesToThisAssembly()
                .OnOrphanedModuleAction(OrphanedActions.Fail)
                .Where(y => y.HandlerType.Namespace != typeof(TemplateRequest).Namespace));
            var specBuilder = new SpecificationBuilder(configuration, new Swank.ActionSource(_graph, configuration), _moduleSource, _resourceSource, _endpointSource);

            Assert.DoesNotThrow(() => specBuilder.Build());
        }
    }
}