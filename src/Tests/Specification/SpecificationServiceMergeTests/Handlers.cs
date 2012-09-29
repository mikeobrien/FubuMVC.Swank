using FubuMVC.Swank.Description;

namespace Tests.Specification.SpecificationServiceMergeTests
{
    namespace NoHandlers { public class Marker { } }

    namespace OverlappingModule
    {
        public class SomeModule : ModuleDescription { public SomeModule() { Name = "Some module"; } }
        public class GetHandler { public object Execute() { return null; } }
    }

    namespace OverlappingModuleResource
    {
        public class SomeModule : ModuleDescription { public SomeModule() { Name = "Some module"; } }
        public class SomeResource : ResourceDescription { public SomeResource() { Name = "Some module resource"; } }
        public class GetHandler { public object Execute() { return null; } }
    }

    namespace OverlappingResource
    {
        public class SomeResource : ResourceDescription { public SomeResource() { Name = "Some resource"; } }
        public class GetHandler { public object Execute() { return null; } }
    }
}