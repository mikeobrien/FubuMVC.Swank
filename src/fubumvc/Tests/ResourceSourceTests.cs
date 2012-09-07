using System.Linq;
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

        [Test]
        public void should_enumerate_resources_using_default_grouping()
        {
            var endpoints = TestBehaviorGraph.Build().Actions().ToDictionary(x => x.HandlerType, _resourceSource.GetResource);

            endpoints[typeof(AdminAccountGetAllHandler)].ShouldBeType<AdminAccountResource>();
            endpoints[typeof(AdminAccountPostHandler)].ShouldBeType<AdminAccountResource>();
            endpoints[typeof(AdminAccountGetHandler)].ShouldBeType<AdminAccountResource>();
            endpoints[typeof(AdminAccountPutHandler)].ShouldBeType<AdminAccountResource>();
            endpoints[typeof(AdminAccountDeleteHandler)].ShouldBeType<AdminAccountResource>();

            endpoints[typeof(AdminUserGetAllHandler)].ShouldBeType<AdminUserResource>();
            endpoints[typeof(AdminUserPostHandler)].ShouldBeType<AdminUserResource>();
            endpoints[typeof(AdminUserGetHandler)].ShouldBeType<AdminUserResource>();
            endpoints[typeof(AdminUserPutHandler)].ShouldBeType<AdminUserResource>();
            endpoints[typeof(AdminUserDeleteHandler)].ShouldBeType<AdminUserResource>();

            endpoints[typeof(AdminAddressGetAllHandler)].ShouldBeType<AdminAddressResource>();
            endpoints[typeof(AdminAddressPostHandler)].ShouldBeType<AdminAddressResource>();
            endpoints[typeof(AdminAddressGetHandler)].ShouldBeType<AdminAddressResource>();
            endpoints[typeof(AdminAddressPutHandler)].ShouldBeType<AdminAddressResource>();
            endpoints[typeof(AdminAddressDeleteHandler)].ShouldBeType<AdminAddressResource>();
        }

        [Test]
        public void should_enumerate_resources_using_custom_grouping()
        {
            var resourceSource = new ResourceSource(
                new DescriptionSource<Resource>(),
                new ActionSource(TestBehaviorGraph.Build(), ConfigurationDsl.CreateConfig(x => x.AppliesToThisAssembly())),
                new ResourceSourceConfig().GroupBy(x => x.ParentChain().Route.FirstPatternSegment()));
            var endpoints = TestBehaviorGraph.Build().Actions().ToDictionary(x => x.HandlerType, resourceSource.GetResource);

            endpoints[typeof(AdminAccountGetAllHandler)].ShouldBeType<AdminAccountResource>();
            endpoints[typeof(AdminAccountPostHandler)].ShouldBeType<AdminAccountResource>();
            endpoints[typeof(AdminAccountGetHandler)].ShouldBeType<AdminAccountResource>();
            endpoints[typeof(AdminAccountPutHandler)].ShouldBeType<AdminAccountResource>();
            endpoints[typeof(AdminAccountDeleteHandler)].ShouldBeType<AdminAccountResource>();

            endpoints[typeof(AdminUserGetAllHandler)].ShouldBeType<AdminAccountResource>();
            endpoints[typeof(AdminUserPostHandler)].ShouldBeType<AdminAccountResource>();
            endpoints[typeof(AdminUserGetHandler)].ShouldBeType<AdminAccountResource>();
            endpoints[typeof(AdminUserPutHandler)].ShouldBeType<AdminAccountResource>();
            endpoints[typeof(AdminUserDeleteHandler)].ShouldBeType<AdminAccountResource>();

            endpoints[typeof(AdminAddressGetAllHandler)].ShouldBeType<AdminAccountResource>();
            endpoints[typeof(AdminAddressPostHandler)].ShouldBeType<AdminAccountResource>();
            endpoints[typeof(AdminAddressGetHandler)].ShouldBeType<AdminAccountResource>();
            endpoints[typeof(AdminAddressPutHandler)].ShouldBeType<AdminAccountResource>();
            endpoints[typeof(AdminAddressDeleteHandler)].ShouldBeType<AdminAccountResource>();
        }
    }
}