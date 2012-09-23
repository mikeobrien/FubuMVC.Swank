using FubuMVC.Swank.Extensions;
using NUnit.Framework;
using Should;

namespace Tests.Specification.SpecificationBuilderEndpointTests
{
    public class OutputTypeTests : TestBase
    {
        [Test]
        public void should_set_get_output_type_description()
        {
            var response = Spec.GetEndpoint<OutputTypeDescriptions.GetHandler>().response;

            response.name.ShouldEqual("GetResponse");
            response.comments.ShouldEqual("Some get response description");
            response.collection.ShouldBeFalse();
        }

        [Test]
        public void should_set_post_output_type_description()
        {
            var response = Spec.GetEndpoint<OutputTypeDescriptions.PostHandler>().response;

            response.name.ShouldEqual("PostResponse");
            response.comments.ShouldEqual("Some post response description");
            response.collection.ShouldBeFalse();
        }

        [Test]
        public void should_set_put_output_type_description()
        {
            var response = Spec.GetEndpoint<OutputTypeDescriptions.PutHandler>().response;

            response.name.ShouldEqual("PutResponse");
            response.comments.ShouldEqual("Some put response description");
            response.collection.ShouldBeFalse();
        }

        [Test]
        public void should_set_delete_output_type_description()
        {
            var response = Spec.GetEndpoint<OutputTypeDescriptions.DeleteHandler>().response;

            response.name.ShouldEqual("DeleteResponse");
            response.comments.ShouldEqual("Some delete response description");
            response.collection.ShouldBeFalse();
        }

        [Test]
        public void should_set_the_datatype_for__output_types_to_a_hash_of_the_datatype()
        {
            Spec.GetEndpoint<OutputTypeDescriptions.GetHandler>().response.type
                .ShouldEqual(typeof(OutputTypeDescriptions.GetResponse).GetHash());

            Spec.GetEndpoint<OutputTypeDescriptions.PostHandler>().response.type
                .ShouldEqual(typeof(OutputTypeDescriptions.PostResponse).GetHash());

            Spec.GetEndpoint<OutputTypeDescriptions.PutHandler>().response.type
                .ShouldEqual(typeof(OutputTypeDescriptions.PutResponse).GetHash());

            Spec.GetEndpoint<OutputTypeDescriptions.DeleteHandler>().response.type
                .ShouldEqual(typeof(OutputTypeDescriptions.DeleteResponse).GetHash());
        }

        [Test]
        public void should_set_output_type_default_collection_name_and_datatype_should_be_a_hash_of_the_element_type_and_action()
        {
            var response = Spec.GetEndpoint<OutputTypeDescriptions.CollectionPostHandler>().response;

            response.name.ShouldEqual("ArrayOfResponseItem");
            response.comments.ShouldBeNull();
            response.type.ShouldEqual(typeof(OutputTypeDescriptions.ResponseItem).GetHash());
            response.collection.ShouldBeTrue();
        }

        [Test]
        public void should_set_output_type_default_collection_name_of_inherited_collection_and_datatype_should_be_a_hash_of_the_element_type_and_action()
        {
            var response = Spec.GetEndpoint<OutputTypeDescriptions.InheritedCollectionPostHandler>().response;

            response.name.ShouldEqual("ArrayOfResponseItem");
            response.comments.ShouldBeNull();
            response.type.ShouldEqual(typeof(OutputTypeDescriptions.ResponseItem).GetHash());
            response.collection.ShouldBeTrue();
        }

        [Test]
        public void should_set_output_type_name_to_the_xml_type_name()
        {
            var response = Spec.GetEndpoint<OutputTypeDescriptions.OverridenRequestPostHandler>().response;

            response.name.ShouldEqual("NewItemName");
            response.comments.ShouldBeNull();
            response.type.ShouldEqual(typeof(OutputTypeDescriptions.OverridenResponseItem).GetHash());
            response.collection.ShouldBeFalse();
        }

        [Test]
        public void should_set_output_type_collection_name_to_the_xml_type_name()
        {
            var response = Spec.GetEndpoint<OutputTypeDescriptions.OverridenCollectionPostHandler>().response;

            response.name.ShouldEqual("NewCollectionName");
            response.comments.ShouldBeNull();
            response.type.ShouldEqual(typeof(OutputTypeDescriptions.OverridenResponseItem).GetHash());
            response.collection.ShouldBeTrue();
        }
    }
}