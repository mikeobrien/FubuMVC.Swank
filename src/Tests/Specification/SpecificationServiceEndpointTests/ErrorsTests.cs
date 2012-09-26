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

            endpoint.Errors.Count.ShouldEqual(4);

            var error = endpoint.Errors[0];
            error.Status.ShouldEqual(410);
            error.Name.ShouldEqual("410 error on handler");
            error.Comments.ShouldEqual("410 error on action description");

            error = endpoint.Errors[1];
            error.Status.ShouldEqual(411);
            error.Name.ShouldEqual("411 error on handler");
            error.Comments.ShouldBeNull();

            error = endpoint.Errors[2];
            error.Status.ShouldEqual(412);
            error.Name.ShouldEqual("412 error on action");
            error.Comments.ShouldEqual("412 error on action description");

            error = endpoint.Errors[3];
            error.Status.ShouldEqual(413);
            error.Name.ShouldEqual("413 error on action");
            error.Comments.ShouldBeNull();
        }

        [Test]
        public void should_not_set_endpoint_errors_when_none_are_set_on_handlers_or_actions()
        {
            Spec.GetEndpoint<ErrorDescriptions.NoErrorsGetHandler>().Errors.Count.ShouldEqual(0);
        }
    }
}