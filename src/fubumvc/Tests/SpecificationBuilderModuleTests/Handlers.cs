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
            public class BatchScheduleAllGetHandler { public object Execute(object request) { return null; } }
        }

        namespace Cells
        {
            public class BatchCellResource : ResourceDescription { public BatchCellResource() { Name = "Batch cells"; } }
            public class BatchCellAllGetHandler { public object Execute(object request) { return null; } }
        }
    }

    namespace Administration
    {
        public class AdministrationModule : ModuleDescription
        { public AdministrationModule() { Name = "Administration"; Comments = "This is admin yo!"; } }

        public class AdminAccountResource : ResourceDescription<AdminAccountAllGetHandler>
        { public AdminAccountResource() { Name = "Accounts"; } }

        public class AdminAccountAllGetHandler { public object Execute(object request) { return null; } }

        namespace Users // These are ordered a certian way on purpose, don't change that.
        {
            public class AdminAddressResource : ResourceDescription
            { public AdminAddressResource() { Name = "User addresses"; } }

            public class AdminAddressAllOfTypeGetHandler { public object Execute_Address(object request) { return null; } }

            public class AdminUserResource : ResourceDescription<AdminUserAllGetHandler>
            { public AdminUserResource() { Name = "Users"; } }

            public class AdminUserAllGetHandler { public object Execute(object request) { return null; } }
        }
    }
}
