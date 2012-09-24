using System.Linq;
using NUnit.Framework;
using Should;

namespace Tests.Specification.SpecificationServiceEndpointTests
{
    public class QuerystringTests : TestBase
    {
        [Test]
        public void should_exclude_auto_bound_properties()
        {
            Spec.GetEndpoint<Querystrings.ImplicitGetHandler>().querystringParameters.Any(x => x.name == "ContentType").ShouldBeFalse();
            Spec.GetEndpoint<Querystrings.ImplicitDeleteHandler>().querystringParameters.Any(x => x.name == "ContentType").ShouldBeFalse();
            Spec.GetEndpoint<Querystrings.ExplicitPostHandler>().querystringParameters.Any(x => x.name == "ContentType").ShouldBeFalse();
            Spec.GetEndpoint<Querystrings.ExplicitPutHandler>().querystringParameters.Any(x => x.name == "ContentType").ShouldBeFalse();
        }

        [Test]
        public void should_hide_parameters_marked_with_the_hide_sttribute()
        {
            Spec.GetEndpoint<Querystrings.ImplicitGetHandler>().querystringParameters.Any(x => x.name == "HiddenParameter").ShouldBeFalse();
            Spec.GetEndpoint<Querystrings.ImplicitDeleteHandler>().querystringParameters.Any(x => x.name == "HiddenParameter").ShouldBeFalse();
            Spec.GetEndpoint<Querystrings.ExplicitPostHandler>().querystringParameters.Any(x => x.name == "HiddenParameter").ShouldBeFalse();
            Spec.GetEndpoint<Querystrings.ExplicitPutHandler>().querystringParameters.Any(x => x.name == "HiddenParameter").ShouldBeFalse();
        }

        [Test]
        public void should_include_implicit_and_explicit_parameters_on_get_and_delete()
        {
            var endpoint = Spec.GetEndpoint<Querystrings.ImplicitGetHandler>();

            endpoint.querystringParameters.Count.ShouldEqual(3);
            endpoint.HasQuerystring<Querystrings.Request>(x => x.Sort).ShouldBeTrue();
            endpoint.HasQuerystring<Querystrings.Request>(x => x.Sort).ShouldBeTrue();

            endpoint = Spec.GetEndpoint<Querystrings.ImplicitDeleteHandler>();

            endpoint.querystringParameters.Count.ShouldEqual(3);
            endpoint.HasQuerystring<Querystrings.Request>(x => x.Sort).ShouldBeTrue();
            endpoint.HasQuerystring<Querystrings.Request>(x => x.Sort).ShouldBeTrue();
        }

        [Test]
        public void should_only_include_explicit_parameters_on_post_or_put()
        {
            var endpoint = Spec.GetEndpoint<Querystrings.ExplicitPutHandler>();

            endpoint.querystringParameters.Count.ShouldEqual(2);
            endpoint.HasQuerystring<Querystrings.Request>(x => x.Sort).ShouldBeTrue();

            endpoint = Spec.GetEndpoint<Querystrings.ExplicitPostHandler>();

            endpoint.querystringParameters.Count.ShouldEqual(2);
            endpoint.HasQuerystring<Querystrings.Request>(x => x.Sort).ShouldBeTrue();
        }

        [Test]
        public void should_sort_parameters_by_name()
        {
            var parameters = Spec.GetEndpoint<Querystrings.ImplicitGetHandler>().querystringParameters;
            parameters[0].name.ShouldEqual("RequiredParameter");
            parameters[1].name.ShouldEqual("Revision");
            parameters[2].name.ShouldEqual("Sort");
        }

        [Test]
        public void should_set_comments_when_there_is_a_comments_attribute()
        {
            var parameter = Spec.GetEndpoint<Querystrings.ImplicitGetHandler>()
                .GetQuerystring<Querystrings.Request>(x => x.Revision);

            parameter.name.ShouldEqual("Revision");
            parameter.comments.ShouldEqual("These are the revision numbers.");
            parameter.options.ShouldBeEmpty();
        }

        [Test]
        public void should_set_comments_default_when_there_is_no_comments_attribute()
        {
            var parameter = Spec.GetEndpoint<Querystrings.ImplicitGetHandler>()
                .GetQuerystring<Querystrings.Request>(x => x.Sort);

            parameter.name.ShouldEqual("Sort");
            parameter.comments.ShouldBeNull();
            parameter.options.ShouldBeEmpty();
        }

        [Test]
        public void should_indicate_that_multiple_are_allowed_when_the_parameter_is_a_list_type()
        {
            Spec.GetEndpoint<Querystrings.ImplicitGetHandler>()
                .GetQuerystring<Querystrings.Request>(x => x.Revision).multipleAllowed.ShouldBeTrue();
        }

        [Test]
        public void should_set_the_element_type_when_multiple_are_allowed()
        {
            Spec.GetEndpoint<Querystrings.ImplicitGetHandler>()
                .GetQuerystring<Querystrings.Request>(x => x.Revision).type.ShouldEqual("int");
        }

        [Test]
        public void should_set_the_property_type_when_multiple_are_not_allowed()
        {
            Spec.GetEndpoint<Querystrings.ImplicitGetHandler>()
                .GetQuerystring<Querystrings.Request>(x => x.Sort).type.ShouldEqual("string");
        }

        [Test]
        public void should_indicate_that_multiple_are_not_allowed_when_the_parameter_is_not_a_list_type()
        {
            Spec.GetEndpoint<Querystrings.ImplicitGetHandler>()
                .GetQuerystring<Querystrings.Request>(x => x.Sort).multipleAllowed.ShouldBeFalse();
        }

        [Test]
        public void should_indicate_when_the_parameter_is_required()
        {
            var endpoint = Spec.GetEndpoint<Querystrings.ImplicitGetHandler>();
            
            endpoint.GetQuerystring<Querystrings.Request>(x => x.RequiredParameter).required.ShouldBeTrue();
            endpoint.GetQuerystring<Querystrings.Request>(x => x.Sort).required.ShouldBeFalse();
            endpoint.GetQuerystring<Querystrings.Request>(x => x.Revision).required.ShouldBeFalse();
        }

        [Test]
        public void should_order_querystring_options_by_name_or_value()
        {
            var options = Spec.GetEndpoint<Querystrings.OptionGetHandler>()
                .GetQuerystring<Querystrings.OptionRequest>(x => x.Options).options;

            options[0].value.ShouldEqual("Option1");
            options[1].value.ShouldEqual("Option3");
        }

        [Test]
        public void should_get_querystring_option_description()
        {
            var option = Spec.GetEndpoint<Querystrings.OptionGetHandler>()
                .GetQuerystring<Querystrings.OptionRequest>(x => x.Options).options[0];

            option.name.ShouldEqual("Option 1");
            option.value.ShouldEqual("Option1");
            option.comments.ShouldEqual("Option 1 description.");
        }

        [Test]
        public void should_set_querystring_option_description_to_default_when_not_specified()
        {
            var option = Spec.GetEndpoint<Querystrings.OptionGetHandler>()
                .GetQuerystring<Querystrings.OptionRequest>(x => x.Options).options[1];

            option.name.ShouldBeNull();
            option.value.ShouldEqual("Option3");
            option.comments.ShouldBeNull();
        }

        [Test]
        public void should_hide_querystring_options_marked_with_the_hide_attribute()
        {
            Spec.GetEndpoint<Querystrings.OptionGetHandler>()
                .GetQuerystring<Querystrings.OptionRequest>(x => x.Options).options.Any(x => x.value == "Option2").ShouldBeFalse();
        }
    }
}