using NUnit.Framework;
using Should;

namespace Tests.Specification.SpecificationService.EndpointTests
{
    public class MimeTypeTests : TestBase
    {
        [Test]
        public void should_set_endpoint_headers_on_handlers_and_actions()
        {
            var endpoint = Spec.GetEndpoint<MimeTypeDescriptions.MimeTypesGetHandler>();

            endpoint.Request.MimeTypes.Count.ShouldEqual(2);
            endpoint.Request.MimeTypes[0].ShouldEqual("application/xml");
            endpoint.Request.MimeTypes[1].ShouldEqual("text/json");

            endpoint.Response.MimeTypes.Count.ShouldEqual(2);
            endpoint.Response.MimeTypes[0].ShouldEqual("application/json");
            endpoint.Response.MimeTypes[1].ShouldEqual("text/xml");
        }

        [Test]
        public void should_not_set_endpoint_headers_when_none_are_set_on_handlers_or_actions()
        {
            var endpoint = Spec.GetEndpoint<MimeTypeDescriptions.NoMimeTypesGetHandler>();
            endpoint.Request.MimeTypes.Count.ShouldEqual(0);
            endpoint.Response.MimeTypes.Count.ShouldEqual(0);
        }
    }
}