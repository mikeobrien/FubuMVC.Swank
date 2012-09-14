using System;
using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using NUnit.Framework;
using Should;
using Swank;
using Swank.Description;
using Tests.Description.ResourceSourceTests.Administration;
using Tests.Description.ResourceSourceTests.Administration.Users;
using Tests.Description.ResourceSourceTests.Templates;
using ActionSource = Swank.ActionSource;

namespace Tests.Description.ResourceSourceTests
{
    [TestFixture]
    public class Tests
    {
        private IDescriptionSource<ActionCall, ResourceDescription> _resourceSource;

        private static readonly Func<BehaviorGraph> CreateBehaviorGraph = () => Behaviors.BuildGraph()
                .AddAction<TemplateGetAllHandler>("/templates", HttpVerbs.Get)
                .AddAction<TemplatePostHandler>("/templates", HttpVerbs.Post)
                .AddAction<TemplateGetHandler>("/templates/{Id}", HttpVerbs.Get)
                .AddAction<TemplatePutHandler>("/templates/{Id}", HttpVerbs.Put)
                .AddAction<TemplateDeleteHandler>("/templates/{Id}", HttpVerbs.Delete)
                .AddAction<AdminAccountGetAllHandler>("/admin", HttpVerbs.Get)
                .AddAction<AdminAccountPostHandler>("/admin", HttpVerbs.Post)
                .AddAction<AdminAccountGetHandler>("/admin/{Id}", HttpVerbs.Get)
                .AddAction<AdminAccountPutHandler>("/admin/{Id}", HttpVerbs.Put)
                .AddAction<AdminAccountDeleteHandler>("/admin/{Id}", HttpVerbs.Delete)
                .AddAction<AdminUserGetAllHandler>("/admin/users", HttpVerbs.Get)
                .AddAction<AdminUserPostHandler>("/admin/users", HttpVerbs.Post)
                .AddAction<AdminUserGetHandler>("/admin/users/{Id}", HttpVerbs.Get)
                .AddAction<AdminUserPutHandler>("/admin/users/{Id}", HttpVerbs.Put)
                .AddAction<AdminUserDeleteHandler>("/admin/users/{Id}", HttpVerbs.Delete)
                .AddAction<AdminAddressGetAllHandler>("/admin/users/{UserId}/addresses", HttpVerbs.Get)
                .AddAction<AdminAddressGetAllOfTypeHandler>("/admin/users/{UserId}/addresses/{AddressType}", HttpVerbs.Get)
                .AddAction<AdminAddressPostHandler>("/admin/users/{UserId}/addresses", HttpVerbs.Post)
                .AddAction<AdminAddressGetHandler>("/admin/users/{UserId}/addresses/{Id}", HttpVerbs.Get)
                .AddAction<AdminAddressPutHandler>("/admin/users/{UserId}/addresses/{Id}", HttpVerbs.Put)
                .AddAction<AdminAddressDeleteHandler>("/admin/users/{UserId}/addresses/{Id}", HttpVerbs.Delete);

        [SetUp]
        public void Setup()
        {
            _resourceSource = new ResourceSource(
                new MarkerSource<ResourceDescription>(),
                new ActionSource(CreateBehaviorGraph(), ConfigurationDsl.CreateConfig(x => x.AppliesToThisAssembly()
                    .Where(y => y.HandlerType.Namespace.StartsWith(GetType().Namespace)))), 
                new ResourceSourceConfig());
        }

        [Test]
        public void should_find_resource_description_when_an_applies_to_type_is_not_specified()
        {
            var resourceDescription = new AdminAddressResource();
            var action = Behaviors.CreateAction<AdminAddressGetAllHandler>("/admin/address", HttpVerbs.Get);
            _resourceSource.HasDescription(action).ShouldBeTrue();
            var resource = _resourceSource.GetDescription(action);
            resource.ShouldNotBeNull();
            resource.Name.ShouldEqual(resourceDescription.Name);
            resource.Comments.ShouldEqual(resourceDescription.Comments);
        }

        [Test]
        public void should_find_resource_description_when_an_applies_to_type_is_specified()
        {
            var resourceDescription = new AdminUserResource();
            var action = Behaviors.CreateAction<AdminUserGetAllHandler>("/admin/users", HttpVerbs.Get);
            _resourceSource.HasDescription(action).ShouldBeTrue();
            var resource = _resourceSource.GetDescription(action);
            resource.ShouldNotBeNull();
            resource.Name.ShouldEqual(resourceDescription.Name);
            resource.Comments.ShouldEqual(resourceDescription.Comments);
        }

        [Test]
        public void should_not_find_resource_description_when_none_is_specified_in_the_same_namespaces()
        {
            var action = Behaviors.CreateAction<TemplateGetAllHandler>("/templates", HttpVerbs.Get);
            _resourceSource.HasDescription(action).ShouldBeFalse();
            _resourceSource.GetDescription(action).ShouldBeNull();
        }

        [Test]
        public void should_enumerate_resources_using_default_grouping()
        {
            var endpoints = CreateBehaviorGraph().Actions().ToDictionary(x => x.HandlerType, _resourceSource.GetDescription);

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
                new MarkerSource<ResourceDescription>(),
                new ActionSource(CreateBehaviorGraph(), ConfigurationDsl.CreateConfig(x => x.AppliesToThisAssembly())),
                new ResourceSourceConfig().GroupBy(x => x.ParentChain().Route.FirstPatternSegment()));
            var endpoints = CreateBehaviorGraph().Actions().ToDictionary(x => x.HandlerType, resourceSource.GetDescription);

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