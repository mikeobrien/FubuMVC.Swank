using System;
using System.Net;
using FubuMVC.Swank.Description;

namespace Tests.Specification.OverrideTests
{
    namespace Handlers
    {
        public class Module : ModuleDescription { public Module() : base("SomeName", "Some comments") { } }
        public class Resource : ResourceDescription { public Resource() : base("SomeName", "Some comments") { } }

        [Comments("Some comments")]
        public class Data
        {
            
            public enum Order
            {
                [Description("SomeName", "Some comments")]
                Asc
            }

            [Comments("Some comments")]
            public Order Id { get; set; }

            [Comments("Some comments")]
            public string Sort { get; set; }
        }

        [Description("SomeName", "Some comments")]
        
        public class GetHandler
        {
            [ResponseComments("Some comments")]
            [StatusCodeDescription(HttpStatusCode.InternalServerError, "SomeName", "Some comments")]
            [HeaderDescription(HttpHeaderType.Request, "SomeName", "Some comments")]
            public Data Execute_Id(Data data) { return null; }
        }

        public class PostHandler
        {
            [RequestComments("Some comments")]
            public Data Execute(Data data) { return null; }
        }
    }
}