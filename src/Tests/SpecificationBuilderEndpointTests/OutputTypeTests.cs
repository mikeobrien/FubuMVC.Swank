using FubuMVC.Swank;
using NUnit.Framework;
using Should;

namespace Tests.SpecificationBuilderEndpointTests
{
    public class OutputTypeTests : TestBase
    {
        [Test]
        public void should_set_get_output_type_description()
        {
            var response = _spec.GetEndpoint<OutputTypeDescriptions.GetHandler>().response;

            response.name.ShouldEqual("GetResponse");
            response.comments.ShouldEqual("Some get response description");
            response.collection.ShouldBeFalse();
        }

        [Test]
        public void should_set_post_output_type_description()
        {
            var response = _spec.GetEndpoint<OutputTypeDescriptions.PostHandler>().response;

            response.name.ShouldEqual("PostResponse");
            response.comments.ShouldEqual("Some post response description");
            response.collection.ShouldBeFalse();
        }

        [Test]
        public void should_set_put_output_type_description()
        {
            var response = _spec.GetEndpoint<OutputTypeDescriptions.PutHandler>().response;

            response.name.ShouldEqual("PutResponse");
            response.comments.ShouldEqual("Some put response description");
            response.collection.ShouldBeFalse();
        }

        [Test]
        public void should_set_delete_output_type_description()
        {
            var response = _spec.GetEndpoint<OutputTypeDescriptions.DeleteHandler>().response;

            response.name.ShouldEqual("DeleteResponse");
            response.comments.ShouldEqual("Some delete response description");
            response.collection.ShouldBeFalse();
        }

        [Test]
        public void should_set_the_datatype_for__output_types_to_a_hash_of_the_datatype()
        {
            _spec.GetEndpoint<OutputTypeDescriptions.GetHandler>().response.type
                .ShouldEqual(typeof(OutputTypeDescriptions.GetResponse).GetHash());

            _spec.GetEndpoint<OutputTypeDescriptions.PostHandler>().response.type
                .ShouldEqual(typeof(OutputTypeDescriptions.PostResponse).GetHash());

            _spec.GetEndpoint<OutputTypeDescriptions.PutHandler>().response.type
                .ShouldEqual(typeof(OutputTypeDescriptions.PutResponse).GetHash());

            _spec.GetEndpoint<OutputTypeDescriptions.DeleteHandler>().response.type
                .ShouldEqual(typeof(OutputTypeDescriptions.DeleteResponse).GetHash());
        }

        [Test]
        public void should_set_output_type_default_collection_name_and_datatype_should_be_a_hash_of_the_element_type_and_action()
        {
            var response = _spec.GetEndpoint<OutputTypeDescriptions.CollectionPostHandler>().response;

            response.name.ShouldEqual("ArrayOfResponseItem");
            response.comments.ShouldBeNull();
            response.type.ShouldEqual(typeof(OutputTypeDescriptions.ResponseItem).GetHash());
            response.collection.ShouldBeTrue();
        }

        [Test]
        public void should_set_output_type_default_collection_name_of_inherited_collection_and_datatype_should_be_a_hash_of_the_element_type_and_action()
        {
            var response = _spec.GetEndpoint<OutputTypeDescriptions.InheritedCollectionPostHandler>().response;

            response.name.ShouldEqual("ArrayOfResponseItem");
            response.comments.ShouldBeNull();
            response.type.ShouldEqual(typeof(OutputTypeDescriptions.ResponseItem).GetHash());
            response.collection.ShouldBeTrue();
        }

        [Test]
        public void should_set_output_type_name_to_the_xml_type_name()
        {
            var response = _spec.GetEndpoint<OutputTypeDescriptions.OverridenRequestPostHandler>().response;

            response.name.ShouldEqual("NewItemName");
            response.comments.ShouldBeNull();
            response.type.ShouldEqual(typeof(OutputTypeDescriptions.OverridenResponseItem).GetHash());
            response.collection.ShouldBeFalse();
        }

        [Test]
        public void should_set_output_type_collection_name_to_the_xml_type_name()
        {
            var response = _spec.GetEndpoint<OutputTypeDescriptions.OverridenCollectionPostHandler>().response;

            response.name.ShouldEqual("NewCollectionName");
            response.comments.ShouldBeNull();
            response.type.ShouldEqual(typeof(OutputTypeDescriptions.OverridenResponseItem).GetHash());
            response.collection.ShouldBeTrue();
        }
    }
}