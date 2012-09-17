using System.Linq;
using NUnit.Framework;
using Should;

namespace Tests.SpecificationBuilderEndpointTests
{
    public class QuerystringTests : TestBase
    {
        [Test]
        public void should_exclude_auto_bound_properties()
        {
            _spec.GetEndpoint<Querystrings.ImplicitGetHandler>().querystringParameters.Any(x => x.name == "ContentType").ShouldBeFalse();
            _spec.GetEndpoint<Querystrings.ImplicitDeleteHandler>().querystringParameters.Any(x => x.name == "ContentType").ShouldBeFalse();
            _spec.GetEndpoint<Querystrings.ExplicitPostHandler>().querystringParameters.Any(x => x.name == "ContentType").ShouldBeFalse();
            _spec.GetEndpoint<Querystrings.ExplicitPutHandler>().querystringParameters.Any(x => x.name == "ContentType").ShouldBeFalse();
        }

        [Test]
        public void should_hide_parameters_marked_with_the_hide_sttribute()
        {
            _spec.GetEndpoint<Querystrings.ImplicitGetHandler>().querystringParameters.Any(x => x.name == "HiddenParameter").ShouldBeFalse();
            _spec.GetEndpoint<Querystrings.ImplicitDeleteHandler>().querystringParameters.Any(x => x.name == "HiddenParameter").ShouldBeFalse();
            _spec.GetEndpoint<Querystrings.ExplicitPostHandler>().querystringParameters.Any(x => x.name == "HiddenParameter").ShouldBeFalse();
            _spec.GetEndpoint<Querystrings.ExplicitPutHandler>().querystringParameters.Any(x => x.name == "HiddenParameter").ShouldBeFalse();
        }

        [Test]
        public void should_include_implicit_and_explicit_parameters_on_get_and_delete()
        {
            var parameters = _spec.GetEndpoint<Querystrings.ImplicitGetHandler>().querystringParameters;

            parameters.Count.ShouldEqual(2);
            parameters.Any(x => x.name == "Revision").ShouldBeTrue();
            parameters.Any(x => x.name == "Sort").ShouldBeTrue();

            parameters = _spec.GetEndpoint<Querystrings.ImplicitDeleteHandler>().querystringParameters;

            parameters.Count.ShouldEqual(2);
            parameters.Any(x => x.name == "Revision").ShouldBeTrue();
            parameters.Any(x => x.name == "Sort").ShouldBeTrue();
        }

        [Test]
        public void should_only_include_explicit_parameters_on_post_or_put()
        {
            var parameters = _spec.GetEndpoint<Querystrings.ExplicitPutHandler>().querystringParameters;

            parameters.Count.ShouldEqual(1);
            parameters.Any(x => x.name == "Sort").ShouldBeTrue();

            parameters = _spec.GetEndpoint<Querystrings.ExplicitPostHandler>().querystringParameters;

            parameters.Count.ShouldEqual(1);
            parameters.Any(x => x.name == "Sort").ShouldBeTrue();
        }

        [Test]
        public void should_sort_parameters_by_name()
        {
            var parameters = _spec.GetEndpoint<Querystrings.ImplicitGetHandler>().querystringParameters;
            parameters[0].name.ShouldEqual("Revision");
            parameters[1].name.ShouldEqual("Sort");
        }

        [Test]
        public void should_set_comments_when_there_is_a_comments_attribute()
        {
            var parameter = _spec.GetEndpoint<Querystrings.ImplicitGetHandler>().querystringParameters[0];

            parameter.name.ShouldEqual("Revision");
            parameter.comments.ShouldEqual("These are the revision numbers.");
            parameter.options.ShouldBeEmpty();
        }

        [Test]
        public void should_set_comments_default_when_there_is_no_comments_attribute()
        {
            var parameter = _spec.GetEndpoint<Querystrings.ImplicitGetHandler>().querystringParameters[1];

            parameter.name.ShouldEqual("Sort");
            parameter.comments.ShouldBeNull();
            parameter.options.ShouldBeEmpty();
        }

        [Test]
        public void should_indicate_that_multiple_are_allowed_when_the_parameter_is_a_list_type()
        {
            _spec.GetEndpoint<Querystrings.ImplicitGetHandler>().querystringParameters[0].multipleAllowed.ShouldBeTrue();
        }

        [Test]
        public void should_set_the_element_type_when_multiple_are_allowed()
        {
            _spec.GetEndpoint<Querystrings.ImplicitGetHandler>().querystringParameters[0].dataType.ShouldEqual("int");
        }

        [Test]
        public void should_set_the_property_type_when_multiple_are_not_allowed()
        {
            _spec.GetEndpoint<Querystrings.ImplicitGetHandler>().querystringParameters[1].dataType.ShouldEqual("string");
        }

        [Test]
        public void should_indicate_that_multiple_are_not_allowed_when_the_parameter_is_not_a_list_type()
        {
            _spec.GetEndpoint<Querystrings.ImplicitGetHandler>().querystringParameters[1].multipleAllowed.ShouldBeFalse();
        }

        [Test]
        public void should_order_querystring_options_by_name_or_value()
        {
            var options = _spec.GetEndpoint<Querystrings.OptionGetHandler>().querystringParameters[0].options;

            options[0].value.ShouldEqual("Option1");
            options[1].value.ShouldEqual("Option3");
        }

        [Test]
        public void should_get_querystring_option_description()
        {
            var option = _spec.GetEndpoint<Querystrings.OptionGetHandler>().querystringParameters[0].options[0];

            option.name.ShouldEqual("Option 1");
            option.value.ShouldEqual("Option1");
            option.comments.ShouldEqual("Option 1 description.");
        }

        [Test]
        public void should_set_querystring_option_description_to_default_when_not_specified()
        {
            var option = _spec.GetEndpoint<Querystrings.OptionGetHandler>().querystringParameters[0].options[1];

            option.name.ShouldBeNull();
            option.value.ShouldEqual("Option3");
            option.comments.ShouldBeNull();
        }

        [Test]
        public void should_hide_querystring_options_marked_with_the_hide_attribute()
        {
            _spec.GetEndpoint<Querystrings.OptionGetHandler>().querystringParameters[0].options.Any(x => x.value == "Option2").ShouldBeFalse();
        }
    }
}