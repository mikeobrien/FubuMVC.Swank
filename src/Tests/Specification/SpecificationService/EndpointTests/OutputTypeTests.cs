using FubuMVC.Swank.Extensions;
using NUnit.Framework;
using Should;

namespace Tests.Specification.SpecificationService.EndpointTests
{
    public class OutputTypeTests : TestBase
    {
        [Test]
        public void should_set_get_output_type_description()
        {
            var response = Spec.GetEndpoint<OutputTypeDescriptions.GetHandler>().Response;

            response.Name.ShouldEqual("GetResponse");
            response.Comments.ShouldEqual("Some get response description");
            response.IsArray.ShouldBeFalse();
        }

        [Test]
        public void should_set_post_output_type_description()
        {
            var response = Spec.GetEndpoint<OutputTypeDescriptions.PostHandler>().Response;

            response.Name.ShouldEqual("PostResponse");
            response.Comments.ShouldEqual("Some post response description");
            response.IsArray.ShouldBeFalse();
        }

        [Test]
        public void should_set_put_output_type_description()
        {
            var response = Spec.GetEndpoint<OutputTypeDescriptions.PutHandler>().Response;

            response.Name.ShouldEqual("PutResponse");
            response.Comments.ShouldEqual("Some put response description");
            response.IsArray.ShouldBeFalse();
        }

        [Test]
        public void should_set_delete_output_type_description()
        {
            var response = Spec.GetEndpoint<OutputTypeDescriptions.DeleteHandler>().Response;

            response.Name.ShouldEqual("DeleteResponse");
            response.Comments.ShouldEqual("Some delete response description");
            response.IsArray.ShouldBeFalse();
        }

        [Test]
        public void should_set_the_datatype_for__output_types_to_a_hash_of_the_datatype()
        {
            Spec.GetEndpoint<OutputTypeDescriptions.GetHandler>().Response.TypeId
                .ShouldEqual(typeof(OutputTypeDescriptions.GetResponse).GetHash());

            Spec.GetEndpoint<OutputTypeDescriptions.PostHandler>().Response.TypeId
                .ShouldEqual(typeof(OutputTypeDescriptions.PostResponse).GetHash());

            Spec.GetEndpoint<OutputTypeDescriptions.PutHandler>().Response.TypeId
                .ShouldEqual(typeof(OutputTypeDescriptions.PutResponse).GetHash());

            Spec.GetEndpoint<OutputTypeDescriptions.DeleteHandler>().Response.TypeId
                .ShouldEqual(typeof(OutputTypeDescriptions.DeleteResponse).GetHash());
        }

        [Test]
        public void should_set_output_type_default_collection_name_and_datatype_should_be_a_hash_of_the_element_type_and_action()
        {
            var response = Spec.GetEndpoint<OutputTypeDescriptions.CollectionPostHandler>().Response;

            response.Name.ShouldEqual("ArrayOfResponseItem");
            response.Comments.ShouldBeNull();
            response.TypeId.ShouldEqual(typeof(OutputTypeDescriptions.ResponseItem).GetHash());
            response.IsArray.ShouldBeTrue();
        }

        [Test]
        public void should_set_output_type_default_collection_name_of_inherited_collection_and_datatype_should_be_a_hash_of_the_element_type_and_action()
        {
            var response = Spec.GetEndpoint<OutputTypeDescriptions.InheritedCollectionPostHandler>().Response;

            response.Name.ShouldEqual("ArrayOfResponseItem");
            response.Comments.ShouldBeNull();
            response.TypeId.ShouldEqual(typeof(OutputTypeDescriptions.ResponseItem).GetHash());
            response.IsArray.ShouldBeTrue();
        }

        [Test]
        public void should_set_output_type_name_to_the_xml_type_name()
        {
            var response = Spec.GetEndpoint<OutputTypeDescriptions.OverridenRequestPostHandler>().Response;

            response.Name.ShouldEqual("NewItemName");
            response.Comments.ShouldBeNull();
            response.TypeId.ShouldEqual(typeof(OutputTypeDescriptions.OverridenResponseItem).GetHash());
            response.IsArray.ShouldBeFalse();
        }

        [Test]
        public void should_set_output_type_collection_name_to_the_xml_type_name()
        {
            var response = Spec.GetEndpoint<OutputTypeDescriptions.OverridenCollectionPostHandler>().Response;

            response.Name.ShouldEqual("NewCollectionName");
            response.Comments.ShouldBeNull();
            response.TypeId.ShouldEqual(typeof(OutputTypeDescriptions.OverridenResponseItem).GetHash());
            response.IsArray.ShouldBeTrue();
        }
    }
}