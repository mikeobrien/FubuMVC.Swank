using Swank.Description;

namespace Tests.Description.ModuleSourceTests
{
    namespace Templates
    {
        public class TemplateGetAllHandler
        {
            public object Execute(object request) { return null; }
        }
    }

    namespace Batches
    {
        public class BatchesModule : ModuleDescription { public BatchesModule() { Name = "Batches"; } }

        namespace Schedules
        {
            public class SchedulesModule : ModuleDescription { public SchedulesModule() { Name = "Schedules"; } }
            public class BatchScheduleGetAllHandler { public object Execute(object request) { return null; } }
        }
    }

    namespace Administration
    {
        public class AdministrationModule : ModuleDescription { public AdministrationModule() { Name = "Administration"; } }

        namespace Users
        {
            public class AdminUserGetAllHandler { public object Execute(object request) { return null; } }
        }
    }
}