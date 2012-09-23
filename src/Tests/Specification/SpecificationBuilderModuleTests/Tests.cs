using System;
using FubuCore.Reflection;
using FubuMVC.Swank;
using FubuMVC.Swank.Description;
using FubuMVC.Swank.Specification;
using NUnit.Framework;
using Should;

namespace Tests.Specification.SpecificationBuilderModuleTests
{
    [TestFixture]
    public class Tests
    {
        private FubuMVC.Swank.Specification.Specification BuildSpec<TNamespace>(Action<Swank> configure = null)
        {
            var graph = Behavior.BuildGraph().AddActionsInThisNamespace();
            var moduleSource = new ModuleSource(new MarkerSource<ModuleDescription>());
            var resourceSource = new ResourceSource(
                new MarkerSource<ResourceDescription>(),
                new ActionSource(graph, 
                    Swank.CreateConfig(x => x.AppliesToThisAssembly()
                        .Where(y => y.HandlerType.InNamespace<Tests>()))));
            var configuration = Swank.CreateConfig(x => 
            { if (configure != null) configure(x); x.AppliesToThisAssembly().Where(y => y.HandlerType.InNamespace<TNamespace>()); });
            return new SpecificationBuilder(configuration, new ActionSource(graph, configuration), new TypeDescriptorCache(),
                moduleSource, resourceSource, new EndpointSource(), new MemberSource(), new OptionSource(), new ErrorSource(), new TypeSource()).Build();
        }

        [Test]
        public void should_set_description_to_default_when_none_is_specified()
        {
            var spec = BuildSpec<ModuleDescriptions.NoDescription.GetHandler>();

            var module = spec.modules[0];

            module.name.ShouldBeNull();
            module.comments.ShouldBeNull();
        }

        [Test]
        public void should_set_description_when_one_is_specified()
        {
            var spec = BuildSpec<ModuleDescriptions.Description.GetHandler>();

            var module = spec.modules[0];

            module.name.ShouldEqual("Some Module");
            module.comments.ShouldEqual("Some comments.");
        }

        [Test]
        public void should_set_description_and_text_embedded_resource_comments_when_specified()
        {
            var spec = BuildSpec<ModuleDescriptions.EmbeddedTextComments.GetHandler>();

            var module = spec.modules[0];

            module.name.ShouldEqual("Some Text Module");
            module.comments.ShouldEqual("<b>Some text comments</b>");
        }

        [Test]
        public void should_set_description_and_markdown_embedded_resource_comments_when_specified()
        {
            var spec = BuildSpec<ModuleDescriptions.EmbeddedMarkdownComments.GetHandler>();

            var module = spec.modules[0];

            module.name.ShouldEqual("Some Markdown Module");
            module.comments.ShouldEqual("<p><strong>Some markdown comments</strong></p>");
        }

        [Test]
        public void should_add_actions_to_closest_parent_module()
        {
            var spec = BuildSpec<NestedModules.GetHandler>();

            spec.modules.Count.ShouldEqual(2);
            spec.resources.Count.ShouldEqual(0);

            var module = spec.modules[0];
            module.name.ShouldEqual("Nested Module");
            module.resources.Count.ShouldEqual(1);
            module.resources[0].endpoints.Count.ShouldEqual(1);
            module.resources[0].endpoints[0].url.ShouldEqual("/nestedmodules/nestedmodule");

            module = spec.modules[1];
            module.name.ShouldEqual("Root Module");
            module.resources.Count.ShouldEqual(2);
            module.resources[0].endpoints.Count.ShouldEqual(1);
            module.resources[0].endpoints[0].url.ShouldEqual("/nestedmodules");
            module.resources[1].endpoints.Count.ShouldEqual(1);
            module.resources[1].endpoints[0].url.ShouldEqual("/nestedmodules/nomodule");
        }

        [Test]
        public void should_return_actions_in_root_resources_when_there_are_no_modules_defined()
        {
            var spec = BuildSpec<NoModules.GetHandler>();

            spec.modules.Count.ShouldEqual(0);
            spec.resources.Count.ShouldEqual(1);
            spec.resources[0].endpoints.Count.ShouldEqual(1);
        }

        [Test]
        public void should_automatically_add_orphaned_actions_to_root_resources_when_modules_are_defined()
        {
            var spec = BuildSpec<OneModuleAndOrphanedAction.GetHandler>();

            spec.modules.Count.ShouldEqual(1);
            spec.resources.Count.ShouldEqual(1);

            var module = spec.modules[0];
            module.name.ShouldEqual("Some Module");
            module.resources.Count.ShouldEqual(1);
            module.resources[0].endpoints.Count.ShouldEqual(1);
            module.resources[0].endpoints[0].url.ShouldEqual("/onemoduleandorphanedaction/withmodule/inmodule");

            var resource = spec.resources[0];
            resource.endpoints.Count.ShouldEqual(1);
            resource.endpoints[0].url.ShouldEqual("/onemoduleandorphanedaction/orphan");
        }

        [Test]
        public void should_automatically_add_orphaned_actions_to_the_specified_default_module()
        {
            var spec = BuildSpec<OneModuleAndOrphanedAction.GetHandler>(x => x
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
            module.resources[0].endpoints[0].url.ShouldEqual("/onemoduleandorphanedaction/withmodule/inmodule");
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
            module.resources[0].endpoints[0].url.ShouldEqual("/onemoduleandorphanedaction/withmodule/inmodule");
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
            Assert.DoesNotThrow(() => BuildSpec<ModuleDescriptions.NoDescription.GetHandler>(x => x
                    .OnOrphanedModuleAction(OrphanedActions.Fail)));
        }
    }
}