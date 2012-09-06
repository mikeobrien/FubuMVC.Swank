using NUnit.Framework;
using Should;
using Swank;
using Tests.Administration;
using Tests.Administration.Users;
using Tests.Batches.Schedules;
using Tests.Templates;

namespace Tests
{
    [TestFixture]
    public class ModuleSourceTests
    {
        private IModuleSource _moduleSource;

        [SetUp]
        public void Setup()
        {
            _moduleSource = new ModuleSource(new DescriptionSource<Module>());
        }

        [Test]
        public void should_find_module_description_when_one_is_specified_in_the_a_parent_namespaces()
        {
            var moduleDescription = new AdministrationModule();
            var action = TestBehaviorGraph.CreateAction<AdminUserGetAllHandler>();
            _moduleSource.HasModule(action).ShouldBeTrue();
            var module = _moduleSource.GetModule(action);
            module.ShouldNotBeNull();
            module.Name.ShouldEqual(moduleDescription.Name);
            module.Comments.ShouldEqual(moduleDescription.Comments);
        }

        [Test]
        public void should_find_nearest_module_description_when_multiple_are_defined_in_parent_namespaces()
        {
            var moduleDescription = new SchedulesModule();
            var action = TestBehaviorGraph.CreateAction<BatchScheduleGetAllHandler>();
            _moduleSource.HasModule(action).ShouldBeTrue();
            var module = _moduleSource.GetModule(action);
            module.ShouldNotBeNull();
            module.Name.ShouldEqual(moduleDescription.Name);
            module.Comments.ShouldEqual(SchedulesModule.ExpectedComments);
        }

        [Test]
        public void should_not_find_module_description_when_none_is_specified_in_any_parent_namespaces()
        {
            var action = TestBehaviorGraph.CreateAction<TemplateGetAllHandler>();
            _moduleSource.HasModule(action).ShouldBeFalse();
            _moduleSource.GetModule(action).ShouldBeNull();
        }
    }
}