using System;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Swank;
using FubuMVC.Swank.Description;
using NUnit.Framework;
using ActionSource = FubuMVC.Swank.ActionSource;

namespace Tests.SpecificationBuilderEndpointTests
{
    [TestFixture]
    public abstract class TestBase
    {
        protected Specification _spec;

        private static readonly Func<ActionCall, bool> ActionFilter = x => x.HandlerType.InNamespace<TestBase>();

        [SetUp]
        public void Setup()
        {
            var graph = Behaviors.BuildGraph().AddActionsInThisNamespace();
            var moduleSource = new ModuleSource(new MarkerSource<ModuleDescription>());
            var resourceSource = new ResourceSource(
                new MarkerSource<ResourceDescription>(),
                new ActionSource(graph, ConfigurationDsl.CreateConfig(x => x.AppliesToThisAssembly().Where(ActionFilter))));
            var endpointSource = new EndpointSource();
            var memberSource = new MemberSource();
            var optionSource = new OptionSource();
            var errors = new ErrorSource();
            var dataTypes = new TypeSource();
            var configuration = ConfigurationDsl.CreateConfig(x => x.AppliesToThisAssembly().Where(ActionFilter));
            var specBuilder = new SpecificationBuilder(configuration, new ActionSource(graph, configuration), new TypeDescriptorCache(),
                moduleSource, resourceSource, endpointSource, memberSource, optionSource, errors, dataTypes);
            _spec = specBuilder.Build();
        }
    }
}