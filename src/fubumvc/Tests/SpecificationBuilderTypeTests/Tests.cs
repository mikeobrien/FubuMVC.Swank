using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore.Reflection;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using NUnit.Framework;
using Should;
using Swank;
using Swank.Description;
using Swank.Models;

namespace Tests.SpecificationBuilderTypeTests
{
    [TestFixture]
    public class Tests
    {
        private BehaviorGraph _graph;
        private IDescriptionSource<ActionCall, ModuleDescription> _moduleSource;
        private IDescriptionSource<ActionCall, ResourceDescription> _resourceSource;
        private IDescriptionSource<ActionCall, EndpointDescription> _endpointSource;
        private IDescriptionSource<PropertyInfo, ParameterDescription> _parameterSource;
        private IDescriptionSource<FieldInfo, OptionDescription> _optionSource;
        private IDescriptionSource<ActionCall, List<ErrorDescription>> _errors;
        private IDescriptionSource<System.Type, DataTypeDescription> _dataTypes;

        [SetUp]
        public void Setup()
        {
            _graph = Behaviors.BuildGraph().AddActionsInThisNamespace();
            _moduleSource = new ModuleSource(new MarkerSource<ModuleDescription>());
            _resourceSource = new ResourceSource(
                new MarkerSource<ResourceDescription>(),
                new Swank.ActionSource(_graph,
                    ConfigurationDsl.CreateConfig(x => x.AppliesToThisAssembly().Where(y => y.HandlerType.InNamespace<SpecificationBuilderModuleTests.Tests>()))),
                    new ResourceSourceConfig());
            _endpointSource = new EndpointSource();
            _parameterSource = new ParameterSource();
            _optionSource = new OptionSource();
            _errors = new ErrorSource();
            _dataTypes = new TypeSource();
        }

        private Specification BuildSpec<T>(Action<ConfigurationDsl> configure)
        {
            var configuration = ConfigurationDsl.CreateConfig(x => { configure(x); x.AppliesToThisAssembly().Where(y => y.HandlerType.InNamespace<T>()); });
            return new SpecificationBuilder(configuration, new Swank.ActionSource(_graph, configuration), new TypeDescriptorCache(),
                _moduleSource, _resourceSource, _endpointSource, _parameterSource, _optionSource, _errors, _dataTypes).Build();
        }

        [Test]
        public void should()
        {
        }

    }
}