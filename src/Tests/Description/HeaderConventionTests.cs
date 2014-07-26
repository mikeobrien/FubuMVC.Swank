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

        [Header(HttpDirection.Response, "content-type")]
        [Header(HttpDirection.Request, "api-key", "This is a handler description.", true)]
        public class HeadersGetHandler
        {
            [Header(HttpDirection.Request, "accept", "This is an endpoint description.", true)]
            [Header(HttpDirection.Response, "content-length")]
            public object Execute_Headers(object request) { return null; }
        }

        [Test]
        public void should_set_endpoint_headers_on_handlers_and_actions()
        {
            var action = _graph.GetAction<HeadersGetHandler>();
            var headerDescriptions = _headerConvention.GetDescription(action.ParentChain());

            headerDescriptions.Count.ShouldEqual(4);

            var header = headerDescriptions[0];
            header.Direction.ShouldEqual(HttpDirection.Request);
            header.Name.ShouldEqual("accept");
            header.Comments.ShouldEqual("This is an endpoint description.");
            header.Optional.ShouldBeTrue();

            header = headerDescriptions[1];
            header.Direction.ShouldEqual(HttpDirection.Request);
            header.Name.ShouldEqual("api-key");
            header.Comments.ShouldEqual("This is a handler description.");
            header.Optional.ShouldBeTrue();

            header = headerDescriptions[2];
            header.Direction.ShouldEqual(HttpDirection.Response);
            header.Name.ShouldEqual("content-length");
            header.Comments.ShouldBeNull();
            header.Optional.ShouldBeFalse();

            header = headerDescriptions[3];
            header.Direction.ShouldEqual(HttpDirection.Response);
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
            _headerConvention.GetDescription(_graph.GetAction<NoHeadersGetHandler>().ParentChain())
                .Count.ShouldEqual(0);
        }
    }
}