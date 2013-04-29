using NUnit.Framework;
using Should;

namespace Tests.Specification.SpecificationService.EndpointTests
{
    public class EndpointTests : TestBase
    {
        [Test]
        public void should_not_set_handler_description_for_endpoint_with_no_description()
        {
            var endpoint = Spec.GetEndpoint<EndpointDescriptions.NoDescriptionGetHandler>();
            endpoint.Name.ShouldBeNull();
            endpoint.Comments.ShouldBeNull();
            endpoint.Method.ShouldEqual("GET");
            endpoint.Url.ShouldEqual("/endpointdescriptions/nodescription");
        }

        [Test]
        public void should_not_set_embedded_handler_description_when_resource_attribute_is_applied()
        {
            var resource = Spec.GetResource<ControllerResource.Controller>();

            resource.Name.ShouldEqual("Some Controller");
            resource.Comments.ShouldEqual("<p><strong>This is a resource</strong></p>");

            var endpoint = resource.Endpoints[0];
            endpoint.Name.ShouldBeNull();
            endpoint.Comments.ShouldBeNull();
            endpoint.Method.ShouldBeNull();
            endpoint.Url.ShouldEqual("/controllerresource");
        }

        [Test]
        public void should_set_embedded_text_handler_description()
        {
            var endpoint = Spec.GetEndpoint<EndpointDescriptions.HandlerDescription.EmbeddedDescriptionGetHandler>();
            endpoint.Name.ShouldBeNull();
            endpoint.Comments.ShouldEqual("<b>An embedded handler text description</b>");
            endpoint.Method.ShouldEqual("GET");
            endpoint.Url.ShouldEqual("/endpointdescriptions/handlerdescription/embeddeddescription");
        }

        [Test]
        public void should_set_handler_description_for_get_endpoint()
        {
            var endpoint = Spec.GetEndpoint<EndpointDescriptions.HandlerDescription.GetHandler>();
            endpoint.Name.ShouldEqual("Some get handler name");
            endpoint.Comments.ShouldEqual("Some get handler description");
            endpoint.Method.ShouldEqual("GET");
            endpoint.Url.ShouldEqual("/endpointdescriptions/handlerdescription/get/{Id}");
        }

        [Test]
        public void should_set_handler_description_for_post_endpoint()
        {
            var endpoint = Spec.GetEndpoint<EndpointDescriptions.HandlerDescription.PostHandler>();
            endpoint.Name.ShouldEqual("Some post handler name");
            endpoint.Comments.ShouldEqual("Some post handler description");
            endpoint.Method.ShouldEqual("POST");
            endpoint.Url.ShouldEqual("/endpointdescriptions/handlerdescription/post");
        }

        [Test]
        public void should_set_handler_description_for_put_endpoint()
        {
            var endpoint = Spec.GetEndpoint<EndpointDescriptions.HandlerDescription.PutHandler>();
            endpoint.Name.ShouldEqual("Some put handler name");
            endpoint.Comments.ShouldEqual("Some put handler description");
            endpoint.Method.ShouldEqual("PUT");
            endpoint.Url.ShouldEqual("/endpointdescriptions/handlerdescription/put/{Id}");
        }

        [Test]
        public void should_set_handler_description_for_delete_endpoint()
        {
            var endpoint = Spec.GetEndpoint<EndpointDescriptions.HandlerDescription.DeleteHandler>();
            endpoint.Name.ShouldEqual("Some delete handler name");
            endpoint.Comments.ShouldEqual("Some delete handler description");
            endpoint.Method.ShouldEqual("DELETE");
            endpoint.Url.ShouldEqual("/endpointdescriptions/handlerdescription/delete/{Id}");
        }

        [Test]
        public void should_set_embedded_markdown_action_description()
        {
            var endpoint = Spec.GetEndpoint<EndpointDescriptions.ActionDescription.EmbeddedDescriptionGetHandler>();
            endpoint.Name.ShouldBeNull();
            endpoint.Comments.ShouldEqual("<p><strong>An embedded action markdown description</strong></p>");
            endpoint.Method.ShouldEqual("GET");
            endpoint.Url.ShouldEqual("/endpointdescriptions/actiondescription/embeddeddescription");
        }

        [Test]
        public void should_set_action_description_for_get_endpoint()
        {
            var endpoint = Spec.GetEndpoint<EndpointDescriptions.ActionDescription.GetHandler>();
            endpoint.Name.ShouldEqual("Some get action name");
            endpoint.Comments.ShouldEqual("Some get action description");
            endpoint.Method.ShouldEqual("GET");
            endpoint.Url.ShouldEqual("/endpointdescriptions/actiondescription/get/{Id}");
        }

        [Test]
        public void should_set_action_description_for_post_endpoint()
        {
            var endpoint = Spec.GetEndpoint<EndpointDescriptions.ActionDescription.PostHandler>();
            endpoint.Name.ShouldEqual("Some post action name");
            endpoint.Comments.ShouldEqual("Some post action description");
            endpoint.Method.ShouldEqual("POST");
            endpoint.Url.ShouldEqual("/endpointdescriptions/actiondescription/post");
        }

        [Test]
        public void should_set_action_description_for_put_endpoint()
        {
            var endpoint = Spec.GetEndpoint<EndpointDescriptions.ActionDescription.PutHandler>();
            endpoint.Name.ShouldEqual("Some put action name");
            endpoint.Comments.ShouldEqual("Some put action description");
            endpoint.Method.ShouldEqual("PUT");
            endpoint.Url.ShouldEqual("/endpointdescriptions/actiondescription/put/{Id}");
        }

        [Test]
        public void should_set_action_description_for_delete_endpoint()
        {
            var endpoint = Spec.GetEndpoint<EndpointDescriptions.ActionDescription.DeleteHandler>();
            endpoint.Name.ShouldEqual("Some delete action name");
            endpoint.Comments.ShouldEqual("Some delete action description");
            endpoint.Method.ShouldEqual("DELETE");
            endpoint.Url.ShouldEqual("/endpointdescriptions/actiondescription/delete/{Id}");
        }

        [Test]
        public void should_not_show_hidden_endpoints()
        {
            Spec.GetEndpoint<HiddenEndpointAttributes.HiddenActionGetHandler>().ShouldBeNull();
        }

        [Test]
        public void should_not_show_endpoints_in_hidden_handlers()
        {
            Spec.GetEndpoint<HiddenEndpointAttributes.HiddenGetHandler>().ShouldBeNull();
        }

        [Test]
        public void should_show_endpoints_not_marked_hidden()
        {
            Spec.GetEndpoint<HiddenEndpointAttributes.VisibleGetHandler>().ShouldNotBeNull();
        }
    }
}