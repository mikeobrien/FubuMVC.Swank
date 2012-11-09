using System;
using FubuCore.Reflection;
using FubuMVC.Swank;
using FubuMVC.Swank.Description;
using FubuMVC.Swank.Specification;
using NUnit.Framework;

namespace Tests.Specification.SpecificationServiceTypeTests
{
    [TestFixture]
    public abstract class TestBase
    {
        protected FubuMVC.Swank.Specification.Specification BuildSpec<TNamespace>(Action<Swank> configure = null)
        {
            var graph = Behavior.BuildGraph().AddActionsInThisNamespace();
            var moduleConvention = new ModuleConvention(new MarkerConvention<ModuleDescription>());
            var resourceConvention = new ResourceConvention(
                new MarkerConvention<ResourceDescription>(),
                new ActionSource(graph,
                    Swank.CreateConfig(x => x.AppliesToThisAssembly()
                        .Where(y => y.HandlerType.InNamespace<SpecificationServiceModuleTests.Tests>()))));
            var configuration = Swank.CreateConfig(x => 
                { if (configure != null) configure(x); x.AppliesToThisAssembly().Where(y => y.HandlerType.InNamespace<TNamespace>()); });
            return new SpecificationService(configuration, new ActionSource(graph, configuration), new TypeDescriptorCache(),
                moduleConvention, resourceConvention, new EndpointConvention(), new MemberConvention(), new OptionConvention(), new ErrorConvention(),
                new HeaderConvention(), new TypeConvention(), new MergeService()).Generate();
        }
    }
}