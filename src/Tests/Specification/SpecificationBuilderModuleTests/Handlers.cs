using FubuMVC.Swank.Description;

namespace Tests.Specification.SpecificationBuilderModuleTests
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

    namespace OneModuleAndOrphanedAction
    {
        public class GetHandler { public object Execute_Orphan(object request) { return null; } }
        namespace WithModule
        {
            public class EmptyModule : ModuleDescription { public EmptyModule() { Name = "Some Module"; } }
            public class GetHandler { public object Execute_InModule(object request) { return null; } }
        }
    }

    namespace NestedModules
    {
        namespace NoModule
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
