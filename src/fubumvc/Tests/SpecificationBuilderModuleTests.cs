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
        private static readonly AdministrationModule AdministrationModule = new AdministrationModule();
        private static readonly BatchesModule BatchesModule = new BatchesModule();
        private static readonly SchedulesModule SchedulesModule = new SchedulesModule();

        private BehaviorGraph _graph;
        private IModuleSource _moduleSource;
        private IResourceSource _resourceSource;

        [SetUp]
        public void Setup()
        {
            _graph = TestBehaviorGraph.Build();
            _moduleSource = new ModuleSource(new DescriptionSource<Module>());
            _resourceSource = new ResourceSource(
                new DescriptionSource<Resource>(),
                new Swank.ActionSource(_graph, ConfigurationDsl.CreateConfig(x => x.AppliesToThisAssembly())), new ResourceSourceConfig());
        }

        [Test]
        public void should_automatically_add_orphaned_actions_to_empty_default_module()
        {
            var configuration = ConfigurationDsl.CreateConfig(x => x
                .AppliesToThisAssembly()
                .OnOrphanedModuleAction(OrphanedActions.Default));
            var specBuilder = new SpecificationBuilder(configuration, new Swank.ActionSource(_graph, configuration), _moduleSource, _resourceSource);

            var spec = specBuilder.Build();

            spec.modules.Count.ShouldEqual(4);

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
                .OnOrphanedModuleAction(OrphanedActions.Default)
                .WithDefaultModule(y => new Module { Name = "API", Comments = "This is the API yo!" }));
            var specBuilder = new SpecificationBuilder(configuration, new Swank.ActionSource(_graph, configuration), _moduleSource, _resourceSource);

            var spec = specBuilder.Build();

            spec.modules.Count.ShouldEqual(4);

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
            var specBuilder = new SpecificationBuilder(configuration, new Swank.ActionSource(_graph, configuration), _moduleSource, _resourceSource);

            var spec = specBuilder.Build();

            spec.modules.Count.ShouldEqual(3);

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
            var specBuilder = new SpecificationBuilder(configuration, new Swank.ActionSource(_graph, configuration), _moduleSource, _resourceSource);

            Assert.Throws<OrphanedModuleActionException>(() => specBuilder.Build());
        }

        [Test]
        public void should_not_throw_an_exception_when_there_are_no_orphaned_actions()
        {
            var configuration = ConfigurationDsl.CreateConfig(x => x
                .AppliesToThisAssembly()
                .OnOrphanedModuleAction(OrphanedActions.Fail)
                .Where(y => y.HandlerType.Namespace != typeof(TemplateRequest).Namespace));
            var specBuilder = new SpecificationBuilder(configuration, new Swank.ActionSource(_graph, configuration), _moduleSource, _resourceSource);

            Assert.DoesNotThrow(() => specBuilder.Build());
        }
    }
}