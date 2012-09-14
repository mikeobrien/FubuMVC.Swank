using FubuMVC.Core.Registration.Nodes;
using NUnit.Framework;
using Should;
using Swank.Description;
using Tests.Description.ModuleSourceTests.Administration;
using Tests.Description.ModuleSourceTests.Administration.Users;
using Tests.Description.ModuleSourceTests.Batches.Schedules;
using Tests.Description.ModuleSourceTests.Templates;

namespace Tests.Description.ModuleSourceTests
{
    [TestFixture]
    public class Tests
    {
        public const string SchedulesModuleComments = "<p><strong>These are schedules yo!</strong></p>";
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
            var action = Behaviors.CreateAction<AdminUserGetAllHandler>("/admin/users", HttpVerbs.Get);
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
            var action = Behaviors.CreateAction<BatchScheduleGetAllHandler>("/batches/schedules", HttpVerbs.Get);
            _moduleSource.HasDescription(action).ShouldBeTrue();
            var module = _moduleSource.GetDescription(action);
            module.ShouldNotBeNull();
            module.Name.ShouldEqual(moduleDescription.Name);
            module.Comments.ShouldEqual(SchedulesModuleComments);
        }

        [Test]
        public void should_not_find_module_description_when_none_is_specified_in_any_parent_namespaces()
        {
            var action = Behaviors.CreateAction<TemplateGetAllHandler>("/templates", HttpVerbs.Get);
            _moduleSource.HasDescription(action).ShouldBeFalse();
            _moduleSource.GetDescription(action).ShouldBeNull();
        }
    }
}