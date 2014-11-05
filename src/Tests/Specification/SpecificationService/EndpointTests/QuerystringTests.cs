using System.Linq;
using NUnit.Framework;
using Should;

namespace Tests.Specification.SpecificationService.EndpointTests
{
    public class QuerystringTests : TestBase
    {
        [Test]
        public void should_exclude_auto_bound_properties()
        {
            Spec.GetEndpoint<Querystrings.ImplicitGetHandler>().QuerystringParameters.Any(x => x.Name == "ContentType").ShouldBeFalse();
            Spec.GetEndpoint<Querystrings.ImplicitDeleteHandler>().QuerystringParameters.Any(x => x.Name == "ContentType").ShouldBeFalse();
            Spec.GetEndpoint<Querystrings.ExplicitPostHandler>().QuerystringParameters.Any(x => x.Name == "ContentType").ShouldBeFalse();
            Spec.GetEndpoint<Querystrings.ExplicitPutHandler>().QuerystringParameters.Any(x => x.Name == "ContentType").ShouldBeFalse();
        }

        [Test]
        public void should_hide_parameters_marked_with_the_hide_sttribute()
        {
            Spec.GetEndpoint<Querystrings.ImplicitGetHandler>().QuerystringParameters.Any(x => x.Name == "HiddenParameter").ShouldBeFalse();
            Spec.GetEndpoint<Querystrings.ImplicitDeleteHandler>().QuerystringParameters.Any(x => x.Name == "HiddenParameter").ShouldBeFalse();
            Spec.GetEndpoint<Querystrings.ExplicitPostHandler>().QuerystringParameters.Any(x => x.Name == "HiddenParameter").ShouldBeFalse();
            Spec.GetEndpoint<Querystrings.ExplicitPutHandler>().QuerystringParameters.Any(x => x.Name == "HiddenParameter").ShouldBeFalse();
        }

        [Test]
        public void should_include_implicit_and_explicit_parameters_on_get_and_delete()
        {
            var endpoint = Spec.GetEndpoint<Querystrings.ImplicitGetHandler>();

            endpoint.Url.EndsWith("?NotRequiredParameter={NotRequiredParameter}&NullableId={NullableId}&Revision={Revision}&Revisions={Revisions}&Sort={Sort}").ShouldBeTrue(endpoint.Url);
            endpoint.QuerystringParameters.Count.ShouldEqual(5);
            endpoint.HasQuerystring<Querystrings.Request>(x => x.Sort).ShouldBeTrue();
            endpoint.HasQuerystring<Querystrings.Request>(x => x.Revisions).ShouldBeTrue();
            endpoint.HasQuerystring<Querystrings.Request>(x => x.Revision).ShouldBeTrue();
            endpoint.HasQuerystring<Querystrings.Request>(x => x.NotRequiredParameter).ShouldBeTrue();

            endpoint = Spec.GetEndpoint<Querystrings.ImplicitDeleteHandler>();

            endpoint.Url.EndsWith("?NotRequiredParameter={NotRequiredParameter}&NullableId={NullableId}&Revision={Revision}&Revisions={Revisions}&Sort={Sort}").ShouldBeTrue(endpoint.Url);
            endpoint.QuerystringParameters.Count.ShouldEqual(5);
            endpoint.HasQuerystring<Querystrings.Request>(x => x.Sort).ShouldBeTrue();
            endpoint.HasQuerystring<Querystrings.Request>(x => x.Revisions).ShouldBeTrue();
            endpoint.HasQuerystring<Querystrings.Request>(x => x.Revision).ShouldBeTrue();
            endpoint.HasQuerystring<Querystrings.Request>(x => x.NotRequiredParameter).ShouldBeTrue();
        }

        [Test]
        public void should_only_include_explicit_parameters_on_post_or_put()
        {
            var endpoint = Spec.GetEndpoint<Querystrings.ExplicitPutHandler>();

            endpoint.QuerystringParameters.Count.ShouldEqual(2);
            endpoint.HasQuerystring<Querystrings.Request>(x => x.Sort).ShouldBeTrue();

            endpoint = Spec.GetEndpoint<Querystrings.ExplicitPostHandler>();

            endpoint.QuerystringParameters.Count.ShouldEqual(2);
            endpoint.HasQuerystring<Querystrings.Request>(x => x.Sort).ShouldBeTrue();
        }

        [Test]
        public void should_sort_parameters_by_name()
        {
            var parameters = Spec.GetEndpoint<Querystrings.ImplicitGetHandler>().QuerystringParameters;
            parameters[0].Name.ShouldEqual("NotRequiredParameter");
            parameters[1].Name.ShouldEqual("NullableId");
            parameters[2].Name.ShouldEqual("Revision");
            parameters[3].Name.ShouldEqual("Revisions");
            parameters[4].Name.ShouldEqual("Sort");
        }

        [Test]
        public void should_set_comments_when_there_is_a_comments_attribute()
        {
            var parameter = Spec.GetEndpoint<Querystrings.ImplicitGetHandler>()
                .GetQuerystring<Querystrings.Request>(x => x.Revisions);

            parameter.Name.ShouldEqual("Revisions");
            parameter.Comments.ShouldEqual("These are the revision numbers.");
            parameter.Options.ShouldBeNull();
        }

        [Test]
        public void should_set_comments_default_when_there_is_no_comments_attribute()
        {
            var parameter = Spec.GetEndpoint<Querystrings.ImplicitGetHandler>()
                .GetQuerystring<Querystrings.Request>(x => x.Sort);

            parameter.Name.ShouldEqual("Sort");
            parameter.Comments.ShouldBeNull();
            parameter.Options.ShouldBeNull();
        }

        [Test]
        public void should_indicate_that_multiple_are_allowed_when_the_parameter_is_a_list_type()
        {
            Spec.GetEndpoint<Querystrings.ImplicitGetHandler>()
                .GetQuerystring<Querystrings.Request>(x => x.Revisions).MultipleAllowed.ShouldBeTrue();
        }

        [Test]
        public void should_set_the_element_type_when_multiple_are_allowed()
        {
            Spec.GetEndpoint<Querystrings.ImplicitGetHandler>()
                .GetQuerystring<Querystrings.Request>(x => x.Revisions).Type.ShouldEqual("int");
        }

        [Test]
        public void should_set_the_property_type_when_multiple_are_not_allowed()
        {
            Spec.GetEndpoint<Querystrings.ImplicitGetHandler>()
                .GetQuerystring<Querystrings.Request>(x => x.Sort).Type.ShouldEqual("string");
        }

        [Test]
        public void should_indicate_that_multiple_are_not_allowed_when_the_parameter_is_not_a_list_type()
        {
            Spec.GetEndpoint<Querystrings.ImplicitGetHandler>()
                .GetQuerystring<Querystrings.Request>(x => x.Sort).MultipleAllowed.ShouldBeFalse();
        }

        [Test]
        public void should_indicate_when_the_parameter_is_required()
        {
            var endpoint = Spec.GetEndpoint<Querystrings.ImplicitGetHandler>();

            endpoint.GetQuerystring<Querystrings.Request>(x => x.NotRequiredParameter).Required.ShouldBeFalse();
            endpoint.GetQuerystring<Querystrings.Request>(x => x.NullableId).Required.ShouldBeFalse();
            endpoint.GetQuerystring<Querystrings.Request>(x => x.Sort).Required.ShouldBeTrue();
            endpoint.GetQuerystring<Querystrings.Request>(x => x.Revisions).Required.ShouldBeTrue();
        }

        [Test]
        public void should_indicate_querystring_default_value()
        {
            Spec.GetEndpoint<Querystrings.ImplicitGetHandler>()
                .GetQuerystring<Querystrings.Request>(x => x.Revision).DefaultValue.ShouldEqual("5");
        }

        [Test]
        public void should_indicate_querystring_default_value_with_custom_format()
        {
            Spec = GetSpec(x => x.WithSampleIntegerFormat("0.00"));
            Spec.GetEndpoint<Querystrings.ImplicitGetHandler>()
                .GetQuerystring<Querystrings.Request>(x => x.Revision).DefaultValue.ShouldEqual("5.00");
        }

        [Test]
        public void should_order_querystring_options_by_name_or_value()
        {
            var options = Spec.GetEndpoint<Querystrings.OptionGetHandler>()
                .GetQuerystring<Querystrings.OptionRequest>(x => x.Options).Options;

            options.Options[0].Value.ShouldEqual("Option1");
            options.Options[1].Value.ShouldEqual("Option3");
        }

        [Test]
        public void should_get_querystring_option_description()
        {
            var option = Spec.GetEndpoint<Querystrings.OptionGetHandler>()
                .GetQuerystring<Querystrings.OptionRequest>(x => x.Options).Options.Options[0];

            option.Name.ShouldEqual("Option 1");
            option.Value.ShouldEqual("Option1");
            option.Comments.ShouldEqual("Option 1 description.");
        }

        [Test]
        public void should_set_querystring_option_description_to_default_when_not_specified()
        {
            var option = Spec.GetEndpoint<Querystrings.OptionGetHandler>()
                .GetQuerystring<Querystrings.OptionRequest>(x => x.Options).Options.Options[1];

            option.Name.ShouldEqual("Option3");
            option.Value.ShouldEqual("Option3");
            option.Comments.ShouldBeNull();
        }

        [Test]
        public void should_hide_querystring_options_marked_with_the_hide_attribute()
        {
            Spec.GetEndpoint<Querystrings.OptionGetHandler>()
                .GetQuerystring<Querystrings.OptionRequest>(x => x.Options).Options.Options.Any(x => x.Value == "Option2").ShouldBeFalse();
        }
    }
}