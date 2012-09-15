using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;
using NUnit.Framework;
using Should;
using Swank;
using Swank.Description;
using Swank.Models;
using Tests.SpecificationBuilderDataTypeTests.Templates;
using ActionSource = Swank.ActionSource;

namespace Tests.SpecificationBuilderDataTypeTests
{
    [TestFixture]
    public class Tests
    {
        private static readonly Func<ActionCall, bool> ActionFilter =
            x => x.HandlerType.Namespace.StartsWith(typeof(Tests).Namespace);

        private List<DataType> _dataTypes;

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
            var dataTypes = new DataTypeSource();
            var configuration = ConfigurationDsl.CreateConfig(x => x.AppliesToThisAssembly().Where(ActionFilter));
            var specBuilder = new SpecificationBuilder(configuration, new ActionSource(graph, configuration), new TypeDescriptorCache(),
                moduleSource, resourceSource, endpointSource, parameterSource, optionSource, errors, dataTypes);
            _dataTypes = specBuilder.Build().dataTypes;
        }

        [Test]
        public void should_not_include_input_types_for_get_or_delete()
        {
            _dataTypes.Any(x => x.id == typeof(TemplateGetRequest).GetHash()).ShouldBeFalse();
            _dataTypes.Any(x => x.id == typeof(TemplateDeleteRequest).GetHash()).ShouldBeFalse();
        }

        [Test]
        public void should_only_include_unique_input_types()
        {
            // Reason we want this behavior is because url params can effect which 
            // properties are displayed in the docs for a type. If an input type is 
            // shared by multiple handlers (for better or worse) that have different 
            // url params then they will not need to display the properties that are
            // for input types.
            _dataTypes.Any(x => x.id == typeof(TemplatePostPutRequest).GetHash()).ShouldBeFalse();
            _dataTypes.Any(x => x.id == typeof(TemplatePostPutRequest).GetHash(typeof(TemplatePostHandler).GetExecuteMethod())).ShouldBeTrue();
            _dataTypes.Any(x => x.id == typeof(TemplatePostPutRequest).GetHash(typeof(TemplatePutHandler).GetExecuteMethod())).ShouldBeTrue();
        }
    }
}