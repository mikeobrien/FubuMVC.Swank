using System;

namespace Tests.Specification.ActionSourceTests
{
    namespace Handlers
    {
        public class Request { public Guid Id { get; set; } }

        public class AllGetHandler { public object Execute(Request request) { return null; } }
        public class GetHandler { public object Execute_Id(Request request) { return null; } }

        namespace Widgets
        {
            public class AllGetHandler { public object Execute(Request request) { return null; } }
            public class GetHandler { public object Execute_Id(Request request) { return null; } }
        }
    }
}