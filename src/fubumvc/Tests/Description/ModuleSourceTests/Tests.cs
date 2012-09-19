using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using NUnit.Framework;
using Should;
using Swank.Description;

namespace Tests.Description.ModuleSourceTests
{
    [TestFixture]
    public class Tests
    {
        public const string SchedulesModuleComments = "<p><strong>These are schedules yo!</strong></p>";
        private IDescriptionSource<ActionCall, ModuleDescription> _moduleSource;
        private BehaviorGraph _graph;

        [SetUp]
        public void Setup()
        {
            _moduleSource = new ModuleSource(new MarkerSource<ModuleDescription>());
            _graph = Behaviors.BuildGraph().AddActionsInThisNamespace();
        }

        [Test]
        public void should_set_description_to_default_when_none_is_specified()
        {
            var module = _moduleSource.GetDescription(_graph.GetAction<ModuleDescriptions.NoDescription.GetHandler>());

            module.Name.ShouldBeNull();
            module.Comments.ShouldBeNull();
        }

        [Test]
        public void should_set_description_when_one_is_specified()
        {
            var module = _moduleSource.GetDescription(_graph.GetAction<ModuleDescriptions.Description.GetHandler>());

            module.Name.ShouldEqual("Some Module");
            module.Comments.ShouldEqual("Some comments.");
        }

        [Test]
        public void should_set_description_and_text_embedded_resource_comments_when_specified()
        {
            var module = _moduleSource.GetDescription(_graph.GetAction<ModuleDescriptions.EmbeddedTextComments.GetHandler>());

            module.Name.ShouldEqual("Some Text Module");
            module.Comments.ShouldEqual("<b>Some text comments</b>");
        }

        [Test]
        public void should_set_description_and_markdown_embedded_resource_comments_when_specified()
        {
            var module = _moduleSource.GetDescription(_graph.GetAction<ModuleDescriptions.EmbeddedMarkdownComments.GetHandler>());

            module.Name.ShouldEqual("Some Markdown Module");
            module.Comments.ShouldEqual("<p><strong>Some markdown comments</strong></p>");
        }

        [Test]
        public void should_set_description_to_parent()
        {
            var module = _moduleSource.GetDescription(_graph.GetAction<NestedModules.NoModules.GetHandler>());

            module.Name.ShouldEqual("Root Module");
            module.Comments.ShouldBeNull();
        }

        [Test]
        public void should_set_description_to_closest_parent()
        {
            var module = _moduleSource.GetDescription(_graph.GetAction<NestedModules.NestedModule.GetHandler>());

            module.Name.ShouldEqual("Nested Module");
            module.Comments.ShouldBeNull();
        }
    }
}