using NUnit.Framework;
using Should;

namespace Tests.Specification.SpecificationServiceEndpointTests
{
    public class ErrorsTests : TestBase
    {
        [Test]
        public void should_set_endpoint_errors_on_handlers_and_actions()
        {
            var endpoint = Spec.GetEndpoint<ErrorDescriptions.ErrorsGetHandler>();

            endpoint.errors.Count.ShouldEqual(4);

            var error = endpoint.errors[0];
            error.status.ShouldEqual(410);
            error.name.ShouldEqual("410 error on handler");
            error.comments.ShouldEqual("410 error on action description");

            error = endpoint.errors[1];
            error.status.ShouldEqual(411);
            error.name.ShouldEqual("411 error on handler");
            error.comments.ShouldBeNull();

            error = endpoint.errors[2];
            error.status.ShouldEqual(412);
            error.name.ShouldEqual("412 error on action");
            error.comments.ShouldEqual("412 error on action description");

            error = endpoint.errors[3];
            error.status.ShouldEqual(413);
            error.name.ShouldEqual("413 error on action");
            error.comments.ShouldBeNull();
        }

        [Test]
        public void should_not_set_endpoint_errors_when_none_are_set_on_handlers_or_actions()
        {
            Spec.GetEndpoint<ErrorDescriptions.NoErrorsGetHandler>().errors.Count.ShouldEqual(0);
        }
    }
}