using FubuMVC.Swank.Description;

namespace Tests.MergeSpecificationTests
{
    namespace Handlers
    {
        public class Response { }
        public class GetHandler { public Response Execute_Status() { return null; } }

        namespace Administration
        {
            public class AdministrationModule : ModuleDescription { public AdministrationModule() { Name = "Administration"; } }

            namespace Users
            {
                public class UserResource : ModuleDescription { public UserResource() { Name = "Users"; Comments = "These are users."; } }
                public class Request {}
                public class Response {}
                public class PostHandler { public Response Execute(Request request) { return null; } }
            }
        }
    }
}