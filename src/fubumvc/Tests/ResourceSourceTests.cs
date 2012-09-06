using NUnit.Framework;
using Should;
using Swank;
using Tests.Administration.Users;
using Tests.Templates;

namespace Tests
{
    [TestFixture]
    public class ResourceSourceTests
    {
        private IResourceSource _resourceSource;

        [SetUp]
        public void Setup()
        {
            _resourceSource = new ResourceSource(
                new DescriptionSource<Resource>(), 
                new ActionSource(TestBehaviorGraph.Build(), ConfigurationDsl.CreateConfig(x => x.AppliesToThisAssembly())), 
                new ResourceSourceConfig());
        }

        [Test]
        public void should_find_resource_description_when_an_applies_to_type_is_not_specified()
        {
            var resourceDescription = new AdminAddressResource();
            var action = TestBehaviorGraph.CreateAction<AdminAddressGetAllHandler>();
            _resourceSource.HasResource(action).ShouldBeTrue();
            var resource = _resourceSource.GetResource(action);
            resource.ShouldNotBeNull();
            resource.Name.ShouldEqual(resourceDescription.Name);
            resource.Comments.ShouldEqual(resourceDescription.Comments);
        }

        [Test]
        public void should_find_resource_description_when_an_applies_to_type_is_specified()
        {
            var resourceDescription = new AdminUserResource();
            var action = TestBehaviorGraph.CreateAction<AdminUserGetAllHandler>();
            _resourceSource.HasResource(action).ShouldBeTrue();
            var resource = _resourceSource.GetResource(action);
            resource.ShouldNotBeNull();
            resource.Name.ShouldEqual(resourceDescription.Name);
            resource.Comments.ShouldEqual(resourceDescription.Comments);
        }

        [Test]
        public void should_not_find_resource_description_when_none_is_specified_in_the_same_namespaces()
        {
            var action = TestBehaviorGraph.CreateAction<TemplateGetAllHandler>();
            _resourceSource.HasResource(action).ShouldBeFalse();
            _resourceSource.GetResource(action).ShouldBeNull();
        }
    }
}