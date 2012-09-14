using FubuCore.Reflection;
using FubuMVC.Core.Registration;
using NUnit.Framework;
using Should;
using Swank;
using Swank.Description;
using ActionSource = Swank.ActionSource;

namespace Tests.SpecificationBuilderDataTypeTests
{
    [TestFixture]
    public class Tests
    {
        private SpecificationBuilder _specBuilder;
            
        [SetUp]
        public void Setup()
        {
            var graph = (BehaviorGraph)null;// TestBehaviorGraph.Build();
            var moduleSource = new ModuleSource(new MarkerSource<ModuleDescription>());
            var resourceSource = new ResourceSource(
                new MarkerSource<ResourceDescription>(),
                new ActionSource(graph, ConfigurationDsl.CreateConfig(x => x.AppliesToThisAssembly())), new ResourceSourceConfig());
            var endpointSource = new EndpointSource();
            var parameterSource = new ParameterSource();
            var optionSource = new OptionSource();
            var errors = new ErrorSource();
            var dataTypes = new DataTypeSource();
            var configuration = ConfigurationDsl.CreateConfig(x => x.AppliesToThisAssembly());
            _specBuilder = new SpecificationBuilder(configuration, new ActionSource(graph, configuration), new TypeDescriptorCache(),
                moduleSource, resourceSource, endpointSource, parameterSource, optionSource, errors, dataTypes);
        }

        [Test]
        public void should_enumerate_all_endpoint_types()
        {
            //var spec = _specBuilder.Build();
            //spec.dataTypes.Count.ShouldEqual(17);
        }
    }
}