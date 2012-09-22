using System;
using FubuCore.Reflection;
using FubuMVC.Swank;
using FubuMVC.Swank.Description;
using NUnit.Framework;
using ActionSource = FubuMVC.Swank.ActionSource;

namespace Tests.SpecificationBuilderTypeTests
{
    [TestFixture]
    public abstract class TestBase
    {
        protected Specification BuildSpec<TNamespace>(Action<ConfigurationDsl> configure = null)
        {
            var graph = Behaviors.BuildGraph().AddActionsInThisNamespace();
            var moduleSource = new ModuleSource(new MarkerSource<ModuleDescription>());
            var resourceSource = new ResourceSource(
                new MarkerSource<ResourceDescription>(),
                new ActionSource(graph,
                    ConfigurationDsl.CreateConfig(x => x.AppliesToThisAssembly()
                        .Where(y => y.HandlerType.InNamespace<SpecificationBuilderModuleTests.Tests>()))));
            var configuration = ConfigurationDsl.CreateConfig(x => 
                { if (configure != null) configure(x); x.AppliesToThisAssembly().Where(y => y.HandlerType.InNamespace<TNamespace>()); });
            return new SpecificationBuilder(configuration, new ActionSource(graph, configuration), new TypeDescriptorCache(),
                moduleSource, resourceSource, new EndpointSource(), new MemberSource(), new OptionSource(), new ErrorSource(), new TypeSource()).Build();
        }
    }
}