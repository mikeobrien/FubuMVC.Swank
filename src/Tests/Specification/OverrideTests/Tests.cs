using System;
using System.Linq;
using System.Reflection;
using FubuCore.Reflection;
using FubuMVC.Core.Registration;
using FubuMVC.Swank;
using FubuMVC.Swank.Description;
using FubuMVC.Swank.Specification;
using NUnit.Framework;
using Should;
using ActionSource = FubuMVC.Swank.Specification.ActionSource;

namespace Tests.Specification.OverrideTests
{
    [TestFixture]
    public class Tests
    {
        private FubuMVC.Swank.Specification.Specification BuildSpec<TNamespace>(Action<Swank> configure = null)
        {
            var graph = Behavior.BuildGraph().AddActionsInThisNamespace();
            var moduleConvention = new ModuleConvention(new MarkerConvention<ModuleDescription>());
            var resourceConvention = new ResourceConvention(
                new MarkerConvention<ResourceDescription>(),
                new ActionSource(graph,
                    Swank.CreateConfig(x => x.AppliesToThisAssembly()
                        .Where(y => y.HandlerType.InNamespace<SpecificationServiceModuleTests.Tests>()))));
            var configuration = Swank.CreateConfig(x =>
            { if (configure != null) configure(x); x.AppliesToThisAssembly().Where(y => y.HandlerType.InNamespace<TNamespace>()); });
            return new SpecificationService(configuration, new ActionSource(graph, configuration), new TypeDescriptorCache(),
                moduleConvention, resourceConvention, new EndpointConvention(), new MemberConvention(), new OptionConvention(), new StatusCodeConvention(),
                new HeaderConvention(), new TypeConvention(), new MergeService()).Generate();
        }

        [Test]
        public void should_override_module()
        {
            var spec = BuildSpec<Handlers.GetHandler>(x => x
                .OverrideModules(y => y.Name = y.Name + "1")
                .OverrideModules(y => y.Comments = y.Comments + "2")
                .OverrideModulesWhen(y => y.Comments = y.Comments + "3", y => y.Comments.EndsWith("2"))
                .OverrideModulesWhen(y => y.Comments = y.Comments + "4", y => y.Comments.EndsWith("2")));

            spec.Modules[0].Name.ShouldEqual("SomeName1");
            spec.Modules[0].Comments.ShouldEqual("Some comments23");
        }

        [Test]
        public void should_override_resource()
        {
            var spec = BuildSpec<Handlers.GetHandler>(x => x
                .OverrideResources(y => y.Name = y.Name + "1")
                .OverrideResources(y => y.Comments = y.Comments + "2")
                .OverrideResourcesWhen(y => y.Comments = y.Comments + "3", y => y.Comments.EndsWith("2"))
                .OverrideResourcesWhen(y => y.Comments = y.Comments + "4", y => y.Comments.EndsWith("2")));

            spec.Modules[0].Resources[0].Name.ShouldEqual("SomeName1");
            spec.Modules[0].Resources[0].Comments.ShouldEqual("Some comments23");
        }

        [Test]
        public void should_override_endpoint()
        {
            var spec = BuildSpec<Handlers.GetHandler>(x => x
                .OverrideEndpoints((y, z) => z.Name = z.Name + "1")
                .OverrideEndpoints((y, z) => z.Comments = z.Comments + "2")
                .OverrideEndpointsWhen((y, z) => z.Comments = z.Comments + "3", (y, z) => z.Comments.EndsWith("2"))
                .OverrideEndpointsWhen((y, z) => z.Comments = z.Comments + "4", (y, z) => z.Comments.EndsWith("2")));

            spec.Modules[0].Resources[0].Endpoints[1].Name.ShouldEqual("SomeName1");
            spec.Modules[0].Resources[0].Endpoints[1].Comments.ShouldEqual("Some comments23");
        }

        [Test]
        public void should_override_type()
        {
            var spec = BuildSpec<Handlers.GetHandler>(x => x
                .OverrideTypes((y, z) => z.Name = z.Name + "1")
                .OverrideTypes((y, z) => z.Comments = z.Comments + "2")
                .OverrideTypesWhen((y, z) => z.Comments = z.Comments + "3", (y, z) => z.Comments.EndsWith("2"))
                .OverrideTypesWhen((y, z) => z.Comments = z.Comments + "4", (y, z) => z.Comments.EndsWith("2")));

            spec.Types[0].Name.ShouldEqual("Data1");
            spec.Types[0].Comments.ShouldEqual("Some comments23");
        }

        [Test]
        public void should_override_member()
        {
            var spec = BuildSpec<Handlers.GetHandler>(x => x
                .OverrideMembers((y, z) => z.Name = z.Name + "1")
                .OverrideMembers((y, z) => z.Comments = z.Comments + "2")
                .OverrideMembersWhen((y, z) => z.Comments = z.Comments + "3", (y, z) => z.Comments.EndsWith("2"))
                .OverrideMembersWhen((y, z) => z.Comments = z.Comments + "4", (y, z) => z.Comments.EndsWith("2")));

            spec.Types[0].Members[0].Name.ShouldEqual("Id1");
            spec.Types[0].Members[0].Comments.ShouldEqual("Some comments23");
        }

        [Test]
        public void should_override_option()
        {
            var spec = BuildSpec<Handlers.GetHandler>(x => x
                .OverrideOptions((y, z) => z.Name = z.Name + "1")
                .OverrideOptions((y, z) => z.Comments = z.Comments + "2")
                .OverrideOptionsWhen((y, z) => z.Comments = z.Comments + "3", (y, z) => z.Comments.EndsWith("2"))
                .OverrideOptionsWhen((y, z) => z.Comments = z.Comments + "4", (y, z) => z.Comments.EndsWith("2")));

            spec.Types[0].Members[0].Options[0].Name.ShouldEqual("SomeName1");
            spec.Types[0].Members[0].Options[0].Comments.ShouldEqual("Some comments23");
        }

        [Test]
        public void should_override_url_parameters()
        {
            var spec = BuildSpec<Handlers.GetHandler>(x => x
                .OverrideUrlParameters((a, b , c) => c.Name = c.Name + "1")
                .OverrideUrlParameters((a, b, c) => c.Comments = c.Comments + "2")
                .OverrideUrlParametersWhen((a, b, c) => c.Comments = c.Comments + "3", (a, b, c) => c.Comments.EndsWith("2"))
                .OverrideUrlParametersWhen((a, b, c) => c.Comments = c.Comments + "4", (a, b, c) => c.Comments.EndsWith("2")));

            spec.Modules[0].Resources[0].Endpoints[1].UrlParameters[0].Name.ShouldEqual("Id1");
            spec.Modules[0].Resources[0].Endpoints[1].UrlParameters[0].Comments.ShouldEqual("Some comments23");
        }

        [Test]
        public void should_override_querystring()
        {
            var spec = BuildSpec<Handlers.GetHandler>(x => x
                .OverrideQuerystring((a, b, c) => c.Name = c.Name + "1")
                .OverrideQuerystring((a, b, c) => c.Comments = c.Comments + "2")
                .OverrideQuerystringWhen((a, b, c) => c.Comments = c.Comments + "3", (a, b, c) => c.Comments.EndsWith("2"))
                .OverrideQuerystringWhen((a, b, c) => c.Comments = c.Comments + "4", (a, b, c) => c.Comments.EndsWith("2")));

            spec.Modules[0].Resources[0].Endpoints[1].QuerystringParameters[0].Name.ShouldEqual("Sort1");
            spec.Modules[0].Resources[0].Endpoints[1].QuerystringParameters[0].Comments.ShouldEqual("Some comments23");
        }

        [Test]
        public void should_override_request()
        {
            var spec = BuildSpec<Handlers.PostHandler>(x => x
                .OverrideRequest((a, b) => b.Name = b.Name + "1")
                .OverrideRequest((a, b) => b.Comments = b.Comments + "2")
                .OverrideRequestWhen((a, b) => b.Comments = b.Comments + "3", (a, b) => b.Comments.EndsWith("2"))
                .OverrideRequestWhen((a, b) => b.Comments = b.Comments + "4", (a, b) => b.Comments.EndsWith("2")));

            spec.Modules[0].Resources[0].Endpoints[0].Request.Name.ShouldEqual("Data1");
            spec.Modules[0].Resources[0].Endpoints[0].Request.Comments.ShouldEqual("Some comments23");
        }

        [Test]
        public void should_override_response()
        {
            var spec = BuildSpec<Handlers.GetHandler>(x => x
                .OverrideResponse((a, b) => b.Name = b.Name + "1")
                .OverrideResponse((a, b) => b.Comments = b.Comments + "2")
                .OverrideResponseWhen((a, b) => b.Comments = b.Comments + "3", (a, b) => b.Comments.EndsWith("2"))
                .OverrideResponseWhen((a, b) => b.Comments = b.Comments + "4", (a, b) => b.Comments.EndsWith("2")));

            spec.Modules[0].Resources[0].Endpoints[1].Response.Name.ShouldEqual("Data1");
            spec.Modules[0].Resources[0].Endpoints[1].Response.Comments.ShouldEqual("Some comments23");
        }

        [Test]
        public void should_override_status_codes()
        {
            var spec = BuildSpec<Handlers.GetHandler>(x => x
                .OverrideStatusCodes((a, b) => b.Name = b.Name + "1")
                .OverrideStatusCodes((a, b) => b.Comments = b.Comments + "2")
                .OverrideStatusCodesWhen((a, b) => b.Comments = b.Comments + "3", (a, b) => b.Comments.EndsWith("2"))
                .OverrideStatusCodesWhen((a, b) => b.Comments = b.Comments + "4", (a, b) => b.Comments.EndsWith("2")));

            spec.Modules[0].Resources[0].Endpoints[1].StatusCodes[0].Name.ShouldEqual("SomeName1");
            spec.Modules[0].Resources[0].Endpoints[1].StatusCodes[0].Comments.ShouldEqual("Some comments23");
        }

        [Test]
        public void should_override_headers()
        {
            var spec = BuildSpec<Handlers.GetHandler>(x => x
                .OverrideHeaders((a, b) => b.Name = b.Name + "1")
                .OverrideHeaders((a, b) => b.Comments = b.Comments + "2")
                .OverrideHeadersWhen((a, b) => b.Comments = b.Comments + "3", (a, b) => b.Comments.EndsWith("2"))
                .OverrideHeadersWhen((a, b) => b.Comments = b.Comments + "4", (a, b) => b.Comments.EndsWith("2")));

            spec.Modules[0].Resources[0].Endpoints[1].Headers[0].Name.ShouldEqual("SomeName1");
            spec.Modules[0].Resources[0].Endpoints[1].Headers[0].Comments.ShouldEqual("Some comments23");
        }
    }
}