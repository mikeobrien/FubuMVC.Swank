using System.Linq;
using System.Reflection;
using FubuMVC.Core.Registration;
using FubuMVC.Swank;
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
            _graph.AddAction<ViewGetHandler>("GET");

            var actions = new BehaviorSource(_graph, new Configuration()).GetChains();

            actions.Count.ShouldEqual(4);
            actions.All(x => x.HandlerType.Assembly == Assembly.GetExecutingAssembly()).ShouldBeTrue();
        }

        [Test]
        public void should_only_enumerate_actions_in_the_specified_assemblies()
        {
            _graph.AddAction<ViewGetHandler>("GET");

            var configuration = Swank.CreateConfig(x => x.AppliesToThisAssembly());
            var actions = new BehaviorSource(_graph, configuration).GetChains();

            actions.Count.ShouldEqual(4);
            actions.All(x => x.HandlerType.Assembly == Assembly.GetExecutingAssembly()).ShouldBeTrue();
        }

        [Test]
        public void should_filter_actions_based_on_filter_in_the_configuration()
        {
            var configuration = Swank.CreateConfig(x => x
                .AppliesToThisAssembly()
                .Where(y => y.Route.Pattern.StartsWith("/handlers/widget")));

            var actions = new BehaviorSource(_graph, configuration).GetChains();

            actions.Count.ShouldEqual(2);
            actions.All(x => x.ParentChain().Route.Pattern.StartsWith("/handlers/widget")).ShouldBeTrue();
        }
    }
}