using Bottles;
using FubuMVC.Core;
using FubuMVC.StructureMap;
using StructureMap;

[assembly: WebActivator.PreApplicationStartMethod(typeof(HelloWorld.Bootstrap), "Start")]

namespace HelloWorld
{
    public class Bootstrap
    {
        public static void Start()
        {
            FubuApplication.For<Conventions>()
                .StructureMap(new Container())
                .Bootstrap();
            PackageRegistry.AssertNoFailures();
        }
    }
}