using System;
using System.Collections.Generic;
using System.Reflection;
using FubuCore.Reflection;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using NUnit.Framework;
using Should;
using Swank;
using Swank.Description;
using Swank.Models;

namespace Tests.SpecificationBuilderModuleTests
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
        private IDescriptionSource<Type, DataTypeDescription> _dataTypes;

        [SetUp]
        public void Setup()
        {
            _graph = Behaviors.BuildGraph().AddActionsInThisNamespace();
            _moduleSource = new ModuleSource(new MarkerSource<ModuleDescription>());
            _resourceSource = new ResourceSource(
                new MarkerSource<ResourceDescription>(),
                new Swank.ActionSource(_graph, ConfigurationDsl.CreateConfig(x => x.AppliesToThisAssembly())), new ResourceSourceConfig());
            _endpointSource = new EndpointSource();
            _parameterSource = new ParameterSource();
            _optionSource = new OptionSource();
            _errors = new ErrorSource();
            _dataTypes = new DataTypeSource();
        }

        private Specification BuildSpec<T>(Action<ConfigurationDsl> configure)
        {
            var configuration = ConfigurationDsl.CreateConfig(x => { configure(x); x.AppliesToThisAssembly().Where(y => y.HandlerType.InNamespace<T>()); });
            return new SpecificationBuilder(configuration, new Swank.ActionSource(_graph, configuration), new TypeDescriptorCache(),
                _moduleSource, _resourceSource, _endpointSource, _parameterSource, _optionSource, _errors, _dataTypes).Build();
        }

        [Test]
        public void should_set_description_when_one_is_specified()
        {
            var spec = BuildSpec<ModuleDescriptions.Description.GetHandler>(x => x
                    .OnOrphanedModuleAction(OrphanedActions.UseDefault));

            var module = spec.modules[0];

            module.name.ShouldEqual("Some Module");
            module.comments.ShouldEqual("Some comments.");
        }

        [Test]
        public void should_set_description_and_text_embedded_resource_comments_when_specified()
        {
            var spec = BuildSpec<ModuleDescriptions.EmbeddedTextComments.GetHandler>(x => x
                    .OnOrphanedModuleAction(OrphanedActions.UseDefault));

            var module = spec.modules[0];

            module.name.ShouldEqual("Some Text Module");
            module.comments.ShouldEqual("<b>Some text comments</b>");
        }

        [Test]
        public void should_set_description_and_markdown_embedded_resource_comments_when_specified()
        {
            var spec = BuildSpec<ModuleDescriptions.EmbeddedMarkdownComments.GetHandler>(x => x
                    .OnOrphanedModuleAction(OrphanedActions.UseDefault));

            var module = spec.modules[0];

            module.name.ShouldEqual("Some Markdown Module");
            module.comments.ShouldEqual("<p><strong>Some markdown comments</strong></p>");
        }

        [Test]
        public void should_return_actions_in_root_resources_when_there_are_no_modules_defined()
        {
            var spec = BuildSpec<NoModules.GetHandler>(x => x
                    .OnOrphanedModuleAction(OrphanedActions.UseDefault));

            spec.modules.Count.ShouldEqual(0);
            spec.resources.Count.ShouldEqual(1);
            spec.resources[0].endpoints.Count.ShouldEqual(1);
        }

        [Test]
        public void should_return_actions_in_root_resources_when_there_are_is_one_empty_module_defined()
        {
            var spec = BuildSpec<OneEmptyModule.GetHandler>(x => x
                    .OnOrphanedModuleAction(OrphanedActions.UseDefault));

            spec.modules.Count.ShouldEqual(0);
            spec.resources.Count.ShouldEqual(1);
            spec.resources[0].endpoints.Count.ShouldEqual(1);
        }

        [Test]
        public void should_return_actions_in_root_resources_when_there_is_one_empty_module_defined_and_orphaned_actions()
        {
            var spec = BuildSpec<OneEmptyModuleAndOrphanedAction.GetHandler>(x => x
                    .OnOrphanedModuleAction(OrphanedActions.UseDefault));

            spec.modules.Count.ShouldEqual(0);
            spec.resources.Count.ShouldEqual(2);
            spec.resources[0].endpoints.Count.ShouldEqual(1);
            spec.resources[1].endpoints.Count.ShouldEqual(1);
        }
        
        [Test]
        public void should_automatically_add_orphaned_actions_to_empty_default_module()
        {
            var spec = BuildSpec<OneModuleAndOrphanedAction.GetHandler>(x => x
                    .OnOrphanedModuleAction(OrphanedActions.UseDefault));

            spec.modules.Count.ShouldEqual(2);
            spec.resources.Count.ShouldEqual(0);

            var module = spec.modules[0];
            module.name.ShouldBeNull();
            module.resources.Count.ShouldEqual(1);
            module.resources[0].endpoints.Count.ShouldEqual(1);
            module.resources[0].endpoints[0].url.ShouldEqual("/onemoduleandorphanedaction/orphan");

            module = spec.modules[1];
            module.name.ShouldEqual("Some Module");
            module.resources.Count.ShouldEqual(1);
            module.resources[0].endpoints.Count.ShouldEqual(1);
            module.resources[0].endpoints[0].url.ShouldEqual("/onemoduleandorphanedaction/somehandler/inmodule");
        }

        [Test]
        public void should_automatically_add_orphaned_actions_to_the_specified_default_module()
        {
            var spec = BuildSpec<OneModuleAndOrphanedAction.GetHandler>(x => x
                    .OnOrphanedModuleAction(OrphanedActions.UseDefault)
                    .WithDefaultModule(y => new ModuleDescription { Name = "Default Module" }));

            spec.modules.Count.ShouldEqual(2);
            spec.resources.Count.ShouldEqual(0);

            var module = spec.modules[0];
            module.name.ShouldEqual("Default Module");
            module.resources.Count.ShouldEqual(1);
            module.resources[0].endpoints.Count.ShouldEqual(1);
            module.resources[0].endpoints[0].url.ShouldEqual("/onemoduleandorphanedaction/orphan");

            module = spec.modules[1];
            module.name.ShouldEqual("Some Module");
            module.resources.Count.ShouldEqual(1);
            module.resources[0].endpoints.Count.ShouldEqual(1);
            module.resources[0].endpoints[0].url.ShouldEqual("/onemoduleandorphanedaction/somehandler/inmodule");
        }

        [Test]
        public void should_ignore_orphaned_actions()
        {
            var spec = BuildSpec<OneModuleAndOrphanedAction.GetHandler>(x => x
                    .OnOrphanedModuleAction(OrphanedActions.Exclude));

            spec.modules.Count.ShouldEqual(1);
            spec.resources.Count.ShouldEqual(0);

            var module = spec.modules[0];
            module.name.ShouldEqual("Some Module");
            module.resources.Count.ShouldEqual(1);
            module.resources[0].endpoints.Count.ShouldEqual(1);
            module.resources[0].endpoints[0].url.ShouldEqual("/onemoduleandorphanedaction/somehandler/inmodule");
        }

        [Test]
        public void should_throw_an_exception_for_orphaned_actions()
        {
            Assert.Throws<OrphanedModuleActionException>(() => BuildSpec<NoModules.GetHandler>(x => x
                    .OnOrphanedModuleAction(OrphanedActions.Fail)));
        }

        [Test]
        public void should_not_throw_an_exception_when_there_are_no_orphaned_actions()
        {
            Assert.DoesNotThrow(() => BuildSpec<OneEmptyModule.GetHandler>(x => x
                    .OnOrphanedModuleAction(OrphanedActions.Fail)));
        }
    }
}