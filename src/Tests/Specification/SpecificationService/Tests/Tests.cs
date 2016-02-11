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
        protected FubuMVC.Swank.Specification.Specification BuildSpec<TNamespace>(Action<Swank> configure = null)
        {
            var graph = Behavior.BuildGraph().AddActionsInNamespace(GetType());
            return BuildSpec<TNamespace>(graph, configure);
        }

        protected FubuMVC.Swank.Specification.Specification BuildSpec<TNamespace>(BehaviorGraph graph, Action<Swank> configure = null)
        {
            var configuration = Swank.CreateConfig(x =>
                {
                    if (configure != null) configure(x);

                    x.AppliesToThisAssembly()
                     .Where(y => y.FirstCall().HandlerType.InNamespace<TNamespace>());
                });

            var behaviorSource = new BehaviorSource(graph, configuration);
            var resourceConvention = new ResourceConvention(new MarkerConvention<ResourceDescription>(), behaviorSource);
            var moduleConvention = new ModuleConvention(new MarkerConvention<ModuleDescription>());
            var typeCache = new TypeDescriptorCache();
            var memberConvention = new MemberConvention();
            var optionFactory = new OptionFactory(configuration,
                new EnumConvention(), new OptionConvention());
            return new FubuMVC.Swank.Specification.SpecificationService(
                configuration,
                new BehaviorSource(graph, configuration),
                typeCache,
                moduleConvention,
                resourceConvention,
                new EndpointConvention(),
                memberConvention,
                new StatusCodeConvention(),
                new HeaderConvention(),
                new MimeTypeConvention(),
                new TypeGraphFactory(
                    configuration,
                    typeCache,
                    new TypeConvention(configuration),
                    memberConvention,
                    optionFactory),
                new BodyDescriptionFactory(configuration),
                new OptionFactory(configuration,
                    new EnumConvention(), 
                    new OptionConvention())).Generate();
        }
    }

    [TestFixture]
    public class Tests : InteractionContext
    {
        [Test]
        public void should_set_description_to_default_when_none_is_specified()
        {
            var spec = BuildSpec<Tests>(x => x
                .Named("Some API")
                .WithLogo("logo.png")
                .WithCopyright("Copyright Now"));

            spec.Name.ShouldEqual("Some API");
            spec.LogoUrl.ShouldEqual("logo.png");
            spec.Comments.ShouldEqual("<p><strong>Some markdown comments</strong></p>");
        }
    }
}