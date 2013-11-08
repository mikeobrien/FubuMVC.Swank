using System;
using FubuCore.Reflection;
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
        protected FubuMVC.Swank.Specification.Specification BuildSpec<TNamespace>(Action<Swank> configure = null, System.Type rootType = null, string specFile = null)
        {
            //this could be tricky
            var graph = rootType == null ? Behavior.BuildGraph().AddActionsInThisNamespace() : Behavior.BuildGraph().AddActionsInNamespace(rootType);
            var moduleConvention = new ModuleConvention(new MarkerConvention<ModuleDescription>());

            var swankConfig = Swank.CreateConfig(x =>
                                       x.AppliesToThisAssembly()
                                        .Where(b => b.FirstCall().HandlerType.InNamespace<Tests>()));

            var resourceConvention = new ResourceConvention(new MarkerConvention<ResourceDescription>(), new BehaviorSource(graph, swankConfig));

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

            return new FubuMVC.Swank.Specification.SpecificationService(configuration, new BehaviorSource(graph, configuration), new TypeDescriptorCache(),
                moduleConvention, resourceConvention, new EndpointConvention(), new MemberConvention(), new OptionConvention(), new StatusCodeConvention(),
                new HeaderConvention(), new TypeConvention(), new MergeService()).Generate();

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