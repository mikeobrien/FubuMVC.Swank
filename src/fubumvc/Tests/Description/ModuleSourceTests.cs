using FubuMVC.Core.Registration.Nodes;
using NUnit.Framework;
using Should;
using Swank.Description;
using Tests.Administration;
using Tests.Administration.Users;
using Tests.Batches.Schedules;
using Tests.Templates;

namespace Tests.Description
{
    [TestFixture]
    public class ModuleSourceTests
    {
        private IDescriptionSource<ActionCall, ModuleDescription> _moduleSource;

        [SetUp]
        public void Setup()
        {
            _moduleSource = new ModuleSource(new MarkerSource<ModuleDescription>());
        }

        [Test]
        public void should_find_module_description_when_one_is_specified_in_the_a_parent_namespaces()
        {
            var moduleDescription = new AdministrationModule();
            var action = TestBehaviorGraph.CreateAction<AdminUserGetAllHandler>();
            _moduleSource.HasDescription(action).ShouldBeTrue();
            var module = _moduleSource.GetDescription(action);
            module.ShouldNotBeNull();
            module.Name.ShouldEqual(moduleDescription.Name);
            module.Comments.ShouldEqual(moduleDescription.Comments);
        }

        [Test]
        public void should_find_nearest_module_description_when_multiple_are_defined_in_parent_namespaces()
        {
            var moduleDescription = new SchedulesModule();
            var action = TestBehaviorGraph.CreateAction<BatchScheduleGetAllHandler>();
            _moduleSource.HasDescription(action).ShouldBeTrue();
            var module = _moduleSource.GetDescription(action);
            module.ShouldNotBeNull();
            module.Name.ShouldEqual(moduleDescription.Name);
            module.Comments.ShouldEqual(SchedulesModule.ExpectedComments);
        }

        [Test]
        public void should_not_find_module_description_when_none_is_specified_in_any_parent_namespaces()
        {
            var action = TestBehaviorGraph.CreateAction<TemplateGetAllHandler>();
            _moduleSource.HasDescription(action).ShouldBeFalse();
            _moduleSource.GetDescription(action).ShouldBeNull();
        }
    }
}