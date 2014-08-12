using System;
using FubuCore.Reflection;
using FubuMVC.Core.Registration;
using FubuMVC.Swank;
using FubuMVC.Swank.Description;
using FubuMVC.Swank.Specification;
using NUnit.Framework;
using Should;

namespace Tests.Specification.SpecificationService.Tests
{
    [TestFixture]
    public abstract class InteractionContext
    {
        protected FubuMVC.Swank.Specification.Specification BuildSpec<TNamespace>(Action<Swank> configure = null, string specFile = null)
        {
            var graph = Behavior.BuildGraph().AddActionsInNamespace(GetType());
            return BuildSpec<TNamespace>(graph, configure, specFile);
        }

        protected FubuMVC.Swank.Specification.Specification BuildSpec<TNamespace>(BehaviorGraph graph, Action<Swank> configure = null, string specFile = null)
        {
            var configuration = Swank.CreateConfig(x =>
                {
                    if (configure != null) configure(x);

                    x.AppliesToThisAssembly()
                     .Where(y => y.FirstCall().HandlerType.InNamespace<TNamespace>());

                    if (specFile != null)
                    {
                        x.MergeThisSpecification(specFile);
                    }
                });

            var behaviorSource = new BehaviorSource(graph, configuration);
            var resourceConvention = new ResourceConvention(new MarkerConvention<ResourceDescription>(), behaviorSource);
            var moduleConvention = new ModuleConvention(new MarkerConvention<ModuleDescription>());

            return new FubuMVC.Swank.Specification.SpecificationService(configuration,
                                                                        behaviorSource,
                                                                        new TypeDescriptorCache(),
                                                                        moduleConvention,
                                                                        resourceConvention,
                                                                        new EndpointConvention(),
                                                                        new MemberConvention(),
                                                                        new OptionConvention(),
                                                                        new StatusCodeConvention(),
                                                                        new HeaderConvention(),
                                                                        new TypeConvention()).Generate();
        
        }
    }

    [TestFixture]
    public class Tests : InteractionContext
    {
        [Test]
        public void should_set_description_to_default_when_none_is_specified()
        {
            var spec = BuildSpec<Tests>(x => x.Named("Some API").WithCopyright("Copyright Now"));

            spec.Name.ShouldEqual("Some API");
            spec.Comments.ShouldEqual("<p><strong>Some markdown comments</strong></p>");
        }
    }
}