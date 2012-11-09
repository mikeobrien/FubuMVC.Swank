using System;
using FubuCore.Reflection;
using FubuMVC.Swank;
using FubuMVC.Swank.Description;
using FubuMVC.Swank.Specification;
using NUnit.Framework;
using Should;

namespace Tests.Specification.SpecificationServiceModuleTests
{
    [TestFixture]
    public class Tests
    {
        private FubuMVC.Swank.Specification.Specification BuildSpec<TNamespace>(Action<Swank> configure = null)
        {
            var graph = Behavior.BuildGraph().AddActionsInThisNamespace();
            var moduleConvention = new ModuleConvention(new MarkerConvention<ModuleDescription>());
            var resourceConvention = new ResourceConvention(
                new MarkerConvention<ResourceDescription>(),
                new ActionSource(graph, 
                    Swank.CreateConfig(x => x.AppliesToThisAssembly()
                        .Where(y => y.HandlerType.InNamespace<Tests>()))));
            var configuration = Swank.CreateConfig(x => 
            { if (configure != null) configure(x); x.AppliesToThisAssembly().Where(y => y.HandlerType.InNamespace<TNamespace>()); });
            return new SpecificationService(configuration, new ActionSource(graph, configuration), new TypeDescriptorCache(),
                moduleConvention, resourceConvention, new EndpointConvention(), new MemberConvention(), new OptionConvention(), new ErrorConvention(),
                new HeaderConvention(), new TypeConvention(), new MergeService()).Generate();
        }

        [Test]
        public void should_set_description_to_default_when_none_is_specified()
        {
            var spec = BuildSpec<ModuleDescriptions.NoDescription.GetHandler>();

            var module = spec.Modules[0];

            module.Name.ShouldBeNull();
            module.Comments.ShouldBeNull();
        }

        [Test]
        public void should_set_description_when_one_is_specified()
        {
            var spec = BuildSpec<ModuleDescriptions.Description.GetHandler>();

            var module = spec.Modules[0];

            module.Name.ShouldEqual("Some Module");
            module.Comments.ShouldEqual("Some comments.");
        }

        [Test]
        public void should_set_description_and_text_embedded_resource_comments_when_specified()
        {
            var spec = BuildSpec<ModuleDescriptions.EmbeddedTextComments.GetHandler>();

            var module = spec.Modules[0];

            module.Name.ShouldEqual("Some Text Module");
            module.Comments.ShouldEqual("<b>Some text comments</b>");
        }

        [Test]
        public void should_set_description_and_markdown_embedded_resource_comments_when_specified()
        {
            var spec = BuildSpec<ModuleDescriptions.EmbeddedMarkdownComments.GetHandler>();

            var module = spec.Modules[0];

            module.Name.ShouldEqual("Some Markdown Module");
            module.Comments.ShouldEqual("<p><strong>Some markdown comments</strong></p>");
        }

        [Test]
        public void should_add_actions_to_closest_parent_module()
        {
            var spec = BuildSpec<NestedModules.GetHandler>();

            spec.Modules.Count.ShouldEqual(2);
            spec.Resources.Count.ShouldEqual(0);

            var module = spec.Modules[0];
            module.Name.ShouldEqual("Nested Module");
            module.Resources.Count.ShouldEqual(1);
            module.Resources[0].Endpoints.Count.ShouldEqual(1);
            module.Resources[0].Endpoints[0].Url.ShouldEqual("/nestedmodules/nestedmodule");

            module = spec.Modules[1];
            module.Name.ShouldEqual("Root Module");
            module.Resources.Count.ShouldEqual(2);
            module.Resources[0].Endpoints.Count.ShouldEqual(1);
            module.Resources[0].Endpoints[0].Url.ShouldEqual("/nestedmodules");
            module.Resources[1].Endpoints.Count.ShouldEqual(1);
            module.Resources[1].Endpoints[0].Url.ShouldEqual("/nestedmodules/nomodule");
        }

        [Test]
        public void should_return_actions_in_root_resources_when_there_are_no_modules_defined()
        {
            var spec = BuildSpec<NoModules.GetHandler>();

            spec.Modules.Count.ShouldEqual(0);
            spec.Resources.Count.ShouldEqual(1);
            spec.Resources[0].Endpoints.Count.ShouldEqual(1);
        }

        [Test]
        public void should_automatically_add_orphaned_actions_to_root_resources_when_modules_are_defined()
        {
            var spec = BuildSpec<OneModuleAndOrphanedAction.GetHandler>();

            spec.Modules.Count.ShouldEqual(1);
            spec.Resources.Count.ShouldEqual(1);

            var module = spec.Modules[0];
            module.Name.ShouldEqual("Some Module");
            module.Resources.Count.ShouldEqual(1);
            module.Resources[0].Endpoints.Count.ShouldEqual(1);
            module.Resources[0].Endpoints[0].Url.ShouldEqual("/onemoduleandorphanedaction/withmodule/inmodule");

            var resource = spec.Resources[0];
            resource.Endpoints.Count.ShouldEqual(1);
            resource.Endpoints[0].Url.ShouldEqual("/onemoduleandorphanedaction/orphan");
        }

        [Test]
        public void should_automatically_add_orphaned_actions_to_the_specified_default_module()
        {
            var spec = BuildSpec<OneModuleAndOrphanedAction.GetHandler>(x => x
                    .WithDefaultModule(y => new ModuleDescription { Name = "Default Module" }));

            spec.Modules.Count.ShouldEqual(2);
            spec.Resources.Count.ShouldEqual(0);

            var module = spec.Modules[0];
            module.Name.ShouldEqual("Default Module");
            module.Resources.Count.ShouldEqual(1);
            module.Resources[0].Endpoints.Count.ShouldEqual(1);
            module.Resources[0].Endpoints[0].Url.ShouldEqual("/onemoduleandorphanedaction/orphan");

            module = spec.Modules[1];
            module.Name.ShouldEqual("Some Module");
            module.Resources.Count.ShouldEqual(1);
            module.Resources[0].Endpoints.Count.ShouldEqual(1);
            module.Resources[0].Endpoints[0].Url.ShouldEqual("/onemoduleandorphanedaction/withmodule/inmodule");
        }

        [Test]
        public void should_ignore_orphaned_actions()
        {
            var spec = BuildSpec<OneModuleAndOrphanedAction.GetHandler>(x => x
                    .OnOrphanedModuleAction(OrphanedActions.Exclude));

            spec.Modules.Count.ShouldEqual(1);
            spec.Resources.Count.ShouldEqual(0);

            var module = spec.Modules[0];
            module.Name.ShouldEqual("Some Module");
            module.Resources.Count.ShouldEqual(1);
            module.Resources[0].Endpoints.Count.ShouldEqual(1);
            module.Resources[0].Endpoints[0].Url.ShouldEqual("/onemoduleandorphanedaction/withmodule/inmodule");
        }

        [Test]
        public void should_throw_an_exception_for_orphaned_actions()
        {
            NUnit.Framework.Assert.Throws<OrphanedModuleActionException>(() => BuildSpec<NoModules.GetHandler>(x => x
                    .OnOrphanedModuleAction(OrphanedActions.Fail)));
        }

        [Test]
        public void should_not_throw_an_exception_when_there_are_no_orphaned_actions()
        {
            NUnit.Framework.Assert.DoesNotThrow(() => BuildSpec<ModuleDescriptions.NoDescription.GetHandler>(x => x
                    .OnOrphanedModuleAction(OrphanedActions.Fail)));
        }
    }
}