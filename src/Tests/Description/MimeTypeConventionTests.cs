using FubuMVC.Core.Registration;
using FubuMVC.Swank.Description;
using FubuMVC.Swank.Net;
using NUnit.Framework;
using Should;

namespace Tests.Description
{
    [TestFixture]
    public class MimeTypeConventionTests
    {
        private BehaviorGraph _graph;
        private MimeTypeConvention _mimeTypeConvention;

        [SetUp]
        public void Setup()
        {
            _graph = Behavior.BuildGraph().AddActionsInThisNamespace();
            _mimeTypeConvention = new MimeTypeConvention();
        }

        [MimeType(HttpDirection.Response, MimeType.ApplicationJson)]
        [MimeType(HttpDirection.Request, MimeType.ApplicationXml)]
        public class MimeTypesGetHandler
        {
            [MimeType(HttpDirection.Request, "text/json")]
            [MimeType(HttpDirection.Response, "text/xml")]
            public object Execute_Headers(object request) { return null; }
        }

        [Test]
        public void should_set_endpoint_mime_types_on_handlers_and_actions()
        {
            var action = _graph.GetAction<MimeTypesGetHandler>();
            var mimeTypes = _mimeTypeConvention.GetDescription(action.ParentChain());

            mimeTypes.Count.ShouldEqual(4);

            var header = mimeTypes[0];
            header.Direction.ShouldEqual(HttpDirection.Request);
            header.Name.ShouldEqual("application/xml");

            header = mimeTypes[1];
            header.Direction.ShouldEqual(HttpDirection.Request);
            header.Name.ShouldEqual("text/json");

            header = mimeTypes[2];
            header.Direction.ShouldEqual(HttpDirection.Response);
            header.Name.ShouldEqual("application/json");

            header = mimeTypes[3];
            header.Direction.ShouldEqual(HttpDirection.Response);
            header.Name.ShouldEqual("text/xml");
        }

        public class NoMimeTypesGetHandler
        {
            public object Execute_NoHeaders(object request) { return null; }
        }

        [Test]
        public void should_not_set_endpoint_mime_types_when_none_are_set_on_handlers_or_actions()
        {
            _mimeTypeConvention.GetDescription(_graph.GetAction<NoMimeTypesGetHandler>()
                .ParentChain()).Count.ShouldEqual(0);
        }
    }
}