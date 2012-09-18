using System;
using NUnit.Framework;
using Should;

namespace Tests.SpecificationBuilderEndpointTests
{
    public class EndpointTests : TestBase
    {
        [Test]
        public void should_not_set_handler_description_for_endpoint_with_no_description()
        {
            var endpoint = _spec.GetEndpoint<EndpointDescriptions.NoDescriptionGetHandler>();
            endpoint.name.ShouldBeNull();
            endpoint.comments.ShouldBeNull();
            endpoint.method.ShouldEqual("GET");
            endpoint.url.ShouldEqual("/endpointdescriptions/nodescription");
        }

        [Test]
        public void should_set_embedded_markdown_handler_description()
        {
            var endpoint = _spec.GetEndpoint<EndpointDescriptions.HandlerDescription.EmbeddedDescriptionGetHandler>();
            endpoint.name.ShouldBeNull();
            endpoint.comments.ShouldEqual("<b>An embedded handler text description</b>");
            endpoint.method.ShouldEqual("GET");
            endpoint.url.ShouldEqual("/endpointdescriptions/handlerdescription/embeddeddescription");
        }

        [Test]
        public void should_not_set_embedded_handler_description_when_resource_attribute_is_applied()
        {
            var resource = _spec.GetResource<ControllerResource.Controller>();

            resource.name.ShouldEqual("Some Controller");
            resource.comments.ShouldEqual("<p><strong>This is a resource</strong></p>");

            var endpoint = resource.endpoints[0];
            endpoint.name.ShouldBeNull();
            endpoint.comments.ShouldBeNull();
            endpoint.method.ShouldBeNull();
            endpoint.url.ShouldEqual("/controllerresource");
        }

        [Test]
        public void should_set_embedded_text_action_description()
        {
            var endpoint = _spec.GetEndpoint<EndpointDescriptions.ActionDescription.EmbeddedDescriptionGetHandler>();
            endpoint.name.ShouldBeNull();
            endpoint.comments.ShouldEqual("<p><strong>An embedded action markdown description</strong></p>");
            endpoint.method.ShouldEqual("GET");
            endpoint.url.ShouldEqual("/endpointdescriptions/actiondescription/embeddeddescription");
        }

        [Test]
        public void should_set_handler_description_for_get_endpoint()
        {
            var endpoint = _spec.GetEndpoint<EndpointDescriptions.HandlerDescription.GetHandler>();
            endpoint.name.ShouldEqual("Some get handler name");
            endpoint.comments.ShouldEqual("Some get handler description");
            endpoint.method.ShouldEqual("GET");
            endpoint.url.ShouldEqual("/endpointdescriptions/handlerdescription/get/{Id}");
        }

        [Test]
        public void should_set_handler_description_for_post_endpoint()
        {
            var endpoint = _spec.GetEndpoint<EndpointDescriptions.HandlerDescription.PostHandler>();
            endpoint.name.ShouldEqual("Some post handler name");
            endpoint.comments.ShouldEqual("Some post handler description");
            endpoint.method.ShouldEqual("POST");
            endpoint.url.ShouldEqual("/endpointdescriptions/handlerdescription/post");
        }

        [Test]
        public void should_set_handler_description_for_put_endpoint()
        {
            var endpoint = _spec.GetEndpoint<EndpointDescriptions.HandlerDescription.PutHandler>();
            endpoint.name.ShouldEqual("Some put handler name");
            endpoint.comments.ShouldEqual("Some put handler description");
            endpoint.method.ShouldEqual("PUT");
            endpoint.url.ShouldEqual("/endpointdescriptions/handlerdescription/put/{Id}");
        }

        [Test]
        public void should_set_handler_description_for_delete_endpoint()
        {
            var endpoint = _spec.GetEndpoint<EndpointDescriptions.HandlerDescription.DeleteHandler>();
            endpoint.name.ShouldEqual("Some delete handler name");
            endpoint.comments.ShouldEqual("Some delete handler description");
            endpoint.method.ShouldEqual("DELETE");
            endpoint.url.ShouldEqual("/endpointdescriptions/handlerdescription/delete/{Id}");
        }

        [Test]
        public void should_set_action_description_for_get_endpoint()
        {
            var endpoint = _spec.GetEndpoint<EndpointDescriptions.ActionDescription.GetHandler>();
            endpoint.name.ShouldEqual("Some get action name");
            endpoint.comments.ShouldEqual("Some get action description");
            endpoint.method.ShouldEqual("GET");
            endpoint.url.ShouldEqual("/endpointdescriptions/actiondescription/get/{Id}");
        }

        [Test]
        public void should_set_action_description_for_post_endpoint()
        {
            var endpoint = _spec.GetEndpoint<EndpointDescriptions.ActionDescription.PostHandler>();
            endpoint.name.ShouldEqual("Some post action name");
            endpoint.comments.ShouldEqual("Some post action description");
            endpoint.method.ShouldEqual("POST");
            endpoint.url.ShouldEqual("/endpointdescriptions/actiondescription/post");
        }

        [Test]
        public void should_set_action_description_for_put_endpoint()
        {
            var endpoint = _spec.GetEndpoint<EndpointDescriptions.ActionDescription.PutHandler>();
            endpoint.name.ShouldEqual("Some put action name");
            endpoint.comments.ShouldEqual("Some put action description");
            endpoint.method.ShouldEqual("PUT");
            endpoint.url.ShouldEqual("/endpointdescriptions/actiondescription/put/{Id}");
        }

        [Test]
        public void should_set_action_description_for_delete_endpoint()
        {
            var endpoint = _spec.GetEndpoint<EndpointDescriptions.ActionDescription.DeleteHandler>();
            endpoint.name.ShouldEqual("Some delete action name");
            endpoint.comments.ShouldEqual("Some delete action description");
            endpoint.method.ShouldEqual("DELETE");
            endpoint.url.ShouldEqual("/endpointdescriptions/actiondescription/delete/{Id}");
        }

        [Test]
        public void should_not_show_hidden_endpoints()
        {
            _spec.GetEndpoint<HiddenEndpointAttributes.HiddenActionGetHandler>().ShouldBeNull();
        }

        [Test]
        public void should_not_show_endpoints_in_hidden_handlers()
        {
            _spec.GetEndpoint<HiddenEndpointAttributes.HiddenGetHandler>().ShouldBeNull();
        }

        [Test]
        public void should_show_endpoints_not_marked_hidden()
        {
            _spec.GetEndpoint<HiddenEndpointAttributes.VisibleGetHandler>().ShouldNotBeNull();
        }
    }
}