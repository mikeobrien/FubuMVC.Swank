using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Swank.Specification;
using NUnit.Framework;
using Tests.Specification.SpecificationService.Tests;
using Tests.Specification.SpecificationService.MultiActionTests.Handlers;
using Should;

namespace Tests.Specification.SpecificationService.MultiActionTests
{
    public class Tests : InteractionContext
    {
        private DataDescription _request;
        private DataDescription _response;

        [SetUp]
        public void SetUp()
        {
            var graph = Behavior.BuildGraph();

            graph.AddAction<PostHandler>();
            var chain = graph.GetAction<PostHandler>().ParentChain();

            var secondAction = new ActionCall(typeof(Handlers.Widgets.PostHandler), ReflectionHelper.GetMethod<Handlers.Widgets.PostHandler>(x => x.Execute_Id(null)));

            chain.AddToEnd(secondAction);

            var spec = BuildSpec<PostHandler>(graph);

            var endpoint = spec.Modules[0].Resources[0].Endpoints[0];
            _request = endpoint.Request.Body[0];
            _response = endpoint.Response.Body[0];
        }

        [Test]
        public void should_register_input_type_from_first_action()
        {
            _request.Name.ShouldEqual("Request");
        }

        [Test]
        public void should_register_output_type_from_last_action()
        {
            _response.Name.ShouldEqual("Data");
        }
    }
}