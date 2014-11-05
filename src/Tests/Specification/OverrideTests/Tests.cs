using NUnit.Framework;
using Should;
using Tests.Specification.SpecificationService.Tests;

namespace Tests.Specification.OverrideTests
{
    [TestFixture]
    public class Tests : InteractionContext
    {
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

            var request = spec.Modules[0].Resources[0].Endpoints[0].Request.Body.Description[0];
            request.Name.ShouldEqual("Data1");
            request.Comments.ShouldEqual("Some comments23");
        }

        [Test]
        public void should_override_member()
        {
            var spec = BuildSpec<Handlers.GetHandler>(x => x
                .OverrideMembers((y, z) => z.Name = z.Name + "1")
                .OverrideMembers((y, z) => z.Comments = z.Comments + "2")
                .OverrideMembersWhen((y, z) => z.Comments = z.Comments + "3", (y, z) => z.Comments.EndsWith("2"))
                .OverrideMembersWhen((y, z) => z.Comments = z.Comments + "4", (y, z) => z.Comments.EndsWith("2")));
            
            var request = spec.Modules[0].Resources[0].Endpoints[0].Request.Body.Description[1];
            request.Name.ShouldEqual("Id1");
            request.Comments.ShouldEqual("Some comments23");
        }

        [Test]
        public void should_override_option()
        {
            var spec = BuildSpec<Handlers.GetHandler>(x => x
                .OverrideOptions((y, z) => z.Name = z.Name + "1")
                .OverrideOptions((y, z) => z.Comments = z.Comments + "2")
                .OverrideOptionsWhen((y, z) => z.Comments = z.Comments + "3", (y, z) => z.Comments.EndsWith("2"))
                .OverrideOptionsWhen((y, z) => z.Comments = z.Comments + "4", (y, z) => z.Comments.EndsWith("2")));


            var request = spec.Modules[0].Resources[0].Endpoints[0].Request.Body.Description[1];
            request.Options.Options[0].Name.ShouldEqual("SomeName1");
            request.Options.Options[0].Comments.ShouldEqual("Some comments23");
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
                .OverrideRequest((a, b) => b.Comments += "2")
                .OverrideRequestWhen((a, b) => b.Comments += "3", (a, b) => b.Comments.EndsWith("2"))
                .OverrideRequestWhen((a, b) => b.Comments += "4", (a, b) => b.Comments.EndsWith("2")));

            spec.Modules[0].Resources[0].Endpoints[0].Request.Comments.ShouldEqual("Some request comments23");
        }

        [Test]
        public void should_override_response()
        {
            var spec = BuildSpec<Handlers.GetHandler>(x => x
                .OverrideResponse((a, b) => b.Comments += "2")
                .OverrideResponseWhen((a, b) => b.Comments += "3", (a, b) => b.Comments.EndsWith("2"))
                .OverrideResponseWhen((a, b) => b.Comments += "4", (a, b) => b.Comments.EndsWith("2")));

            spec.Modules[0].Resources[0].Endpoints[1].Response.Comments.ShouldEqual("Some response comments23");
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
        public void should_override_all_headers()
        {
            var spec = BuildSpec<Handlers.GetHandler>(x => x
                .OverrideHeaders((a, b) => b.Name = b.Name + "1")
                .OverrideHeaders((a, b) => b.Comments = b.Comments + "2")
                .OverrideHeadersWhen((a, b) => b.Comments = b.Comments + "3", (a, b) => b.Comments.EndsWith("2"))
                .OverrideHeadersWhen((a, b) => b.Comments = b.Comments + "4", (a, b) => b.Comments.EndsWith("2")));

            var header = spec.Modules[0].Resources[0].Endpoints[1].Request.Headers[0];
            header.Name.ShouldEqual("SomeRequestHeader1");
            header.Comments.ShouldEqual("Some request header comments23");

            header = spec.Modules[0].Resources[0].Endpoints[1].Response.Headers[0];
            header.Name.ShouldEqual("SomeResponseHeader1");
            header.Comments.ShouldEqual("Some response header comments23");
        }

        [Test]
        public void should_override_request_headers()
        {
            var spec = BuildSpec<Handlers.GetHandler>(x => x
                .OverrideRequestHeaders((a, b) => b.Name = b.Name + "1")
                .OverrideRequestHeaders((a, b) => b.Comments = b.Comments + "2")
                .OverrideRequestHeadersWhen((a, b) => b.Comments = b.Comments + "3", (a, b) => b.Comments.EndsWith("2"))
                .OverrideRequestHeadersWhen((a, b) => b.Comments = b.Comments + "4", (a, b) => b.Comments.EndsWith("2")));

            var header = spec.Modules[0].Resources[0].Endpoints[1].Request.Headers[0];
            header.Name.ShouldEqual("SomeRequestHeader1");
            header.Comments.ShouldEqual("Some request header comments23");

            header = spec.Modules[0].Resources[0].Endpoints[1].Response.Headers[0];
            header.Name.ShouldEqual("SomeResponseHeader");
            header.Comments.ShouldEqual("Some response header comments");
        }

        [Test]
        public void should_override_response_headers()
        {
            var spec = BuildSpec<Handlers.GetHandler>(x => x
                .OverrideResponseHeaders((a, b) => b.Name = b.Name + "1")
                .OverrideResponseHeaders((a, b) => b.Comments = b.Comments + "2")
                .OverrideResponseHeadersWhen((a, b) => b.Comments = b.Comments + "3", (a, b) => b.Comments.EndsWith("2"))
                .OverrideResponseHeadersWhen((a, b) => b.Comments = b.Comments + "4", (a, b) => b.Comments.EndsWith("2")));

            var header = spec.Modules[0].Resources[0].Endpoints[1].Request.Headers[0];
            header.Name.ShouldEqual("SomeRequestHeader");
            header.Comments.ShouldEqual("Some request header comments");

            header = spec.Modules[0].Resources[0].Endpoints[1].Response.Headers[0];
            header.Name.ShouldEqual("SomeResponseHeader1");
            header.Comments.ShouldEqual("Some response header comments23");
        }
    }
}