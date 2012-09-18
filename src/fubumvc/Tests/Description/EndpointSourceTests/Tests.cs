using FubuMVC.Core.Registration;
using NUnit.Framework;
using Should;
using Swank.Description;
using Tests.Description.EndpointSourceTests.Templates;

namespace Tests.Description.EndpointSourceTests
{
    [TestFixture]
    public class Tests
    {
        private BehaviorGraph _graph;

        [SetUp]
        public void Setup()
        {
            _graph = Behaviors.BuildGraph().AddActionsInThisNamespace();
        }

        [Test]
        public void should_pull_description_from_method_attribute()
        {
            var action = _graph.GetAction<TemplatePostHandler>();
            var endpointSource = new EndpointSource();
            var description = endpointSource.GetDescription(action);
            description.Name.ShouldEqual("AddTemplate");
            description.Comments.ShouldEqual("This adds a the template yo.");
        }

        [Test]
        public void should_pull_description_from_handler_attribute()
        {
            var action = _graph.GetAction<TemplateAllGetHandler>();
            var endpointSource = new EndpointSource();
            var description = endpointSource.GetDescription(action);
            description.Name.ShouldEqual("GetTemplates");
            description.Comments.ShouldEqual("This gets all the templates yo.");
        }

        [Test]
        public void should_pull_description_from_embedded_resource_named_as_handler()
        {
            var action = _graph.GetAction<TemplateGetHandler>();
            var endpointSource = new EndpointSource();
            var description = endpointSource.GetDescription(action);
            description.Name.ShouldBeNull();
            description.Comments.ShouldEqual("<b>This gets a template yo!</b>");
        }

        [Test]
        public void should_pull_description_from_embedded_resource_named_as_handler_and_method()
        {
            var action = _graph.GetAction<TemplatePutHandler>();
            var endpointSource = new EndpointSource();
            var description = endpointSource.GetDescription(action);
            description.Name.ShouldBeNull();
            description.Comments.ShouldEqual("<p><strong>This updates a template yo!</strong></p>");
        }

        [Test]
        public void should_not_pull_description_from_embedded_resource_named_as_handler_when_handler_has_resource_attribute()
        {
            var action = _graph.GetAction<ControllerResource.Controller>();
            var endpointSource = new EndpointSource();
            var description = endpointSource.GetDescription(action);
            description.Name.ShouldBeNull();
            description.Comments.ShouldBeNull();
        }
    }
}