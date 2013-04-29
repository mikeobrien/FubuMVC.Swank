using System;
using FubuCore.Reflection;
using FubuMVC.Swank;
using FubuMVC.Swank.Description;
using FubuMVC.Swank.Specification;
using NUnit.Framework;
using Should;

namespace Tests.Specification.SpecificationService.MergeTests
{
    namespace NoHandlers { public class Marker { } }

    [TestFixture]
    public class Tests
    {
        protected FubuMVC.Swank.Specification.Specification BuildSpec<TNamespace>(Action<Swank> configure = null)
        {
            var graph = Behavior.BuildGraph().AddActionsInThisNamespace();
            var moduleConvention = new ModuleConvention(new MarkerConvention<ModuleDescription>());
            var resourceConvention = new ResourceConvention(
                new MarkerConvention<ResourceDescription>(),
                new ActionSource(graph,
                    Swank.CreateConfig(x => x.AppliesToThisAssembly()
                        .Where(y => y.HandlerType.InNamespace<Tests>()))));
            var configuration = Swank.CreateConfig(x =>
                { if (configure != null) configure(x); x.AppliesToThisAssembly().Where(y => y.HandlerType.InNamespace<TNamespace>())
                    .MergeThisSpecification(@"Specification\SpecificationService\MergeTests\Merge.json");
                });
            return new FubuMVC.Swank.Specification.SpecificationService(configuration, new ActionSource(graph, configuration), new TypeDescriptorCache(),
                moduleConvention, resourceConvention, new EndpointConvention(), new MemberConvention(), new OptionConvention(), new StatusCodeConvention(),
                new HeaderConvention(), new TypeConvention(), new MergeService()).Generate();
        }

        [Test]
        public void should_merge_all_the_things()
        {
            var spec = BuildSpec<NoHandlers.Marker>();

            spec.Types.Count.ShouldEqual(1);
            var type = spec.Types[0];

            type.Id.ShouldEqual("Some type id");
            type.Name.ShouldEqual("SomeType");
            type.Comments.ShouldEqual("Some type comments");
            type.Members.Count.ShouldEqual(1);

            var member = type.Members[0];
            member.Name.ShouldEqual("SomeMember");
            member.Comments.ShouldEqual("Some member comments");
            member.Required.ShouldBeTrue();
            member.DefaultValue.ShouldEqual("some default value");
            member.Type.ShouldEqual("some type");
            member.Collection.ShouldBeTrue();
            member.Options.Count.ShouldEqual(1);

            var option = member.Options[0];
            option.Name.ShouldEqual("SomeOption");
            option.Comments.ShouldEqual("Some option comments");
            option.Value.ShouldEqual("Some option value");

            spec.Modules.Count.ShouldEqual(1);
            var module = spec.Modules[0];

            module.Name.ShouldEqual("Some module");
            module.Comments.ShouldEqual("Some module comments");
            module.Resources.Count.ShouldEqual(1);

            var resource = module.Resources[0];

            resource.Name.ShouldEqual("Some module resource");
            resource.Comments.ShouldEqual("Some module resource comments");
            resource.Endpoints.Count.ShouldEqual(1);

            var endpoint = resource.Endpoints[0];

            endpoint.Name.ShouldEqual("Some endpoint");
            endpoint.Comments.ShouldEqual("Some endpoint comments");
            endpoint.Url.ShouldEqual("/some/url");
            endpoint.Method.ShouldEqual("METHOD");
            endpoint.UrlParameters.Count.ShouldEqual(1);

            var urlParameter = endpoint.UrlParameters[0];
            urlParameter.Name.ShouldEqual("Some url param");
            urlParameter.Comments.ShouldEqual("Some url param comments");
            urlParameter.Type.ShouldEqual("Some type");
            urlParameter.Options.Count.ShouldEqual(1);

            option = urlParameter.Options[0];
            option.Name.ShouldEqual("Some option");
            option.Value.ShouldEqual("Some option value");
            option.Comments.ShouldEqual("Some option comments");

            endpoint.QuerystringParameters.Count.ShouldEqual(1);
            var querystringParameter = endpoint.QuerystringParameters[0];
            querystringParameter.Name.ShouldEqual("Some querystring");
            querystringParameter.Comments.ShouldEqual("Some querystring comments");
            querystringParameter.DefaultValue.ShouldEqual("Some default value");
            querystringParameter.MultipleAllowed.ShouldBeTrue();
            querystringParameter.Required.ShouldBeTrue();
            querystringParameter.Type.ShouldEqual("Some type");
            querystringParameter.Options.Count.ShouldEqual(1);

            option = urlParameter.Options[0];
            option.Name.ShouldEqual("Some option");
            option.Value.ShouldEqual("Some option value");
            option.Comments.ShouldEqual("Some option comments");

            endpoint.StatusCodes.Count.ShouldEqual(1);
            var statusCode = endpoint.StatusCodes[0];
            statusCode.Name.ShouldEqual("Some error");
            statusCode.Comments.ShouldEqual("Some error comments");
            statusCode.Code.ShouldEqual(999);

            endpoint.Request.Name.ShouldEqual("Some request");
            endpoint.Request.Comments.ShouldEqual("Some request comments");
            endpoint.Request.Type.ShouldEqual("Some type");
            endpoint.Request.Collection.ShouldBeTrue();

            endpoint.Response.Name.ShouldEqual("Some response");
            endpoint.Response.Comments.ShouldEqual("Some response comments");
            endpoint.Response.Type.ShouldEqual("Some type");
            endpoint.Response.Collection.ShouldBeTrue();
        }
    }
}