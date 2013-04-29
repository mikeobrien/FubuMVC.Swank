using FubuMVC.Swank.Extensions;
using NUnit.Framework;
using Should;

namespace Tests.Specification.SpecificationService.EndpointTests
{
    public class InputTypeTests : TestBase
    {
        [Test]
        public void should_set_post_input_type_description()
        {
            var request = Spec.GetEndpoint<InputTypeDescriptions.PostHandler>().Request;

            request.Name.ShouldEqual("PostRequest");
            request.Comments.ShouldEqual("Some post request description");
            request.Collection.ShouldBeFalse();
        }

        [Test]
        public void should_set_put_input_type_description()
        {
            var request = Spec.GetEndpoint<InputTypeDescriptions.PutHandler>().Request;

            request.Name.ShouldEqual("PutRequest");
            request.Comments.ShouldEqual("Some put request description");
            request.Collection.ShouldBeFalse();
        }

        [Test]
        public void should_set_the_datatype_for_post_input_post_and_put_types_to_a_hash_of_the_datatype_and_handler_method()
        {
            Spec.GetEndpoint<InputTypeDescriptions.PostHandler>().Request.Type
                .ShouldEqual(typeof(InputTypeDescriptions.PostRequest)
                    .GetHash(typeof(InputTypeDescriptions.PostHandler).GetExecuteMethod()));

            Spec.GetEndpoint<InputTypeDescriptions.PutHandler>().Request.Type
                .ShouldEqual(typeof(InputTypeDescriptions.PutRequest)
                    .GetHash(typeof(InputTypeDescriptions.PutHandler).GetExecuteMethod()));
        }

        [Test]
        public void should_not_set_input_type_for_get()
        {
            Spec.GetEndpoint<InputTypeDescriptions.GetHandler>().Request.ShouldBeNull();
        }

        [Test]
        public void should_not_set_input_type_for_delete()
        {
            Spec.GetEndpoint<InputTypeDescriptions.DeleteHandler>().Request.ShouldBeNull();
        }

        [Test]
        public void should_set_input_type_default_collection_name_and_datatype_should_be_a_hash_of_the_element_type_and_action()
        {
            var request = Spec.GetEndpoint<InputTypeDescriptions.CollectionPostHandler>().Request;

            request.Name.ShouldEqual("ArrayOfRequestItem");
            request.Comments.ShouldBeNull();
            request.Type.ShouldEqual(typeof(InputTypeDescriptions.RequestItem)
                .GetHash(typeof(InputTypeDescriptions.CollectionPostHandler).GetExecuteMethod()));
            request.Collection.ShouldBeTrue();
        }

        [Test]
        public void should_set_input_type_default_collection_name_of_inherited_collection_and_datatype_should_be_a_hash_of_the_element_type_and_action()
        {
            var request = Spec.GetEndpoint<InputTypeDescriptions.InheritedCollectionPostHandler>().Request;

            request.Name.ShouldEqual("ArrayOfRequestItem");
            request.Comments.ShouldBeNull();
            request.Type.ShouldEqual(typeof(InputTypeDescriptions.RequestItem)
                .GetHash(typeof(InputTypeDescriptions.InheritedCollectionPostHandler).GetExecuteMethod()));
            request.Collection.ShouldBeTrue();
        }

        [Test]
        public void should_set_input_type_name_to_the_xml_type_name()
        {
            var request = Spec.GetEndpoint<InputTypeDescriptions.OverridenRequestPostHandler>().Request;

            request.Name.ShouldEqual("NewItemName");
            request.Comments.ShouldBeNull();
            request.Type.ShouldEqual(typeof(InputTypeDescriptions.OverridenRequestItem)
                .GetHash(typeof(InputTypeDescriptions.OverridenRequestPostHandler).GetExecuteMethod()));
            request.Collection.ShouldBeFalse();
        }

        [Test]
        public void should_set_input_type_collection_name_to_the_xml_type_name()
        {
            var request = Spec.GetEndpoint<InputTypeDescriptions.OverridenCollectionPostHandler>().Request;

            request.Name.ShouldEqual("NewCollectionName");
            request.Comments.ShouldBeNull();
            request.Type.ShouldEqual(typeof(InputTypeDescriptions.OverridenRequestItem)
                .GetHash(typeof(InputTypeDescriptions.OverridenCollectionPostHandler).GetExecuteMethod()));
            request.Collection.ShouldBeTrue();
        }
    }
}