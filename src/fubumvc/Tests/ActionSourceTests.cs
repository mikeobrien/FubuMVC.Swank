using System.Linq;
using System.Reflection;
using FubuMVC.Core.Registration;
using NUnit.Framework;
using Should;
using Swank;

namespace Tests
{
    [TestFixture]
    public class ActionSourceTests
    {
        private BehaviorGraph _graph;

        [SetUp]
        public void Setup()
        {
            _graph = TestBehaviorGraph.Build();
        }

        [Test]
        public void should_enumerate_actions_in_all_assemblies_by_default()
        {
            _graph.AddAction<SpecificationHandler>("/documentation", HttpVerbs.Get);

            var actions = new Swank.ActionSource(_graph, new Configuration()).GetActions();

            actions.Count.ShouldEqual(37);
            actions.Count(x => x.HandlerType.Assembly == Assembly.GetExecutingAssembly()).ShouldEqual(36);
            actions.Count(x => x.HandlerType.Assembly == typeof(SpecificationHandler).Assembly).ShouldEqual(1);
        }

        [Test]
        public void should_only_enumerate_actions_in_the_specified_assemblies()
        {
            var configuration = ConfigurationDsl.CreateConfig(x => x.AppliesToThisAssembly());

            _graph.AddAction<SpecificationHandler>("/documentation", HttpVerbs.Get);

            var actions = new Swank.ActionSource(_graph, configuration).GetActions();

            actions.Count.ShouldEqual(36);
            actions.All(x => x.HandlerType.Assembly == Assembly.GetExecutingAssembly()).ShouldBeTrue();
        }

        [Test]
        public void should_filter_actions_based_on_filter_in_the_configuration()
        {
            var configuration = ConfigurationDsl.CreateConfig(x => x
                .AppliesToThisAssembly()
                .Where(y => y.ParentChain().Route.FirstPatternSegment() != "batches"));

            var actions = new Swank.ActionSource(_graph, configuration).GetActions();

            actions.Count.ShouldEqual(26);
            actions.All(x => x.ParentChain().Route.FirstPatternSegment() != "/batches/").ShouldBeTrue();
        }
    }
}