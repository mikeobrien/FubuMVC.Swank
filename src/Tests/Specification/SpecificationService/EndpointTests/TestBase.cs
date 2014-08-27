using System;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Swank;
using FubuMVC.Swank.Description;
using FubuMVC.Swank.Specification;
using NUnit.Framework;

namespace Tests.Specification.SpecificationService.EndpointTests
{
    [TestFixture]
    public abstract class TestBase
    {
        protected FubuMVC.Swank.Specification.Specification Spec;

        private static readonly Func<BehaviorChain, bool> ActionFilter = x => x.FirstCall().HandlerType.InNamespace<TestBase>();

        [SetUp]
        public void Setup()
        {
            Spec = GetSpec();
        }

        public static FubuMVC.Swank.Specification.Specification GetSpec(Action<Swank> configure = null)
        {
            var graph = Behavior.BuildGraph().AddActionsInThisNamespace();
            var moduleConvention = new ModuleConvention(new MarkerConvention<ModuleDescription>());

            var resourceConvention = new ResourceConvention(
                new MarkerConvention<ResourceDescription>(),
                new BehaviorSource(graph, Swank.CreateConfig(x => x.AppliesToThisAssembly().Where(ActionFilter))));

            var configuration = Swank.CreateConfig(x =>
                {
                    x.AppliesToThisAssembly().Where(ActionFilter).WithEnumValueTypeOf(EnumValue.AsString);
                    if (configure != null) configure(x);
                });
            var typeCache = new TypeDescriptorCache();
            var memberConvention = new MemberConvention();
            var optionFactory = new OptionFactory(configuration, new OptionConvention());
            var specBuilder = new FubuMVC.Swank.Specification.SpecificationService(
                configuration,
                new BehaviorSource(graph, configuration),
                typeCache,
                moduleConvention,
                resourceConvention,
                new EndpointConvention(),
                memberConvention,
                new StatusCodeConvention(),
                new HeaderConvention(),
                new TypeGraphFactory(
                    configuration, 
                    typeCache,
                    new TypeConvention(configuration), 
                    memberConvention,
                    optionFactory), 
                new DataDescriptionFactory(configuration), 
                new OptionFactory(configuration, new OptionConvention()));
            return specBuilder.Generate();
        }
    }
}