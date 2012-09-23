using System;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Swank;
using FubuMVC.Swank.Description;
using FubuMVC.Swank.Specification;
using NUnit.Framework;

namespace Tests.Specification.SpecificationBuilderEndpointTests
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
            var endpointSource = new EndpointSource();
            var memberSource = new MemberSource();
            var optionSource = new OptionSource();
            var errors = new ErrorSource();
            var dataTypes = new TypeSource();
            var configuration = Swank.CreateConfig(x => x.AppliesToThisAssembly().Where(ActionFilter));
            var specBuilder = new SpecificationBuilder(configuration, new ActionSource(graph, configuration), new TypeDescriptorCache(),
                moduleSource, resourceSource, endpointSource, memberSource, optionSource, errors, dataTypes);
            Spec = specBuilder.Build();
        }
    }
}