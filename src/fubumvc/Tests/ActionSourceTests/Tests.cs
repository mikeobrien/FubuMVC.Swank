using System.Linq;
using System.Reflection;
using FubuMVC.Core.Registration;
using NUnit.Framework;
using Should;
using Swank;
using Tests.ActionSourceTests.Administration;
using Tests.ActionSourceTests.Administration.Users;
using Tests.ActionSourceTests.Batches.Cells;
using Tests.ActionSourceTests.Batches.Schedules;
using Tests.ActionSourceTests.Templates;

namespace Tests.ActionSourceTests
{
    [TestFixture]
    public class Tests
    {
        private BehaviorGraph _graph;

        [SetUp]
        public void Setup()
        {
            _graph = Behaviors.BuildGraph()
                .AddAction<TemplatePutHandler>("/templates", HttpVerbs.Put)
                .AddAction<AdminAccountGetAllHandler>("/admin", HttpVerbs.Get)
                .AddAction<AdminUserGetAllHandler>("/admin/users", HttpVerbs.Get)
                .AddAction<AdminAddressGetAllOfTypeHandler>("/admin/users/addresses", HttpVerbs.Get)
                .AddAction<BatchCellGetAllHandler>("/batches/cells", HttpVerbs.Get)
                .AddAction<BatchScheduleGetAllHandler>("/batches/schedules", HttpVerbs.Get);
        }

        [Test]
        public void should_enumerate_actions_in_all_assemblies_by_default()
        {
            _graph.AddAction<SpecificationHandler>("/documentation", HttpVerbs.Get);

            var actions = new Swank.ActionSource(_graph, new Configuration()).GetActions();

            actions.Count.ShouldEqual(7);
            actions.Count(x => x.HandlerType.Assembly == Assembly.GetExecutingAssembly()).ShouldEqual(6);
            actions.Count(x => x.HandlerType.Assembly == typeof(SpecificationHandler).Assembly).ShouldEqual(1);
        }

        [Test]
        public void should_only_enumerate_actions_in_the_specified_assemblies()
        {
            var configuration = ConfigurationDsl.CreateConfig(x => x.AppliesToThisAssembly());

            _graph.AddAction<SpecificationHandler>("/documentation", HttpVerbs.Get);

            var actions = new Swank.ActionSource(_graph, configuration).GetActions();

            actions.Count.ShouldEqual(6);
            actions.All(x => x.HandlerType.Assembly == Assembly.GetExecutingAssembly()).ShouldBeTrue();
        }

        [Test]
        public void should_filter_actions_based_on_filter_in_the_configuration()
        {
            var configuration = ConfigurationDsl.CreateConfig(x => x
                .AppliesToThisAssembly()
                .Where(y => y.ParentChain().Route.FirstPatternSegment() != "batches"));

            var actions = new Swank.ActionSource(_graph, configuration).GetActions();

            actions.Count.ShouldEqual(4);
            actions.All(x => x.ParentChain().Route.FirstPatternSegment() != "/batches/").ShouldBeTrue();
        }
    }
}