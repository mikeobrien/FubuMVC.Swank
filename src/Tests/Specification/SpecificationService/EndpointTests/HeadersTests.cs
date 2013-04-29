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

            endpoint.Headers.Count.ShouldEqual(4);

            var header = endpoint.Headers[0];
            header.Type.ShouldEqual("Request");
            header.Name.ShouldEqual("accept");
            header.Comments.ShouldEqual("This is an endpoint description.");
            header.Optional.ShouldBeTrue();

            header = endpoint.Headers[1];
            header.Type.ShouldEqual("Request");
            header.Name.ShouldEqual("api-key");
            header.Comments.ShouldEqual("This is a handler description.");
            header.Optional.ShouldBeTrue();

            header = endpoint.Headers[2];
            header.Type.ShouldEqual("Response");
            header.Name.ShouldEqual("content-length");
            header.Comments.ShouldBeNull();
            header.Optional.ShouldBeFalse();

            header = endpoint.Headers[3];
            header.Type.ShouldEqual("Response");
            header.Name.ShouldEqual("content-type");
            header.Comments.ShouldBeNull();
            header.Optional.ShouldBeFalse();
        }

        [Test]
        public void should_not_set_endpoint_headers_when_none_are_set_on_handlers_or_actions()
        {
            Spec.GetEndpoint<HeaderDescriptions.NoHeadersGetHandler>().StatusCodes.Count.ShouldEqual(0);
        }
    }
}