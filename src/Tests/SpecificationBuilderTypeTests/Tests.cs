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
    [TestFixture(Ignore = true)]
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
                new ActionSource(_graph,
                    ConfigurationDsl.CreateConfig(x => x.AppliesToThisAssembly().Where(y => y.HandlerType.InNamespace<SpecificationBuilderModuleTests.Tests>()))));
            _endpointSource = new EndpointSource();
            _parameterSource = new ParameterSource();
            _optionSource = new OptionSource();
            _errors = new ErrorSource();
            _dataTypes = new TypeSource();
        }

        private Specification BuildSpec<T>(Action<ConfigurationDsl> configure)
        {
            var configuration = ConfigurationDsl.CreateConfig(x => { configure(x); x.AppliesToThisAssembly().Where(y => y.HandlerType.InNamespace<T>()); });
            return new SpecificationBuilder(configuration, new ActionSource(_graph, configuration), new TypeDescriptorCache(),
                _moduleSource, _resourceSource, _endpointSource, _parameterSource, _optionSource, _errors, _dataTypes).Build();
        }

        [Test]
        public void should_not_include_input_types_from_hidden_endpoints()
        {
            //var spec = BuildSpec<>();
        }

        [Test]
        public void should_not_include_input_types_from_excluded_endpoints()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void should_not_include_output_types_from_hidden_endpoints()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void should_not_include_output_types_from_excluded_endpoints()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void should_not_include_input_types_for_get_and_delete_handlers()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void should_create_unique_input_types()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void should_set_output_type_id_to_hash_of_type_name()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void should_set_type_description()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void should_enumerate_type_members()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void should_recursively_include_member_types()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void should_not_return_duplicate_types()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void should_order_types_by_type_name()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void should_set_member_description()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void should_indicate_a_members_default_value()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void should_indicate_if_a_member_is_required()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void should_set_member_type_ids_to_hash_of_type_name()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void should_set_input_type_id_to_hash_of_type_name_and_handler_method()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void should_exclude_auto_bound_properties_from_input_type_members()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void should_exclude_url_parameters_from_input_type_members()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void should_exclude_querystring_parameters_from_input_type_members()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void should_not_include_members_marked_with_hide_on_any_type()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void should_set_system_type_members_as_the_type_name()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void should_set_non_system_type_members_as_the_type_id()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void should_set_collections_of_system_types_as_the_type_name()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void should_set_collections_of_non_system_types_as_the_type_id()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void should_enumerate_options_for_enum_members()
        {
            throw new NotImplementedException();
        }
    }
}