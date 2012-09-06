using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Should;
using Swank;
using Tests.Administration;
using Tests.Administration.Users;
using Tests.Batches;
using Tests.Batches.Cells;
using Tests.Batches.Schedules;

namespace Tests
{
    [TestFixture]
    public class DescriptionSourceTests
    {
        private static readonly AdministrationModule AdministrationModule = new AdministrationModule();
        private static readonly BatchesModule BatchesModule = new BatchesModule();
        private static readonly SchedulesModule SchedulesModule = new SchedulesModule();

        private static readonly BatchCellResource BatchCellResource = new BatchCellResource();
        private static readonly AdminAccountResource AdminAccountResource = new AdminAccountResource();
        private static readonly AdminAddressResource AdminAddressResource = new AdminAddressResource();
        private static readonly AdminUserResource AdminUserResource = new AdminUserResource();

        private IList<Swank.Module> _modules;
        private IList<Resource> _resources;

        [SetUp]
        public void Setup()
        {
            _modules = new DescriptionSource<Swank.Module>().GetDescriptions(Assembly.GetExecutingAssembly());
            _resources = new DescriptionSource<Resource>().GetDescriptions(Assembly.GetExecutingAssembly());
        }

        [Test]
        public void should_enumerate_descriptions()
        {
            _modules.Count.ShouldEqual(3);
            _resources.Count.ShouldEqual(4);
        }

        [Test]
        public void should_be_ordered_by_descending_namespace_and_ascending_name()
        {
            _modules[0].Name.ShouldEqual(SchedulesModule.Name);
            _modules[1].Name.ShouldEqual(BatchesModule.Name);
            _modules[2].Name.ShouldEqual(AdministrationModule.Name);

            _resources[0].Name.ShouldEqual(BatchCellResource.Name);
            _resources[1].Name.ShouldEqual(AdminAccountResource.Name);
            _resources[2].Name.ShouldEqual(AdminAddressResource.Name);
            _resources[3].Name.ShouldEqual(AdminUserResource.Name);
        }

        [Test]
        public void should_set_namespace()
        {
            _modules[0].Namespace.ShouldEqual(SchedulesModule.GetType().Namespace);
            _modules[1].Namespace.ShouldEqual(BatchesModule.GetType().Namespace);
            _modules[2].Namespace.ShouldEqual(AdministrationModule.GetType().Namespace);

            _resources[0].Namespace.ShouldEqual(BatchCellResource.GetType().Namespace);
            _resources[1].Namespace.ShouldEqual(AdminAccountResource.GetType().Namespace);
            _resources[2].Namespace.ShouldEqual(AdminAddressResource.GetType().Namespace);
            _resources[3].Namespace.ShouldEqual(AdminUserResource.GetType().Namespace);
        }

        [Test]
        public void should_set_applies_to()
        {
            _modules[0].AppliesTo.ShouldBeNull();
            _modules[1].AppliesTo.ShouldBeNull();
            _modules[2].AppliesTo.ShouldBeNull();

            _resources[0].AppliesTo.ShouldBeNull();
            _resources[1].AppliesTo.ShouldEqual(typeof(AdminAccountGetAllHandler));
            _resources[2].AppliesTo.ShouldBeNull();
            _resources[3].AppliesTo.ShouldEqual(typeof(AdminUserGetAllHandler));
        }

        [Test]
        public void should_set_description_comments_from_code()
        {
            _modules[2].Comments.ShouldEqual(AdministrationModule.Comments);
        }

        [Test]
        public void should_set_description_comments_from_embedded_text()
        {
            _modules[1].Comments.ShouldEqual(BatchesModule.ExpectedComments);
        }

        [Test]
        public void should_set_description_comments_from_embedded_markdown()
        {
            _modules[0].Comments.ShouldEqual(SchedulesModule.ExpectedComments);
        }
    }
}