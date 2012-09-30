using System;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Swank;
using FubuMVC.Swank.Description;
using FubuMVC.Swank.Specification;
using NUnit.Framework;

namespace Tests.Specification.SpecificationServiceEndpointTests
{
    [TestFixture]
    public abstract class TestBase
    {
        protected FubuMVC.Swank.Specification.Specification Spec;

        private static readonly Func<ActionCall, bool> ActionFilter = x => x.HandlerType.InNamespace<TestBase>();

        [SetUp]
        public void Setup()
        {
            var graph = Behavior.BuildGraph().AddActionsInThisNamespace();
            var moduleSource = new ModuleSource(new MarkerSource<ModuleDescription>());
            var resourceSource = new ResourceSource(
                new MarkerSource<ResourceDescription>(),
                new ActionSource(graph, Swank.CreateConfig(x => x.AppliesToThisAssembly().Where(ActionFilter))));
            var configuration = Swank.CreateConfig(x => x.AppliesToThisAssembly().Where(ActionFilter));
            var specBuilder = new SpecificationService(configuration, new ActionSource(graph, configuration), new TypeDescriptorCache(),
                moduleSource, resourceSource, new EndpointSource(), new MemberSource(), new OptionSource(), new ErrorSource(), 
                new TypeSource(), new MergeService());
            Spec = specBuilder.Generate();
        }
    }
}