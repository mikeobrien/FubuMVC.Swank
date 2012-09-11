using System.Diagnostics;
using FubuCore.Reflection;
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
    public class SpecificationBuilderDataTypeTests
    {
        private SpecificationBuilder _specBuilder;
            
        [SetUp]
        public void Setup()
        {
            var graph = TestBehaviorGraph.Build();
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
            var spec = _specBuilder.Build();
            spec.dataTypes.Count.ShouldEqual(17);

            /* 
             * Notes on data types:
             * Should only include root input types from POST and PUTS
             * Should define a unique *input* type for each POST and PUT handler even though it may
             *     be a shared type as the handler signature could alter what is 
             *     in the type. For example querystring or url parameters will
             *     remove properties from the type.
             * If a unique *input* type for a POST or PUT ends up not containing any properties
             *     it should be considered as not accepting an input. This needs to happen in 
             *     the request section as well so that needs modded.
             * Should include all root output types, these can be shared
             * Types should properly show collections of system type i.e. List<Guid>
             * Types should properly show collections of non system types and reference 
             *     them as another type.
             *     
             */
        }
    }
}