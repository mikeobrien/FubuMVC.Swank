using System;

namespace Tests.Specification.SpecificationService.MultiActionTests
{
    namespace Handlers
    {
        public class Request { public Guid Id { get; set; } }
        public class UnregisterdOutput { public Guid Id { get; set; } }
        public class PostHandler { public UnregisterdOutput Execute_Id(Request request) { return null; } }

        namespace Widgets
        {
            public class Data { public Guid Id { get; set; } }
            public class UnregisteredInput { public Guid Id { get; set; } }
            public class PostHandler { public Data Execute_Id(UnregisteredInput request) { return null; } }
        }
    }
}