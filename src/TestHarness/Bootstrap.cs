using Bottles;
using FubuMVC.Core;
using FubuMVC.StructureMap;
using StructureMap;
using TestHarness;

[assembly: WebActivator.PreApplicationStartMethod(typeof(Bootstrap), "Start")]
namespace TestHarness
{

    public static class Bootstrap
    {
        public static void Start()
        {
            FubuApplication.For<Conventions>().StructureMap(new Container()).Bootstrap();
            PackageRegistry.AssertNoFailures();
        }
    }
}