using Swank.Description;

namespace Tests.SpecificationBuilderModuleTests
{
    namespace Templates
    { public class TemplatePutHandler { public object Execute(object request) { return null; } } }

    namespace Batches
    {
        public class BatchesModule : ModuleDescription { public BatchesModule() { Name = "Batches"; } }

        namespace Schedules
        {
            public class SchedulesModule : ModuleDescription { public SchedulesModule() { Name = "Schedules"; } }
            public class BatchScheduleGetAllHandler { public object Execute(object request) { return null; } }
        }

        namespace Cells
        {
            public class BatchCellResource : ResourceDescription
            { public BatchCellResource() { Name = "Batch cells"; Comments = "These are batch cells yo!"; } }

            public class BatchCellGetAllHandler { public object Execute(object request) { return null; } }
        }
    }

    namespace Administration
    {
        public class AdministrationModule : ModuleDescription
        { public AdministrationModule() { Name = "Administration"; Comments = "This is admin yo!"; } }

        public class AdminAccountResource : ResourceDescription<AdminAccountGetAllHandler>
        { public AdminAccountResource() { Name = "Accounts"; Comments = "These are accounts yo!"; } }

        public class AdminAccountGetAllHandler { public object Execute(object request) { return null; } }

        namespace Users // These are ordered a certian way on purpose, don't change that.
        {
            public class AdminAddressResource : ResourceDescription
            { public AdminAddressResource() { Name = "User addresses"; Comments = "These are user addresses yo!"; } }

            public class AdminAddressGetAllOfTypeHandler { public object Execute(object request) { return null; } }

            public class AdminUserResource : ResourceDescription<AdminUserGetAllHandler>
            { public AdminUserResource() { Name = "Users"; Comments = "These are users yo!"; } }

            public class AdminUserGetAllHandler { public object Execute(object request) { return null; } }
        }
    }
}
