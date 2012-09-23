using System.Net;
using FubuMVC.Core.Registration;
using FubuMVC.Swank.Description;
using NUnit.Framework;
using Should;

namespace Tests.Description
{
    [TestFixture]
    public class ErrorSourceTests
    {
        private BehaviorGraph _graph;
        private ErrorSource _errorSource;

        [SetUp]
        public void Setup()
        {
            _graph = Behavior.BuildGraph().AddActionsInThisNamespace();
            _errorSource = new ErrorSource();
        }

        [ErrorDescription(HttpStatusCode.LengthRequired, "411 error on handler")]
        [ErrorDescription(410, "410 error on handler", "410 error on action description")]
        public class ErrorsGetHandler
        {
            [ErrorDescription(HttpStatusCode.RequestEntityTooLarge, "413 error on action")]
            [ErrorDescription(412, "412 error on action", "412 error on action description")]
            public object Execute_Errors(object request) { return null; }
        }

        [Test]
        public void should_set_endpoint_errors_on_handlers_and_actions()
        {
            var action = _graph.GetAction<ErrorsGetHandler>();
            var errorDescriptions = _errorSource.GetDescription(action);

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

        public class NoErrorsGetHandler
        {
            public object Execute_NoErrors(object request) { return null; }
        }

        [Test]
        public void should_not_set_endpoint_errors_when_none_are_set_on_handlers_or_actions()
        {
            var action = _graph.GetAction<NoErrorsGetHandler>();
            var errorDescriptions = _errorSource.GetDescription(action);

            errorDescriptions.Count.ShouldEqual(0);
        }
    }
}