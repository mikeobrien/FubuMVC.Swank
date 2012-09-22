using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore.Reflection;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Swank;
using FubuMVC.Swank.Description;
using NUnit.Framework;
using Should;
using ActionSource = FubuMVC.Swank.ActionSource;

namespace Tests.SpecificationBuilderTypeTests
{
    [TestFixture]
    public abstract class TestBase
    {
        private BehaviorGraph _graph;
        private IDescriptionSource<ActionCall, ModuleDescription> _moduleSource;
        private IDescriptionSource<ActionCall, ResourceDescription> _resourceSource;
        private IDescriptionSource<ActionCall, EndpointDescription> _endpointSource;
        private IDescriptionSource<PropertyInfo, FubuMVC.Swank.Description.MemberDescription> _memberSource;
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
                new ActionSource(_graph,
                    ConfigurationDsl.CreateConfig(x => x.AppliesToThisAssembly().Where(y => y.HandlerType.InNamespace<SpecificationBuilderModuleTests.Tests>()))));
            _endpointSource = new EndpointSource();
            _memberSource = new MemberSource();
            _optionSource = new OptionSource();
            _errors = new ErrorSource();
            _dataTypes = new TypeSource();
        }

        protected Specification BuildSpec<TNamespace>(Action<ConfigurationDsl> configure = null)
        {
            var configuration = ConfigurationDsl.CreateConfig(x => { if (configure != null) configure(x); x.AppliesToThisAssembly().Where(y => y.HandlerType.InNamespace<TNamespace>()); });
            return new SpecificationBuilder(configuration, new ActionSource(_graph, configuration), new TypeDescriptorCache(),
                _moduleSource, _resourceSource, _endpointSource, _memberSource, _optionSource, _errors, _dataTypes).Build();
        }
    }
}