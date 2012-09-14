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
            public class BatchScheduleAllGetHandler { public object Execute(object request) { return null; } }
        }

        namespace Cells
        {
            public class BatchCellAllGetHandler { public object Execute(object request) { return null; } }
        }
    }

    namespace Administration
    {
        public class AdminAccountAllGetHandler { public object Execute(object request) { return null; } }

        namespace Users
        {
            public class AdminAddressAllOfTypeGetHandler { public object Execute_Address(object request) { return null; } }
            public class AdminUserAllGetHandler { public object Execute(object request) { return null; } }
        }
    }
}