using System.Net;
using FubuMVC.Core.Registration;
using FubuMVC.Swank.Description;
using NUnit.Framework;
using Should;

namespace Tests.Description
{
    [TestFixture]
    public class HeaderConventionTests
    {
        private BehaviorGraph _graph;
        private HeaderConvention _headerConvention;

        [SetUp]
        public void Setup()
        {
            _graph = Behavior.BuildGraph().AddActionsInThisNamespace();
            _headerConvention = new HeaderConvention();
        }

        [HeaderDescription(HttpHeaderType.Response, "content-type")]
        [HeaderDescription(HttpHeaderType.Request, "api-key", "This is a handler description.", true)]
        public class HeadersGetHandler
        {
            [HeaderDescription(HttpHeaderType.Request, "accept", "This is an endpoint description.", true)]
            [HeaderDescription(HttpHeaderType.Response, "content-length")]
            public object Execute_Headers(object request) { return null; }
        }

        [Test]
        public void should_set_endpoint_headers_on_handlers_and_actions()
        {
            var action = _graph.GetAction<HeadersGetHandler>();
            var headerDescriptions = _headerConvention.GetDescription(action);

            headerDescriptions.Count.ShouldEqual(4);

            var header = headerDescriptions[0];
            header.Type.ShouldEqual(HttpHeaderType.Request);
            header.Name.ShouldEqual("accept");
            header.Comments.ShouldEqual("This is an endpoint description.");
            header.Optional.ShouldBeTrue();

            header = headerDescriptions[1];
            header.Type.ShouldEqual(HttpHeaderType.Request);
            header.Name.ShouldEqual("api-key");
            header.Comments.ShouldEqual("This is a handler description.");
            header.Optional.ShouldBeTrue();

            header = headerDescriptions[2];
            header.Type.ShouldEqual(HttpHeaderType.Response);
            header.Name.ShouldEqual("content-length");
            header.Comments.ShouldBeNull();
            header.Optional.ShouldBeFalse();

            header = headerDescriptions[3];
            header.Type.ShouldEqual(HttpHeaderType.Response);
            header.Name.ShouldEqual("content-type");
            header.Comments.ShouldBeNull();
            header.Optional.ShouldBeFalse();
        }

        public class NoHeadersGetHandler
        {
            public object Execute_NoHeaders(object request) { return null; }
        }

        [Test]
        public void should_not_set_endpoint_headers_when_none_are_set_on_handlers_or_actions()
        {
            var action = _graph.GetAction<NoHeadersGetHandler>();
            var headerDescriptions = _headerConvention.GetDescription(action);

            headerDescriptions.Count.ShouldEqual(0);
        }
    }
}