using System;
using FubuCore.Reflection;
using FubuMVC.Swank;
using FubuMVC.Swank.Description;
using FubuMVC.Swank.Specification;
using NUnit.Framework;
using Should;

namespace Tests.Specification.SpecificationServiceTests
{
    [TestFixture]
    public class Tests
    {
        private FubuMVC.Swank.Specification.Specification BuildSpec(Action<Swank> configure = null)
        {
            var graph = Behavior.BuildGraph().AddActionsInThisNamespace();
            var moduleSource = new ModuleSource(new MarkerSource<ModuleDescription>());
            var resourceSource = new ResourceSource(
                new MarkerSource<ResourceDescription>(),
                new ActionSource(graph,Swank.CreateConfig(x => x.AppliesToThisAssembly().Where(y => y.HandlerType.InNamespace<Tests>()))));
            var configuration = Swank.CreateConfig(x => 
            { if (configure != null) configure(x); x.AppliesToThisAssembly().Where(y => y.HandlerType.InNamespace<Tests>()); });
            return new SpecificationService(configuration, new ActionSource(graph, configuration), new TypeDescriptorCache(),
                moduleSource, resourceSource, new EndpointSource(), new MemberSource(), new OptionSource(), new ErrorSource(), 
                new TypeSource(), new MergeService()).Generate();
        }

        [Test]
        public void should_set_description_to_default_when_none_is_specified()
        {
            var spec = BuildSpec(x => x.Named("Some API").WithCopyright("Copyright Now"));

            spec.Name.ShouldEqual("Some API");
            spec.Copyright.ShouldEqual("Copyright Now");
            spec.Comments.ShouldEqual("<p><strong>Some markdown comments</strong></p>");
        }
    }
}