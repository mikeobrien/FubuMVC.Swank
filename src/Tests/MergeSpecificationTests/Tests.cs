using System;
using FubuCore.Reflection;
using FubuMVC.Swank;
using FubuMVC.Swank.Description;
using NUnit.Framework;
using Should;

namespace Tests.MergeSpecificationTests
{
    [TestFixture]
    public class Tests
    {
        protected Specification BuildSpec<TNamespace>(Action<ConfigurationDsl> configure = null)
        {
            var graph = Behaviors.BuildGraph().AddActionsInThisNamespace();
            var moduleSource = new ModuleSource(new MarkerSource<ModuleDescription>());
            var resourceSource = new ResourceSource(
                new MarkerSource<ResourceDescription>(),
                new ActionSource(graph,
                    ConfigurationDsl.CreateConfig(x => x.AppliesToThisAssembly()
                        .Where(y => y.HandlerType.InNamespace<SpecificationBuilderModuleTests.Tests>()))));
            var configuration = ConfigurationDsl.CreateConfig(x =>
                { if (configure != null) configure(x); x.AppliesToThisAssembly().Where(y => y.HandlerType.InNamespace<TNamespace>())
                    .MergeThisSpecification(@"MergeSpecificationTests\Merge.json"); });
            return new SpecificationBuilder(configuration, new ActionSource(graph, configuration), new TypeDescriptorCache(),
                moduleSource, resourceSource, new EndpointSource(), new MemberSource(), new OptionSource(), new ErrorSource(), new TypeSource()).Build();
        }

        [Test]
        public void should_merge_all_the_things()
        {
            var spec = BuildSpec<NoHandlers.Marker>();

            spec.types.Count.ShouldEqual(1);
            var type = spec.types[0];

            type.id.ShouldEqual("Some type id");
            type.name.ShouldEqual("SomeType");
            type.comments.ShouldEqual("Some type comments");
            type.members.Count.ShouldEqual(1);

            var member = type.members[0];
            member.name.ShouldEqual("SomeMember");
            member.comments.ShouldEqual("Some member comments");
            member.required.ShouldBeTrue();
            member.defaultValue.ShouldEqual("some default value");
            member.type.ShouldEqual("some type");
            member.collection.ShouldBeTrue();
            member.options.Count.ShouldEqual(1);

            var option = member.options[0];
            option.name.ShouldEqual("SomeOption");
            option.comments.ShouldEqual("Some option comments");
            option.value.ShouldEqual("Some option value");

            spec.modules.Count.ShouldEqual(1);
            var module = spec.modules[0];

            module.name.ShouldEqual("Some module");
            module.comments.ShouldEqual("Some module comments");
            module.resources.Count.ShouldEqual(1);

            var resource = module.resources[0];

            resource.name.ShouldEqual("Some module resource");
            resource.comments.ShouldEqual("Some module resource comments");
            resource.endpoints.Count.ShouldEqual(1);

            var endpoint = resource.endpoints[0];

            endpoint.name.ShouldEqual("Some endpoint");
            endpoint.comments.ShouldEqual("Some endpoint comments");
            endpoint.url.ShouldEqual("/some/url");
            endpoint.method.ShouldEqual("METHOD");
            endpoint.urlParameters.Count.ShouldEqual(1);

            var urlParameter = endpoint.urlParameters[0];
            urlParameter.name.ShouldEqual("Some url param");
            urlParameter.comments.ShouldEqual("Some url param comments");
            urlParameter.type.ShouldEqual("Some type");
            urlParameter.options.Count.ShouldEqual(1);

            option = urlParameter.options[0];
            option.name.ShouldEqual("Some option");
            option.value.ShouldEqual("Some option value");
            option.comments.ShouldEqual("Some option comments");

            endpoint.querystringParameters.Count.ShouldEqual(1);
            var querystringParameter = endpoint.querystringParameters[0];
            querystringParameter.name.ShouldEqual("Some querystring");
            querystringParameter.comments.ShouldEqual("Some querystring comments");
            querystringParameter.defaultValue.ShouldEqual("Some default value");
            querystringParameter.multipleAllowed.ShouldBeTrue();
            querystringParameter.required.ShouldBeTrue();
            querystringParameter.type.ShouldEqual("Some type");
            querystringParameter.options.Count.ShouldEqual(1);

            option = urlParameter.options[0];
            option.name.ShouldEqual("Some option");
            option.value.ShouldEqual("Some option value");
            option.comments.ShouldEqual("Some option comments");

            endpoint.errors.Count.ShouldEqual(1);
            var error = endpoint.errors[0];
            error.name.ShouldEqual("Some error");
            error.comments.ShouldEqual("Some error comments");
            error.status.ShouldEqual(999);

            endpoint.request.name.ShouldEqual("Some request");
            endpoint.request.comments.ShouldEqual("Some request comments");
            endpoint.request.type.ShouldEqual("Some type");
            endpoint.request.collection.ShouldBeTrue();

            endpoint.response.name.ShouldEqual("Some response");
            endpoint.response.comments.ShouldEqual("Some response comments");
            endpoint.response.type.ShouldEqual("Some type");
            endpoint.response.collection.ShouldBeTrue();
        }

        [Test]
        public void should_merge_overlapping_modules()
        {
            var spec = BuildSpec<OverlappingModule.GetHandler>();

            spec.modules.Count.ShouldEqual(1);
            var module = spec.modules[0];

            module.name.ShouldEqual("Some module");
            module.comments.ShouldBeNull();
            module.resources.Count.ShouldEqual(2);

            var resource = module.resources[0];
            resource.name.ShouldEqual("overlappingmodule");
            resource.endpoints.Count.ShouldEqual(1);

            resource = module.resources[1];
            resource.name.ShouldEqual("Some module resource");
            resource.comments.ShouldEqual("Some module resource comments");
            resource.endpoints.Count.ShouldEqual(1);
        }

        [Test]
        public void should_merge_overlapping_resource_modules()
        {
            var spec = BuildSpec<OverlappingModuleResource.GetHandler>();

            spec.modules.Count.ShouldEqual(1);
            var module = spec.modules[0];

            module.name.ShouldEqual("Some module");
            module.comments.ShouldBeNull();
            module.resources.Count.ShouldEqual(1);

            var resource = module.resources[0];
            resource.name.ShouldEqual("Some module resource");
            resource.comments.ShouldBeNull();
            resource.endpoints.Count.ShouldEqual(2);

            var endpoint = resource.endpoints[0];
            endpoint.name.ShouldBeNull();
            endpoint.comments.ShouldBeNull();
            endpoint.url.ShouldEqual("/overlappingmoduleresource");
            endpoint.method.ShouldEqual("GET");

            endpoint = resource.endpoints[1];
            endpoint.name.ShouldEqual("Some endpoint");
            endpoint.comments.ShouldEqual("Some endpoint comments");
            endpoint.url.ShouldEqual("/some/url");
            endpoint.method.ShouldEqual("METHOD");
        }

        [Test]
        public void should_merge_overlapping_resources()
        {
            var spec = BuildSpec<OverlappingResource.GetHandler>();

            spec.modules.Count.ShouldEqual(1);

            spec.resources.Count.ShouldEqual(1);

            var resource = spec.resources[0];
            resource.name.ShouldEqual("Some resource");
            resource.comments.ShouldBeNull();
            resource.endpoints.Count.ShouldEqual(2);

            var endpoint = resource.endpoints[0];
            endpoint.name.ShouldBeNull();
            endpoint.comments.ShouldBeNull();
            endpoint.url.ShouldEqual("/overlappingresource");
            endpoint.method.ShouldEqual("GET");

            endpoint = resource.endpoints[1];
            endpoint.name.ShouldEqual("Some endpoint");
            endpoint.comments.ShouldEqual("Some endpoint comments");
            endpoint.url.ShouldEqual("/some/url");
            endpoint.method.ShouldEqual("METHOD");
        }
    }
}