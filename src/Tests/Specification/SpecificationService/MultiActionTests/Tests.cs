using System;
using FubuCore.Reflection;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using NUnit.Framework;
using Tests.Specification.SpecificationService.Tests;
using Tests.Specification.SpecificationService.MultiActionTests.Handlers;
using Should;

namespace Tests.Specification.SpecificationService.MultiActionTests
{
    public class Tests : InteractionContext
    {
        private BehaviorGraph _graph;
        private FubuMVC.Swank.Specification.Specification _spec;

        [SetUp]
        public void SetUp()
        {
            _graph = Behavior.BuildGraph();

            _graph.AddAction<PostHandler>();
            var chain = _graph.GetAction<PostHandler>().ParentChain();

            var secondAction = new ActionCall(typeof(Handlers.Widgets.PostHandler), ReflectionHelper.GetMethod<Handlers.Widgets.PostHandler>(x => x.Execute_Id(null)));

            chain.AddToEnd(secondAction);

            _spec = BuildSpec<PostHandler>(_graph);
        }

        [Test]
        public void should_only_register_input_and_output_types()
        {
            _spec.Types.Count.ShouldEqual(2);
        }

        [Test]
        public void should_register_input_type_from_frist_action()
        {
            throw new NotImplementedException();
            //_spec.Types.ShouldContainOneInputType<Request, PostHandler>();
        }

        [Test]
        public void should_not_register_input_from_last_action()
        {
            throw new NotImplementedException();
            //_spec.Types.ShouldNotContainAnyInputType<Handlers.Widgets.UnregisteredInput, Handlers.Widgets.PostHandler>();
        }

        [Test]
        public void should_register_output_type_from_last_action()
        {
            throw new NotImplementedException();
            //_spec.Types.ShouldContainOneOutputType<Handlers.Widgets.Data>();
        }

        [Test]
        public void should_not_register_output_type_from_first_action()
        {
            throw new NotImplementedException();
            //_spec.Types.ShouldNotContainAnyOutputTypes<UnregisterdOutput>();
        }
    }
}