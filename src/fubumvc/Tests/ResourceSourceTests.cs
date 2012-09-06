using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration;
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
    public class ResourceSourceTests
    {
        public const string BatchesModuleDescription = "<b>These are batches yo!</b>";
        public const string SchedulesModuleDescription = "<p><strong>These are schedules yo!</strong></p>";

        [Test]
        public void should_find_resource_description_when_one_is_specified_in_the_same_namespaces()
        {
            var resourceDescription = new AdminUserResource();
            var action = new ActionCall(typeof(AdminUserGetAllHandler), typeof(AdminUserGetAllHandler).GetMethod("Execute"));
            var resourceSource = new ResourceSource();
            resourceSource.HasDescription(action).ShouldBeTrue();
            var resource = resourceSource.GetDescription(action);
            resource.ShouldNotBeNull();
            resource.Name.ShouldEqual(resourceDescription.Name);
            resource.Comments.ShouldEqual(resourceDescription.Comments);
        }

        [Test]
        public void should_find_resource_description_when_a_handler_type_is_specified()
        {
            var resourceDescription = new AdminAddressResource();
            var action = new ActionCall(typeof(AdminAddressGetAllHandler), typeof(AdminAddressGetAllHandler).GetMethod("Execute"));
            var resourceSource = new ResourceSource();
            resourceSource.HasDescription(action).ShouldBeTrue();
            var resource = resourceSource.GetDescription(action);
            resource.ShouldNotBeNull();
            resource.Name.ShouldEqual(resourceDescription.Name);
            resource.Comments.ShouldEqual(resourceDescription.Comments);
        }

        [Test]
        public void should_not_find_resource_description_when_none_is_specified_in_the_same_namespaces()
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