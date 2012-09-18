using System;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;
using NUnit.Framework;
using Swank;
using Swank.Description;
using Swank.Models;
using ActionSource = Swank.ActionSource;

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
                new ActionSource(graph, ConfigurationDsl.CreateConfig(x => x.AppliesToThisAssembly().Where(ActionFilter))), new ResourceSourceConfig());
            var endpointSource = new EndpointSource();
            var parameterSource = new ParameterSource();
            var optionSource = new OptionSource();
            var errors = new ErrorSource();
            var dataTypes = new TypeSource();
            var configuration = ConfigurationDsl.CreateConfig(x => x.AppliesToThisAssembly().Where(ActionFilter));
            var specBuilder = new SpecificationBuilder(configuration, new ActionSource(graph, configuration), new TypeDescriptorCache(),
                moduleSource, resourceSource, endpointSource, parameterSource, optionSource, errors, dataTypes);
            _spec = specBuilder.Build();
        }
    }
}