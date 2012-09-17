using FubuMVC.Core.Registration;
using NUnit.Framework;
using Should;
using Swank.Description;

namespace Tests.Description.ErrorSourceTests
{
    [TestFixture]
    public class Tests
    {
        private BehaviorGraph _graph;

        [SetUp]
        public void Setup()
        {
            _graph = Behaviors.BuildGraph().AddActionsInThisNamespace();
        }

        [Test]
        public void should_set_endpoint_errors_on_handlers_and_actions()
        {
            var action = _graph.GetAction<ErrorsTests.ErrorsGetHandler>();
            var errorSource = new ErrorSource();
            var errorDescriptions = errorSource.GetDescription(action);

            errorDescriptions.Count.ShouldEqual(4);

            var error = errorDescriptions[0];
            error.Status.ShouldEqual(410);
            error.Name.ShouldEqual("410 error on handler");
            error.Comments.ShouldEqual("410 error on action description");

            error = errorDescriptions[1];
            error.Status.ShouldEqual(411);
            error.Name.ShouldEqual("411 error on handler");
            error.Comments.ShouldBeNull();

            error = errorDescriptions[2];
            error.Status.ShouldEqual(412);
            error.Name.ShouldEqual("412 error on action");
            error.Comments.ShouldEqual("412 error on action description");

            error = errorDescriptions[3];
            error.Status.ShouldEqual(413);
            error.Name.ShouldEqual("413 error on action");
            error.Comments.ShouldBeNull();
        }

        [Test]
        public void should_not_set_endpoint_errors_when_none_are_set_on_handlers_or_actions()
        {
            var action = _graph.GetAction<ErrorsTests.NoErrorsGetHandler>();
            var errorSource = new ErrorSource();
            var errorDescriptions = errorSource.GetDescription(action);

            errorDescriptions.Count.ShouldEqual(0);
        }
    }
}