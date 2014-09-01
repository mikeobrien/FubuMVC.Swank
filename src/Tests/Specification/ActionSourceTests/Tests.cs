using System.Linq;
using System.Reflection;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Swank;
using FubuMVC.Swank.Documentation;
using FubuMVC.Swank.Specification;
using NUnit.Framework;
using Should;

namespace Tests.Specification.ActionSourceTests
{
    [TestFixture]
    public class Tests
    {
        private BehaviorGraph _graph;

        [SetUp]
        public void Setup()
        {
            _graph = Behavior.BuildGraph().AddActionsInThisNamespace();
        }

        [Test]
        public void should_enumerate_actions_in_all_assemblies_except_the_swank_assembly_by_default()
        {
            _graph.AddAction<GetHandler>("GET");

            var chains = new BehaviorSource(_graph, new Configuration()).GetChains();

            chains.Count.ShouldEqual(4);
            chains.All(x => x.FirstCall().HandlerType.Assembly == Assembly.GetExecutingAssembly()).ShouldBeTrue();
        }

        [Test]
        public void should_only_enumerate_actions_in_the_specified_assemblies()
        {
            _graph.AddAction<GetHandler>("GET");

            var configuration = Swank.CreateConfig(x => x.AppliesToThisAssembly());
            var chains = new BehaviorSource(_graph, configuration).GetChains();

            chains.Count.ShouldEqual(4);
            chains.All(x => x.FirstCall().HandlerType.Assembly == Assembly.GetExecutingAssembly()).ShouldBeTrue();
        }

        [Test]
        public void should_filter_actions_based_on_filter_in_the_configuration()
        {
            var configuration = Swank.CreateConfig(x => x
                .AppliesToThisAssembly()
                .Where(y => y.Route.Pattern.StartsWith("/handlers/widget")));

            var chains = new BehaviorSource(_graph, configuration).GetChains();

            chains.Count.ShouldEqual(2);
            chains.All(x => x.Route.Pattern.StartsWith("/handlers/widget")).ShouldBeTrue();
        }
    }
}