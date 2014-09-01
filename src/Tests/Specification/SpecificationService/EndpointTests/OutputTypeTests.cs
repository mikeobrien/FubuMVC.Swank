using System;
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
            Spec.GetEndpoint<OutputTypeDescriptions.GetHandler>().Response
                .Comments.ShouldEqual("Some get response description");
        }

        [Test]
        public void should_set_post_output_type_description()
        {
            Spec.GetEndpoint<OutputTypeDescriptions.PostHandler>().Response
                .Comments.ShouldEqual("Some post response description");
        }

        [Test]
        public void should_set_put_output_type_description()
        {
            Spec.GetEndpoint<OutputTypeDescriptions.PutHandler>().Response
                .Comments.ShouldEqual("Some put response description");
        }

        [Test]
        public void should_set_delete_output_type_description()
        {
            Spec.GetEndpoint<OutputTypeDescriptions.DeleteHandler>().Response
                .Comments.ShouldEqual("Some delete response description");
        }

        [Test]
        public void should_set_the_name_for_output_types()
        {
            Spec.GetEndpoint<OutputTypeDescriptions.GetHandler>().Response.Description[0].Name.ShouldEqual("GetResponse");
            Spec.GetEndpoint<OutputTypeDescriptions.PostHandler>().Response.Description[0].Name.ShouldEqual("PostResponse");
            Spec.GetEndpoint<OutputTypeDescriptions.PutHandler>().Response.Description[0].Name.ShouldEqual("PutResponse");
            Spec.GetEndpoint<OutputTypeDescriptions.DeleteHandler>().Response.Description[0].Name.ShouldEqual("DeleteResponse");
        }

        [Test]
        public void should_set_output_type_default_collection_name_and_datatype()
        {
            var response = Spec.GetEndpoint<OutputTypeDescriptions.CollectionPostHandler>().Response;

            response.Comments.ShouldBeNull();
            response.Description[0].Name.ShouldEqual("ArrayOfResponseItem");
            response.Description[0].IsArray.ShouldEqual(true);
        }

        [Test]
        public void should_set_output_type_default_collection_name_of_inherited_collection_and_datatype()
        {
            var response = Spec.GetEndpoint<OutputTypeDescriptions.InheritedCollectionPostHandler>().Response;

            response.Comments.ShouldBeNull();
            response.Description[0].Name.ShouldEqual("ArrayOfResponseItem");
            response.Description[0].IsArray.ShouldEqual(true);
        }

        [Test]
        public void should_set_output_type_name_to_the_xml_type_name()
        {
            var response = Spec.GetEndpoint<OutputTypeDescriptions.OverridenRequestPostHandler>().Response;

            response.Comments.ShouldBeNull();
            response.Description[0].Name.ShouldEqual("NewItemName");
        }

        [Test]
        public void should_set_output_type_collection_name_to_the_xml_type_name()
        {
            var response = Spec.GetEndpoint<OutputTypeDescriptions.OverridenCollectionPostHandler>().Response;

            response.Comments.ShouldBeNull();
            response.Description[0].Name.ShouldEqual("NewCollectionName");
            response.Description[0].IsArray.ShouldEqual(true);
        }
    }
}