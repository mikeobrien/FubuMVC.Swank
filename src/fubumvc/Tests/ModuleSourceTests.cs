using FubuMVC.Core.Registration.Nodes;
using NUnit.Framework;
using Should;
using Swank;
using Tests.Administration;
using Tests.Administration.Users;
using Tests.Batches.Cells;
using Tests.Batches.Schedules;
using Tests.Templates;

namespace Tests
{
    [TestFixture]
    public class ModuleSourceTests
    {
        public const string BatchesModuleDescription = "<b>These are batches yo!</b>";
        public const string SchedulesModuleDescription = "<p><strong>These are schedules yo!</strong></p>";

        [Test]
        public void should_find_module_description_when_one_is_specified_in_the_a_parent_namespaces()
        {
            var moduleDescription = new AdministrationModule();
            var action = new ActionCall(typeof(AdminUserGetAllHandler), typeof(AdminUserGetAllHandler).GetMethod("Execute"));
            var moduleSource = new ModuleSource();
            moduleSource.HasDescription(action).ShouldBeTrue();
            var module = moduleSource.GetDescription(action);
            module.ShouldNotBeNull();
            module.Name.ShouldEqual(moduleDescription.Name);
            module.Comments.ShouldEqual(moduleDescription.Comments);
        }

        [Test]
        public void should_find_nearest_module_description_when_multiple_are_defined_in_parent_namespaces()
        {
            var moduleDescription = new SchedulesModule();
            var action = new ActionCall(typeof(BatchScheduleGetAllHandler), typeof(BatchScheduleGetAllHandler).GetMethod("Execute"));
            var moduleSource = new ModuleSource();
            moduleSource.HasDescription(action).ShouldBeTrue();
            var module = moduleSource.GetDescription(action);
            module.ShouldNotBeNull();
            module.Name.ShouldEqual(moduleDescription.Name);
            module.Comments.ShouldEqual(SchedulesModuleDescription);
        }

        [Test]
        public void should_not_find_module_description_when_none_is_specified_in_any_parent_namespaces()
        {
            var action = new ActionCall(typeof(TemplateGetAllHandler), typeof(TemplateGetAllHandler).GetMethod("Execute"));
            var moduleSource = new ModuleSource();
            moduleSource.HasDescription(action).ShouldBeFalse();
            moduleSource.GetDescription(action).ShouldBeNull();
        }

        [Test]
        public void should_pull_description_from_embedded_text_file()
        {
            var action = new ActionCall(typeof(BatchCellGetAllHandler), typeof(BatchCellGetAllHandler).GetMethod("Execute"));
            var moduleSource = new ModuleSource();
            moduleSource.GetDescription(action).Comments.ShouldEqual(BatchesModuleDescription);
        }

        [Test]
        public void should_pull_description_from_embedded_markdown_file()
        {
            var action = new ActionCall(typeof(BatchScheduleGetAllHandler), typeof(BatchScheduleGetAllHandler).GetMethod("Execute"));
            var moduleSource = new ModuleSource();
            moduleSource.GetDescription(action).Comments.ShouldEqual(SchedulesModuleDescription);
        }
    }
}