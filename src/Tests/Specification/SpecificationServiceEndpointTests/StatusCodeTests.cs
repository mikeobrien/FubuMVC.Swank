using NUnit.Framework;
using Should;

namespace Tests.Specification.SpecificationServiceEndpointTests
{
    public class StatusCodeTests : TestBase
    {
        [Test]
        public void should_set_endpoint_status_codes_on_handlers_and_actions()
        {
            var endpoint = Spec.GetEndpoint<StatusCodeDescriptions.StatusCodesGetHandler>();

            endpoint.StatusCodes.Count.ShouldEqual(4);

            var statusCode = endpoint.StatusCodes[0];
            statusCode.Code.ShouldEqual(410);
            statusCode.Name.ShouldEqual("410 error on handler");
            statusCode.Comments.ShouldEqual("410 error on action description");

            statusCode = endpoint.StatusCodes[1];
            statusCode.Code.ShouldEqual(411);
            statusCode.Name.ShouldEqual("411 error on handler");
            statusCode.Comments.ShouldBeNull();

            statusCode = endpoint.StatusCodes[2];
            statusCode.Code.ShouldEqual(412);
            statusCode.Name.ShouldEqual("412 error on action");
            statusCode.Comments.ShouldEqual("412 error on action description");

            statusCode = endpoint.StatusCodes[3];
            statusCode.Code.ShouldEqual(413);
            statusCode.Name.ShouldEqual("413 error on action");
            statusCode.Comments.ShouldBeNull();
        }

        [Test]
        public void should_not_set_endpoint_status_codes_when_none_are_set_on_handlers_or_actions()
        {
            Spec.GetEndpoint<StatusCodeDescriptions.NoStatusCodesGetHandler>().StatusCodes.Count.ShouldEqual(0);
        }
    }
}