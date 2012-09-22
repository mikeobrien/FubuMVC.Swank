using System.Linq;
using NUnit.Framework;
using Should;

namespace Tests.SpecificationBuilderEndpointTests
{
    public class UrlParameterTests : TestBase
    {
        [Test]
        public void should_enumerate_url_parameters_ordered_by_position_in_the_url()
        {
            var parameters = _spec.GetEndpoint<UrlParameters.GetHandler>().urlParameters;
                
            parameters.Count.ShouldEqual(2);
            parameters[0].name.ShouldEqual("WidgetId");
            parameters[1].name.ShouldEqual("Revision");
        }

        [Test]
        public void should_set_url_parameter_to_default_when_no_comments_specified()
        {
            var parameter = _spec.GetEndpoint<UrlParameters.GetHandler>()
                .GetUrlParameter<UrlParameters.Request>(x => x.WidgetId);

            parameter.name.ShouldEqual("WidgetId");
            parameter.type.ShouldEqual("uuid");
            parameter.comments.ShouldBeNull();
            parameter.options.ShouldBeEmpty();
        }

        [Test]
        public void should_set_url_parameter_with_comments()
        {
            var parameter = _spec.GetEndpoint<UrlParameters.GetHandler>()
                .GetUrlParameter<UrlParameters.Request>(x => x.Revision);

            parameter.name.ShouldEqual("Revision");
            parameter.type.ShouldEqual("int");
            parameter.comments.ShouldEqual("This the revision number.");
            parameter.options.ShouldBeEmpty();
        }

        [Test]
        public void should_order_url_paramaters_options_by_name_or_value()
        {
            var options = _spec.GetEndpoint<UrlParameters.OptionGetHandler>()
                .GetUrlParameter<UrlParameters.OptionRequest>(x => x.Options).options;

            options[0].value.ShouldEqual("Option1");
            options[1].value.ShouldEqual("Option3");
        }

        [Test]
        public void should_get_url_paramaters_option_description()
        {
            var option = _spec.GetEndpoint<UrlParameters.OptionGetHandler>()
                .GetUrlParameter<UrlParameters.OptionRequest>(x => x.Options).options[0];

            option.name.ShouldEqual("Option 1");
            option.value.ShouldEqual("Option1");
            option.comments.ShouldEqual("Option 1 description.");
        }

        [Test]
        public void should_set_url_paramaters_option_description_to_default_when_not_specified()
        {
            var option = _spec.GetEndpoint<UrlParameters.OptionGetHandler>()
                .GetUrlParameter<UrlParameters.OptionRequest>(x => x.Options).options[1];

            option.name.ShouldBeNull();
            option.value.ShouldEqual("Option3");
            option.comments.ShouldBeNull();
        }

        [Test]
        public void should_hide_url_paramaters_options_marked_with_the_hide_attribute()
        {
            _spec.GetEndpoint<UrlParameters.OptionGetHandler>()
                .GetUrlParameter<UrlParameters.OptionRequest>(x => x.Options)
                .options.Any(x => x.value == "Option2").ShouldBeFalse();
        }
    }
}