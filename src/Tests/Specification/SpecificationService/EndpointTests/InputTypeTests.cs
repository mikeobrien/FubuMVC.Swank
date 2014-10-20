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

            request.Comments.ShouldEqual("Some post request description");
            request.Body.Description[0].IsComplexType.ShouldEqual(true);
        }

        [Test]
        public void should_set_put_input_type_description()
        {
            var request = Spec.GetEndpoint<InputTypeDescriptions.PutHandler>().Request;

            request.Comments.ShouldEqual("Some put request description");
            request.Body.Description[0].IsComplexType.ShouldEqual(true);
        }

        [Test]
        public void should_set_the_datatype_for_post_input_post_types()
        {
            Spec.GetEndpoint<InputTypeDescriptions.PostHandler>()
                .Request.Body.Description[0].Name.ShouldEqual("PostRequest");
        }

        [Test]
        public void should_set_the_datatype_for_post_input_put_types()
        {
            Spec.GetEndpoint<InputTypeDescriptions.PutHandler>()
                .Request.Body.Description[0].Name.ShouldEqual("PutRequest");
        }

        [Test]
        public void should_not_set_input_type_for_get()
        {
            Spec.GetEndpoint<InputTypeDescriptions.GetHandler>().Request.Body.Description.ShouldBeNull();
        }

        [Test]
        public void should_not_set_input_type_for_delete()
        {
            Spec.GetEndpoint<InputTypeDescriptions.DeleteHandler>().Request.Body.Description.ShouldBeNull();
        }

        [Test]
        public void should_set_input_type_default_collection_name_and_datatype()
        {
            var request = Spec.GetEndpoint<InputTypeDescriptions.CollectionPostHandler>().Request;

            request.Comments.ShouldBeNull();
            request.Body.Description[0].Name.ShouldEqual("ArrayOfRequestItem");
            request.Body.Description[0].IsArray.ShouldEqual(true);
        }

        [Test]
        public void should_set_input_type_default_collection_name_of_inherited_collection_and_datatype()
        {
            var request = Spec.GetEndpoint<InputTypeDescriptions.InheritedCollectionPostHandler>().Request;

            request.Comments.ShouldBeNull();
            request.Body.Description[0].Name.ShouldEqual("ArrayOfRequestItem");
            request.Body.Description[0].IsArray.ShouldEqual(true);
        }

        [Test]
        public void should_set_input_type_name_to_the_xml_type_name()
        {
            var request = Spec.GetEndpoint<InputTypeDescriptions.OverridenRequestPostHandler>().Request;

            request.Comments.ShouldBeNull();
            request.Body.Description[0].Name.ShouldEqual("NewItemName");
        }

        [Test]
        public void should_set_input_type_collection_name_to_the_xml_type_name()
        {
            var request = Spec.GetEndpoint<InputTypeDescriptions.OverridenCollectionPostHandler>().Request;

            request.Comments.ShouldBeNull();
            request.Body.Description[0].Name.ShouldEqual("NewCollectionName");
        }
    }
}