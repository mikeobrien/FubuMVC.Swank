using FubuMVC.Core.Registration;
using NUnit.Framework;
using Should;
using Swank;
using Tests.Administration;
using Tests.Batches;
using Tests.Batches.Schedules;
using Tests.Templates;

namespace Tests
{
    [TestFixture]
    public class SpecificationBuilderModuleTests
    {
        private BehaviorGraph _graph;
        private static readonly ModuleDescription AdministrationModule = new AdministrationModule();
        private static readonly ModuleDescription BatchesModule = new BatchesModule();
        private static readonly ModuleDescription SchedulesModule = new SchedulesModule();

        [SetUp]
        public void Setup()
        {
            _graph = TestBehaviorGraph.Build();
        }

        [Test]
        public void should_add_orphaned_actions_to_empty_default_module()
        {
            var configuration = new Configuration();
            new ConfigurationDsl(configuration)
                .AppliesToThisAssembly()
                .OnOrphanedAction(OrphanedActionsBehavior.AddToDefaultModule);
            var specBuilder = new SpecificationBuilder(configuration, _graph);

            var spec = specBuilder.Build();

            spec.modules.Count.ShouldEqual(4);

            spec.modules[0].name.ShouldBeNull();
            spec.modules[0].description.ShouldBeNull();

            spec.modules[1].name.ShouldEqual(AdministrationModule.Name);
            spec.modules[1].description.ShouldEqual(AdministrationModule.Comments);

            spec.modules[2].name.ShouldEqual(BatchesModule.Name);
            spec.modules[2].description.ShouldEqual(ModuleSourceTests.BatchesModuleDescription);

            spec.modules[3].name.ShouldEqual(SchedulesModule.Name);
            spec.modules[3].description.ShouldEqual(ModuleSourceTests.SchedulesModuleDescription);
        }

        [Test]
        public void should_add_orphaned_actions_to_specified_default_module()
        {
            var configuration = new Configuration();
            new ConfigurationDsl(configuration)
                .AppliesToThisAssembly()
                .OnOrphanedAction(OrphanedActionsBehavior.AddToDefaultModule)
                .WithDefaultModule("API", "This is the API yo!");
            var specBuilder = new SpecificationBuilder(configuration, _graph);

            var spec = specBuilder.Build();

            spec.modules.Count.ShouldEqual(4);

            spec.modules[0].name.ShouldEqual(AdministrationModule.Name);
            spec.modules[0].description.ShouldEqual(AdministrationModule.Comments);

            spec.modules[1].name.ShouldEqual(configuration.DefaultModule.Name);
            spec.modules[1].description.ShouldEqual(configuration.DefaultModule.Comments);

            spec.modules[2].name.ShouldEqual(BatchesModule.Name);
            spec.modules[2].description.ShouldEqual(ModuleSourceTests.BatchesModuleDescription);

            spec.modules[3].name.ShouldEqual(SchedulesModule.Name);
            spec.modules[3].description.ShouldEqual(ModuleSourceTests.SchedulesModuleDescription);
        }

        [Test]
        public void should_ignore_orphaned_actions()
        {
            var configuration = new Configuration();
            new ConfigurationDsl(configuration)
                .AppliesToThisAssembly()
                .OnOrphanedAction(OrphanedActionsBehavior.Ignore);
            var specBuilder = new SpecificationBuilder(configuration, _graph);

            var spec = specBuilder.Build();

            spec.modules.Count.ShouldEqual(3);

            spec.modules[0].name.ShouldEqual(AdministrationModule.Name);
            spec.modules[0].description.ShouldEqual(AdministrationModule.Comments);

            spec.modules[1].name.ShouldEqual(BatchesModule.Name);
            spec.modules[1].description.ShouldEqual(ModuleSourceTests.BatchesModuleDescription);

            spec.modules[2].name.ShouldEqual(SchedulesModule.Name);
            spec.modules[2].description.ShouldEqual(ModuleSourceTests.SchedulesModuleDescription);
        }

        [Test]
        public void should_throw_an_exception_for_orphaned_actions()
        {
            var configuration = new Configuration();
            new ConfigurationDsl(configuration)
                .AppliesToThisAssembly()
                .OnOrphanedAction(OrphanedActionsBehavior.ThrowException);
            var specBuilder = new SpecificationBuilder(configuration, _graph);

            Assert.Throws<OrphanedActionException>(() => specBuilder.Build());
        }

        [Test]
        public void should_enumerate_actions_in_all_assemblies_by_default()
        {
            var configuration = new Configuration();
            new ConfigurationDsl(configuration)
                .OnOrphanedAction(OrphanedActionsBehavior.AddToDefaultModule)
                .WithDefaultModule("API", "This is the API yo!")
                .Where(x => x.HandlerType.Namespace != typeof(TemplateRequest).Namespace);
            _graph.AddAction<SpecificationHandler>("/documentation", HttpVerbs.Get);
            var specBuilder = new SpecificationBuilder(configuration, _graph);

            var spec = specBuilder.Build();

            spec.modules.Count.ShouldEqual(4);

            spec.modules[0].name.ShouldEqual(AdministrationModule.Name);
            spec.modules[0].description.ShouldEqual(AdministrationModule.Comments);

            spec.modules[1].name.ShouldEqual(configuration.DefaultModule.Name);
            spec.modules[1].description.ShouldEqual(configuration.DefaultModule.Comments);

            spec.modules[2].name.ShouldEqual(BatchesModule.Name);
            spec.modules[2].description.ShouldEqual(ModuleSourceTests.BatchesModuleDescription);

            spec.modules[3].name.ShouldEqual(SchedulesModule.Name);
            spec.modules[3].description.ShouldEqual(ModuleSourceTests.SchedulesModuleDescription);
        }

        [Test]
        public void should_only_enumerate_actions_in_the_specified_assemblies()
        {
            var configuration = new Configuration();
            new ConfigurationDsl(configuration)
                .AppliesToThisAssembly()
                .OnOrphanedAction(OrphanedActionsBehavior.AddToDefaultModule)
                .WithDefaultModule("API", "This is the API yo!")
                .Where(x => x.HandlerType.Namespace != typeof(TemplateRequest).Namespace);
            _graph.AddAction<SpecificationHandler>("/documentation", HttpVerbs.Get);
            var specBuilder = new SpecificationBuilder(configuration, _graph);

            var spec = specBuilder.Build();

            spec.modules.Count.ShouldEqual(3);

            spec.modules[0].name.ShouldEqual(AdministrationModule.Name);
            spec.modules[0].description.ShouldEqual(AdministrationModule.Comments);

            spec.modules[1].name.ShouldEqual(BatchesModule.Name);
            spec.modules[1].description.ShouldEqual(ModuleSourceTests.BatchesModuleDescription);

            spec.modules[2].name.ShouldEqual(SchedulesModule.Name);
            spec.modules[2].description.ShouldEqual(ModuleSourceTests.SchedulesModuleDescription);
        }

        [Test]
        public void should_filter_actions_based_on_filter_in_the_configuration()
        {
            var configuration = new Configuration();
            new ConfigurationDsl(configuration)
                .AppliesToThisAssembly()
                .Where(x => x.ParentChain().Route.FirstPatternSegment() != "batches")
                .OnOrphanedAction(OrphanedActionsBehavior.Ignore);
            var specBuilder = new SpecificationBuilder(configuration, _graph);

            var spec = specBuilder.Build();

            spec.modules.Count.ShouldEqual(1);

            spec.modules[0].name.ShouldEqual(AdministrationModule.Name);
            spec.modules[0].description.ShouldEqual(AdministrationModule.Comments);
        }
    }
}