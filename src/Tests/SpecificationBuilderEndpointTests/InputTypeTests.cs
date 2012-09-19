using FubuMVC.Swank;
using NUnit.Framework;
using Should;

namespace Tests.SpecificationBuilderEndpointTests
{
    public class InputTypeTests : TestBase
    {
        [Test]
        public void should_set_post_input_type_description()
        {
            var request = _spec.GetEndpoint<InputTypeDescriptions.PostHandler>().request;

            request.name.ShouldEqual("PostRequest");
            request.comments.ShouldEqual("Some post request description");
            request.collection.ShouldBeFalse();
        }

        [Test]
        public void should_set_put_input_type_description()
        {
            var request = _spec.GetEndpoint<InputTypeDescriptions.PutHandler>().request;

            request.name.ShouldEqual("PutRequest");
            request.comments.ShouldEqual("Some put request description");
            request.collection.ShouldBeFalse();
        }

        [Test]
        public void should_set_the_datatype_for_post_input_post_and_put_types_to_a_hash_of_the_datatype_and_handler_method()
        {
            _spec.GetEndpoint<InputTypeDescriptions.PostHandler>().request.type
                .ShouldEqual(typeof(InputTypeDescriptions.PostRequest)
                    .GetHash(typeof(InputTypeDescriptions.PostHandler).GetExecuteMethod()));

            _spec.GetEndpoint<InputTypeDescriptions.PutHandler>().request.type
                .ShouldEqual(typeof(InputTypeDescriptions.PutRequest)
                    .GetHash(typeof(InputTypeDescriptions.PutHandler).GetExecuteMethod()));
        }

        [Test]
        public void should_not_set_input_type_for_get()
        {
            _spec.GetEndpoint<InputTypeDescriptions.GetHandler>().request.ShouldBeNull();
        }

        [Test]
        public void should_not_set_input_type_for_delete()
        {
            _spec.GetEndpoint<InputTypeDescriptions.DeleteHandler>().request.ShouldBeNull();
        }

        [Test]
        public void should_set_input_type_default_collection_name_and_datatype_should_be_a_hash_of_the_element_type_and_action()
        {
            var request = _spec.GetEndpoint<InputTypeDescriptions.CollectionPostHandler>().request;

            request.name.ShouldEqual("ArrayOfRequestItem");
            request.comments.ShouldBeNull();
            request.type.ShouldEqual(typeof(InputTypeDescriptions.RequestItem)
                .GetHash(typeof(InputTypeDescriptions.CollectionPostHandler).GetExecuteMethod()));
            request.collection.ShouldBeTrue();
        }

        [Test]
        public void should_set_input_type_default_collection_name_of_inherited_collection_and_datatype_should_be_a_hash_of_the_element_type_and_action()
        {
            var request = _spec.GetEndpoint<InputTypeDescriptions.InheritedCollectionPostHandler>().request;

            request.name.ShouldEqual("ArrayOfRequestItem");
            request.comments.ShouldBeNull();
            request.type.ShouldEqual(typeof(InputTypeDescriptions.RequestItem)
                .GetHash(typeof(InputTypeDescriptions.InheritedCollectionPostHandler).GetExecuteMethod()));
            request.collection.ShouldBeTrue();
        }

        [Test]
        public void should_set_input_type_name_to_the_xml_type_name()
        {
            var request = _spec.GetEndpoint<InputTypeDescriptions.OverridenRequestPostHandler>().request;

            request.name.ShouldEqual("NewItemName");
            request.comments.ShouldBeNull();
            request.type.ShouldEqual(typeof(InputTypeDescriptions.OverridenRequestItem)
                .GetHash(typeof(InputTypeDescriptions.OverridenRequestPostHandler).GetExecuteMethod()));
            request.collection.ShouldBeFalse();
        }

        [Test]
        public void should_set_input_type_collection_name_to_the_xml_type_name()
        {
            var request = _spec.GetEndpoint<InputTypeDescriptions.OverridenCollectionPostHandler>().request;

            request.name.ShouldEqual("NewCollectionName");
            request.comments.ShouldBeNull();
            request.type.ShouldEqual(typeof(InputTypeDescriptions.OverridenRequestItem)
                .GetHash(typeof(InputTypeDescriptions.OverridenCollectionPostHandler).GetExecuteMethod()));
            request.collection.ShouldBeTrue();
        }
    }
}