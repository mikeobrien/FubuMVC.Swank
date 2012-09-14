namespace Tests.ActionSourceTests
{
    namespace Templates
    {
        public class TemplatePutHandler { public object Execute(object request) { return null; } }
    }

    namespace Batches
    {
        namespace Schedules
        {
            public class BatchScheduleGetAllHandler { public object Execute(object request) { return null; } }
        }

        namespace Cells
        {
            public class BatchCellGetAllHandler { public object Execute(object request) { return null; } }
        }
    }

    namespace Administration
    {
        public class AdminAccountGetAllHandler { public object Execute(object request) { return null; } }

        namespace Users
        {
            public class AdminAddressGetAllOfTypeHandler { public object Execute(object request) { return null; } }
            public class AdminUserGetAllHandler { public object Execute(object request) { return null; } }
        }
    }
}