using System;
using System.Collections.Generic;

namespace Tests.SpecificationBuilderTypeTests
{
    namespace ExcludedHandlers
    {
        namespace Excluded
        {
            public class Request {}
            public class Response {}
            public class PostHandler { public Response Execute(Request request) { return null; } }
        }
    }
}