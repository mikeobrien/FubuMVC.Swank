using FubuMVC.Swank.Description;

namespace Tests.Description.ModuleConventionTests
{
    namespace ModuleDescriptions
    {
        namespace NoDescription
        {
            public class Module : ModuleDescription { }
            public class GetHandler { public object Execute(object request) { return null; } }
        }

        namespace Description
        {
            public class Module : ModuleDescription { public Module() { Name = "Some Module"; Comments = "Some comments."; } }
            public class GetHandler { public object Execute(object request) { return null; } }
        }

        namespace EmbeddedTextComments
        {
            public class Module : ModuleDescription { public Module() { Name = "Some Text Module"; } }
            public class GetHandler { public object Execute(object request) { return null; } }
        }

        namespace EmbeddedMarkdownComments
        {
            public class Module : ModuleDescription { public Module() { Name = "Some Markdown Module"; } }
            public class GetHandler { public object Execute(object request) { return null; } }
        }
    }

    namespace NoModules
    {
        public class GetHandler { public object Execute(object request) { return null; } }
    }

    namespace NestedModules
    {
        namespace NoModules
        {
            public class GetHandler { public object Execute(object request) { return null; } }
        }

        public class RootModule : ModuleDescription { public RootModule() { Name = "Root Module"; } }
        public class GetHandler { public object Execute(object request) { return null; } }

        namespace NestedModule
        {
            public class NestedModule : ModuleDescription { public NestedModule() { Name = "Nested Module"; } }
            public class GetHandler { public object Execute(object request) { return null; } }
        }
    }
}