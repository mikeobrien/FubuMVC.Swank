using System.Linq;
using NUnit.Framework;
using Should;

namespace Tests.Specification.SpecificationServiceEndpointTests
{
    public class UrlParameterTests : TestBase
    {
        [Test]
        public void should_enumerate_url_parameters_ordered_by_position_in_the_url()
        {
            var parameters = Spec.GetEndpoint<UrlParameters.GetHandler>().UrlParameters;
                
            parameters.Count.ShouldEqual(2);
            parameters[0].Name.ShouldEqual("WidgetId");
            parameters[1].Name.ShouldEqual("Revision");
        }

        [Test]
        public void should_set_url_parameter_to_default_when_no_comments_specified()
        {
            var parameter = Spec.GetEndpoint<UrlParameters.GetHandler>()
                .GetUrlParameter<UrlParameters.Request>(x => x.WidgetId);

            parameter.Name.ShouldEqual("WidgetId");
            parameter.Type.ShouldEqual("uuid");
            parameter.Comments.ShouldBeNull();
            parameter.Options.ShouldBeEmpty();
        }

        [Test]
        public void should_set_url_parameter_with_comments()
        {
            var parameter = Spec.GetEndpoint<UrlParameters.GetHandler>()
                .GetUrlParameter<UrlParameters.Request>(x => x.Revision);

            parameter.Name.ShouldEqual("Revision");
            parameter.Type.ShouldEqual("int");
            parameter.Comments.ShouldEqual("This the revision number.");
            parameter.Options.ShouldBeEmpty();
        }

        [Test]
        public void should_order_url_paramaters_options_by_name_or_value()
        {
            var options = Spec.GetEndpoint<UrlParameters.OptionGetHandler>()
                .GetUrlParameter<UrlParameters.OptionRequest>(x => x.Options).Options;

            options[0].Value.ShouldEqual("Option1");
            options[1].Value.ShouldEqual("Option3");
        }

        [Test]
        public void should_get_url_paramaters_option_description()
        {
            var option = Spec.GetEndpoint<UrlParameters.OptionGetHandler>()
                .GetUrlParameter<UrlParameters.OptionRequest>(x => x.Options).Options[0];

            option.Name.ShouldEqual("Option 1");
            option.Value.ShouldEqual("Option1");
            option.Comments.ShouldEqual("Option 1 description.");
        }

        [Test]
        public void should_set_url_paramaters_option_description_to_default_when_not_specified()
        {
            var option = Spec.GetEndpoint<UrlParameters.OptionGetHandler>()
                .GetUrlParameter<UrlParameters.OptionRequest>(x => x.Options).Options[1];

            option.Name.ShouldBeNull();
            option.Value.ShouldEqual("Option3");
            option.Comments.ShouldBeNull();
        }

        [Test]
        public void should_hide_url_paramaters_options_marked_with_the_hide_attribute()
        {
            Spec.GetEndpoint<UrlParameters.OptionGetHandler>()
                .GetUrlParameter<UrlParameters.OptionRequest>(x => x.Options)
                .Options.Any(x => x.Value == "Option2").ShouldBeFalse();
        }
    }
}