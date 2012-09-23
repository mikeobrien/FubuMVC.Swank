using System.Linq;
using System.Reflection;
using FubuMVC.Core.Registration;
using FubuMVC.Swank;
using NUnit.Framework;
using Should;
using ActionSource = FubuMVC.Swank.ActionSource;

namespace Tests.ActionSourceTests
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
        public void should_enumerate_actions_in_all_assemblies_by_default()
        {
            _graph.AddAction<SpecificationHandler>("GET");

            var actions = new ActionSource(_graph, new Configuration()).GetActions();

            actions.Count.ShouldEqual(5);
            actions.Count(x => x.HandlerType.Assembly == Assembly.GetExecutingAssembly()).ShouldEqual(4);
            actions.Count(x => x.HandlerType.Assembly == typeof(SpecificationHandler).Assembly).ShouldEqual(1);
        }

        [Test]
        public void should_only_enumerate_actions_in_the_specified_assemblies()
        {
            _graph.AddAction<SpecificationHandler>("GET");

            var configuration = ConfigurationDsl.CreateConfig(x => x.AppliesToThisAssembly());
            var actions = new ActionSource(_graph, configuration).GetActions();

            actions.Count.ShouldEqual(4);
            actions.All(x => x.HandlerType.Assembly == Assembly.GetExecutingAssembly()).ShouldBeTrue();
        }

        [Test]
        public void should_filter_actions_based_on_filter_in_the_configuration()
        {
            var configuration = ConfigurationDsl.CreateConfig(x => x
                .AppliesToThisAssembly()
                .Where(y => y.ParentChain().Route.Pattern.StartsWith("/handlers/widget")));

            var actions = new ActionSource(_graph, configuration).GetActions();

            actions.Count.ShouldEqual(2);
            actions.All(x => x.ParentChain().Route.Pattern.StartsWith("/handlers/widget")).ShouldBeTrue();
        }
    }
}