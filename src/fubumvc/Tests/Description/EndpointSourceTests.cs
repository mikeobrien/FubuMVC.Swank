using NUnit.Framework;
using Should;
using Swank.Description;
using Tests.Templates;

namespace Tests.Description
{
    [TestFixture]
    public class EndpointSourceTests
    {
        [Test]
        public void should_pull_description_from_method_attribute()
        {
            var action = TestBehaviorGraph.CreateAction<TemplatePostHandler>();
            var endpointSource = new EndpointSource();
            var description = endpointSource.GetDescription(action);
            description.Name.ShouldEqual("AddTemplate");
            description.Comments.ShouldEqual("This adds a the template yo.");
        }

        [Test]
        public void should_pull_description_from_handler_attribute()
        {
            var action = TestBehaviorGraph.CreateAction<TemplateGetAllHandler>();
            var endpointSource = new EndpointSource();
            var description = endpointSource.GetDescription(action);
            description.Name.ShouldEqual("GetTemplates");
            description.Comments.ShouldEqual("This gets all the templates yo.");
        }

        [Test]
        public void should_pull_description_from_embedded_resource_named_as_handler()
        {
            var action = TestBehaviorGraph.CreateAction<TemplateGetHandler>();
            var endpointSource = new EndpointSource();
            var description = endpointSource.GetDescription(action);
            description.Name.ShouldBeNull();
            description.Comments.ShouldEqual("<b>This gets a template yo!</b>");
        }

        [Test]
        public void should_pull_description_from_embedded_resource_named_as_handler_and_method()
        {
            var action = TestBehaviorGraph.CreateAction<TemplatePutHandler>();
            var endpointSource = new EndpointSource();
            var description = endpointSource.GetDescription(action);
            description.Name.ShouldBeNull();
            description.Comments.ShouldEqual("<p><strong>This updates a template yo!</strong></p>");
        }
    }
}