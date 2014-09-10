using NUnit.Framework;
using Should;

namespace Tests.Specification.SpecificationService.EndpointTests
{
    public class HeadersTests : TestBase
    {
        [Test]
        public void should_set_endpoint_headers_on_handlers_and_actions()
        {
            var endpoint = Spec.GetEndpoint<HeaderDescriptions.HeadersGetHandler>();

            endpoint.Request.Headers.Count.ShouldEqual(2);

            var header = endpoint.Request.Headers[0];
            header.Name.ShouldEqual("accept");
            header.Comments.ShouldEqual("This is an endpoint description.");
            header.Optional.ShouldBeTrue();
            header.Required.ShouldBeFalse();
            header.IsAccept.ShouldBeTrue();
            header.IsContentType.ShouldBeFalse();

            header = endpoint.Request.Headers[1];
            header.Name.ShouldEqual("api-key");
            header.Comments.ShouldEqual("This is a handler description.");
            header.Optional.ShouldBeFalse();
            header.Required.ShouldBeTrue();
            header.IsAccept.ShouldBeFalse();
            header.IsContentType.ShouldBeFalse();

            endpoint.Response.Headers.Count.ShouldEqual(2);

            header = endpoint.Response.Headers[0];
            header.Name.ShouldEqual("content-length");
            header.Comments.ShouldBeNull();
            header.Optional.ShouldBeFalse();
            header.Required.ShouldBeFalse();
            header.IsAccept.ShouldBeFalse();
            header.IsContentType.ShouldBeFalse();

            header = endpoint.Response.Headers[1];
            header.Name.ShouldEqual("content-type");
            header.Comments.ShouldBeNull();
            header.Optional.ShouldBeFalse();
            header.Required.ShouldBeFalse();
            header.IsAccept.ShouldBeFalse();
            header.IsContentType.ShouldBeTrue();
        }

        [Test]
        public void should_not_set_endpoint_headers_when_none_are_set_on_handlers_or_actions()
        {
            var endpoint = Spec.GetEndpoint<HeaderDescriptions.NoHeadersGetHandler>();
            endpoint.Request.Headers.Count.ShouldEqual(0);
            endpoint.Response.Headers.Count.ShouldEqual(0);
        }
    }
}