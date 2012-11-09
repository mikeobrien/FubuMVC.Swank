using System.Net;
using FubuMVC.Core.Registration;
using FubuMVC.Swank.Description;
using NUnit.Framework;
using Should;

namespace Tests.Description
{
    [TestFixture]
    public class StatusCodeConventionTests
    {
        private BehaviorGraph _graph;
        private StatusCodeConvention _statusCodeConvention;

        [SetUp]
        public void Setup()
        {
            _graph = Behavior.BuildGraph().AddActionsInThisNamespace();
            _statusCodeConvention = new StatusCodeConvention();
        }

        [StatusCodeDescription(HttpStatusCode.LengthRequired, "411 error on handler")]
        [StatusCodeDescription(410, "410 error on handler", "410 error on action description")]
        public class StatusGetHandler
        {
            [StatusCodeDescription(HttpStatusCode.RequestEntityTooLarge, "413 error on action")]
            [StatusCodeDescription(412, "412 error on action", "412 error on action description")]
            public object Execute_StatusCodes(object request) { return null; }
        }

        [Test]
        public void should_set_endpoint_status_codes_on_handlers_and_actions()
        {
            var action = _graph.GetAction<StatusGetHandler>();
            var descriptions = _statusCodeConvention.GetDescription(action);

            descriptions.Count.ShouldEqual(4);

            var statusCode = descriptions[0];
            statusCode.Code.ShouldEqual(410);
            statusCode.Name.ShouldEqual("410 error on handler");
            statusCode.Comments.ShouldEqual("410 error on action description");

            statusCode = descriptions[1];
            statusCode.Code.ShouldEqual(411);
            statusCode.Name.ShouldEqual("411 error on handler");
            statusCode.Comments.ShouldBeNull();

            statusCode = descriptions[2];
            statusCode.Code.ShouldEqual(412);
            statusCode.Name.ShouldEqual("412 error on action");
            statusCode.Comments.ShouldEqual("412 error on action description");

            statusCode = descriptions[3];
            statusCode.Code.ShouldEqual(413);
            statusCode.Name.ShouldEqual("413 error on action");
            statusCode.Comments.ShouldBeNull();
        }

        public class NoStatusCodesGetHandler
        {
            public object Execute_NoStatusCodes(object request) { return null; }
        }

        [Test]
        public void should_not_set_endpoint_status_codes_when_none_are_set_on_handlers_or_actions()
        {
            var action = _graph.GetAction<NoStatusCodesGetHandler>();
            _statusCodeConvention.GetDescription(action).Count.ShouldEqual(0);
        }
    }
}