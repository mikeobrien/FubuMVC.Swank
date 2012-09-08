using System;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using NUnit.Framework;
using Should;
using Swank;
using Swank.Description;
using Tests.Administration.Users;
using Tests.Batches.Cells;
using Tests.Batches.Schedules;
using Tests.Templates;

namespace Tests
{
    [TestFixture]
    public class SpecificationBuilderEndpointTests
    {
        private BehaviorGraph _graph;
        private IDescriptionSource<ActionCall, ModuleDescription> _moduleSource;
        private IDescriptionSource<ActionCall, ResourceDescription> _resourceSource;
        private IDescriptionSource<ActionCall, EndpointDescription> _endpointSource;

        [SetUp]
        public void Setup()
        {
            _graph = TestBehaviorGraph.Build();
            _moduleSource = new ModuleSource(new MarkerSource<ModuleDescription>());
            _resourceSource = new ResourceSource(
                new MarkerSource<ResourceDescription>(),
                new Swank.ActionSource(_graph, ConfigurationDsl.CreateConfig(x => x.AppliesToThisAssembly())), new ResourceSourceConfig());
            _endpointSource = new EndpointSource();
        }

        [Test]
        public void should()
        {
            var configuration = ConfigurationDsl.CreateConfig(x => x.AppliesToThisAssembly());
            var specBuilder = new SpecificationBuilder(configuration, new Swank.ActionSource(_graph, configuration), _moduleSource, _resourceSource, _endpointSource);

            var spec = specBuilder.Build();

            throw new NotImplementedException();
        }
    }
}