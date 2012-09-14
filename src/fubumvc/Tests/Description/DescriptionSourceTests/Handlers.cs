using Swank.Description;

namespace Tests.Description.DescriptionSourceTests
{
    namespace Batches
    {
        public class BatchesModule : ModuleDescription { public BatchesModule() { Name = "Batches"; } }

        namespace Schedules
        { public class SchedulesModule : ModuleDescription { public SchedulesModule() { Name = "Schedules"; } } }

        namespace Cells
        {
            public class BatchCellResource : ResourceDescription
            { public BatchCellResource() { Name = "Batch cells"; Comments = "These are batch cells yo!"; } }
        }
    }

    namespace Administration
    {
        public class AdministrationModule : ModuleDescription
        { public AdministrationModule() { Name = "Administration"; Comments = "This is admin yo!"; } }

        namespace Users // These are ordered a certian way on purpose, don't change that.
        {
            public class AdminAccountResource : ResourceDescription<AdminAccountGetAllHandler>
            { public AdminAccountResource() { Name = "Accounts"; Comments = "These are accounts yo!"; } }

            public class AdminAccountGetAllHandler { public object Execute(object request) { return null; } }

            public class AdminAddressResource : ResourceDescription
            { public AdminAddressResource() { Name = "User addresses"; Comments = "These are user addresses yo!"; } }

            public class AdminUserResource : ResourceDescription<AdminUserGetAllHandler>
            { public AdminUserResource() { Name = "Users"; Comments = "These are users yo!"; } }

            public class AdminUserGetAllHandler { public object Execute(object request) { return null; } }
        }
    }
}