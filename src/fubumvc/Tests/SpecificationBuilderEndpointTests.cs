using FubuCore.Reflection;
using NUnit.Framework;
using Should;
using Swank;
using Swank.Description;
using Tests.Administration.Users;

namespace Tests
{
    [TestFixture]
    public class SpecificationBuilderEndpointTests
    {
        private SpecificationBuilder _specBuilder;

        [SetUp]
        public void Setup()
        {
            var graph = TestBehaviorGraph.Build();
            var moduleSource = new ModuleSource(new MarkerSource<ModuleDescription>());
            var resourceSource = new ResourceSource(
                new MarkerSource<ResourceDescription>(),
                new ActionSource(graph, ConfigurationDsl.CreateConfig(x => x.AppliesToThisAssembly())), new ResourceSourceConfig());
            var endpointSource = new EndpointSource();
            var parameterSource = new ParameterSource();
            var optionSource = new OptionSource();
            var errors = new ErrorSource();
            var dataTypes = new DataTypeSource();
            var configuration = ConfigurationDsl.CreateConfig(x => x.AppliesToThisAssembly());
            _specBuilder = new SpecificationBuilder(configuration, new ActionSource(graph, configuration), new TypeDescriptorCache(),
                moduleSource, resourceSource, endpointSource, parameterSource, optionSource, errors, dataTypes);
        }

        [Test]
        public void should_enumerate_endpoints()
        {
            var spec = _specBuilder.Build();

            var resources = spec.modules[0].resources;
            resources.Count.ShouldEqual(2);
            var endpoints = resources[0].endpoints;
            endpoints.Count.ShouldEqual(5);
            endpoints = resources[1].endpoints;
            endpoints.Count.ShouldEqual(5);

            resources = spec.modules[1].resources;
            endpoints = resources[0].endpoints;
            endpoints.Count.ShouldEqual(5);
            endpoints = resources[1].endpoints;
            endpoints.Count.ShouldEqual(6);
            endpoints = resources[2].endpoints;
            endpoints.Count.ShouldEqual(5);

            resources = spec.modules[2].resources;
            endpoints = resources[0].endpoints;
            endpoints.Count.ShouldEqual(5);

            resources = spec.modules[3].resources;
            endpoints = resources[0].endpoints;
            endpoints.Count.ShouldEqual(5);
        }

        [Test]
        public void should_enumerate_endpoint_description()
        {
            var spec = _specBuilder.Build();

            var endpoints = spec.modules[0].resources[0].endpoints;

            var endpoint = endpoints[0];
            endpoint.name.ShouldEqual("GetTemplates");
            endpoint.comments.ShouldEqual("This gets all the templates yo.");
            endpoint.method.ShouldEqual("GET");
            endpoint.url.ShouldEqual("/templates");

            endpoint = endpoints[1];
            endpoint.name.ShouldEqual("AddTemplate");
            endpoint.comments.ShouldEqual("This adds a the template yo.");
            endpoint.method.ShouldEqual("POST");
            endpoint.url.ShouldEqual("/templates");

            endpoint = endpoints[2];
            endpoint.name.ShouldBeNull();
            endpoint.comments.ShouldBeNull();
            endpoint.method.ShouldEqual("DELETE");
            endpoint.url.ShouldEqual("/templates/{Id}");

            endpoint = endpoints[3];
            endpoint.name.ShouldBeNull();
            endpoint.comments.ShouldEqual("<b>This gets a template yo!</b>");
            endpoint.method.ShouldEqual("GET");
            endpoint.url.ShouldEqual("/templates/{Id}");

            endpoint = endpoints[4];
            endpoint.name.ShouldBeNull();
            endpoint.comments.ShouldEqual("<p><strong>This updates a template yo!</strong></p>");
            endpoint.method.ShouldEqual("PUT");
            endpoint.url.ShouldEqual("/templates/{Id}");
        }

        [Test]
        public void should_enumerate_endpoint_url_parameters()
        {
            var spec = _specBuilder.Build();

            var endpoint = spec.modules[1].resources[1].endpoints[2];

            endpoint.querystringParameters.Count.ShouldEqual(2);
            endpoint.urlParameters.Count.ShouldEqual(2);

            var parameter = endpoint.urlParameters[0];
            parameter.name.ShouldEqual("UserId");
            parameter.dataType.ShouldEqual("uuid");
            parameter.comments.ShouldEqual("This is the id of the user.");
            parameter.options.ShouldBeEmpty();

            parameter = endpoint.urlParameters[1];
            parameter.name.ShouldEqual("AddressType");
            parameter.dataType.ShouldEqual("enum");
            parameter.comments.ShouldBeNull();
            parameter.options.Count.ShouldEqual(3);
            
            var options = parameter.options;

            options.Count.ShouldEqual(3);

            var option = options[0];
            option.name.ShouldBeNull();
            option.value.ShouldEqual("Emergency");
            option.comments.ShouldBeNull();

            option = options[1];
            option.name.ShouldEqual("Home address");
            option.value.ShouldEqual("Home");
            option.comments.ShouldEqual("This is the home address of the user.");

            option = options[2];
            option.name.ShouldEqual("Work Address");
            option.value.ShouldEqual("Work");
            option.comments.ShouldEqual("This is the work address of the user.");
        }

        [Test]
        public void should_enumerate_endpoint_implicit_querystring_parameters()
        {
            var spec = _specBuilder.Build();

            var endpoint = spec.modules[1].resources[1].endpoints[2];

            endpoint.querystringParameters.Count.ShouldEqual(2);
            endpoint.urlParameters.Count.ShouldEqual(2);

            var parameter = endpoint.querystringParameters[0];
            parameter.name.ShouldEqual("Id");
            parameter.comments.ShouldEqual("This is the id.");
            parameter.dataType.ShouldEqual("uuid");
            parameter.multipleAllowed.ShouldBeFalse();
            parameter.defaultValue.ShouldBeNull();
            parameter.options.ShouldBeEmpty();

            parameter = endpoint.querystringParameters[1];
            parameter.name.ShouldEqual("Order");
            parameter.comments.ShouldBeNull();
            parameter.dataType.ShouldEqual("enum");
            parameter.multipleAllowed.ShouldBeFalse();
            parameter.defaultValue.ShouldBeNull();
            parameter.options.Count.ShouldEqual(2);

            var options = parameter.options;

            options.Count.ShouldEqual(2);

            var option = options[0];
            option.name.ShouldBeNull();
            option.value.ShouldEqual("Asc");
            option.comments.ShouldBeNull();

            option = options[1];
            option.name.ShouldBeNull();
            option.value.ShouldEqual("Desc");
            option.comments.ShouldBeNull();
        }

        [Test]
        public void should_enumerate_endpoint_explicit_querystring_parameters()
        {
            var spec = _specBuilder.Build();

            var endpoint = spec.modules[1].resources[0].endpoints[1];

            endpoint.querystringParameters.Count.ShouldEqual(2);
            endpoint.urlParameters.Count.ShouldEqual(0);

            var parameter = endpoint.querystringParameters[0];
            parameter.name.ShouldEqual("Order");
            parameter.comments.ShouldBeNull();
            parameter.dataType.ShouldEqual("enum");
            parameter.multipleAllowed.ShouldBeFalse();
            parameter.defaultValue.ShouldEqual("Desc");
            parameter.options.Count.ShouldEqual(2);

            var options = parameter.options;

            options.Count.ShouldEqual(2);

            var option = options[0];
            option.name.ShouldBeNull();
            option.value.ShouldEqual("Asc");
            option.comments.ShouldBeNull();

            option = options[1];
            option.name.ShouldBeNull();
            option.value.ShouldEqual("Desc");
            option.comments.ShouldBeNull();

            parameter = endpoint.querystringParameters[1];
            parameter.name.ShouldEqual("Show");
            parameter.comments.ShouldBeNull();
            parameter.dataType.ShouldEqual("uuid");
            parameter.multipleAllowed.ShouldBeTrue();
            parameter.defaultValue.ShouldBeNull();
            parameter.options.Count.ShouldEqual(0);
        }

        [Test]
        public void should_enumerate_endpoint_errors()
        {
            var spec = _specBuilder.Build();

            var endpoint = spec.modules[1].resources[1].endpoints[0];

            endpoint.errors.Count.ShouldEqual(2);
            var error = endpoint.errors[0];
            error.status.ShouldEqual(410);
            error.name.ShouldEqual("Invalid address");
            error.comments.ShouldEqual("An invalid address was entered fool!");

            error = endpoint.errors[1];
            error.status.ShouldEqual(411);
            error.name.ShouldEqual("Swank address");
            error.comments.ShouldBeNull();
        }

        [Test]
        public void should_specify_input_type()
        {
            var spec = _specBuilder.Build();

            var request = spec.modules[1].resources[1].endpoints[0].request;

            request.name.ShouldEqual("AdminAddressRequest");
            request.comments.ShouldEqual("This is an address request yo!");
            request.dataType.ShouldEqual(typeof(AdminAddressRequest).FullName.Hash());
            request.collection.ShouldBeFalse();
        }

        [Test]
        public void should_specify_output_type()
        {
            var spec = _specBuilder.Build();

            var request = spec.modules[1].resources[1].endpoints[0].response;

            request.name.ShouldEqual("AdminAddressResponse");
            request.comments.ShouldBeNull();
            request.dataType.ShouldEqual(typeof(AdminAddressResponse).FullName.Hash());
            request.collection.ShouldBeFalse();
        }

        [Test]
        public void should_specify_list_output_type()
        {
            var spec = _specBuilder.Build();

            var request = spec.modules[1].resources[1].endpoints[4].response;

            request.name.ShouldEqual("ArrayOfAdminAddressResponse");
            request.comments.ShouldBeNull();
            request.dataType.ShouldEqual(typeof(AdminAddressResponse).FullName.Hash());
            request.collection.ShouldBeTrue();
        }

        [Test]
        public void should_specify_inherited_list_output_type()
        {
            var spec = _specBuilder.Build();

            var request = spec.modules[1].resources[1].endpoints[2].response;

            request.name.ShouldEqual("ArrayOfAdminAddressResponse");
            request.comments.ShouldEqual("These are addresses yo!");
            request.dataType.ShouldEqual(typeof(AdminAddressResponse).FullName.Hash());
            request.collection.ShouldBeTrue();
        }

        [Test]
        public void should_specify_overriden_output_type()
        {
            var spec = _specBuilder.Build();

            var request = spec.modules[1].resources[2].endpoints[0].request;

            request.name.ShouldEqual("User");
            request.comments.ShouldBeNull();
            request.dataType.ShouldEqual(typeof(AdminUserRequest).FullName.Hash());
            request.collection.ShouldBeFalse();
        }

        [Test]
        public void should_specify_overriden_list_output_type()
        {
            var spec = _specBuilder.Build();

            var request = spec.modules[1].resources[2].endpoints[0].response;

            request.name.ShouldEqual("Users");
            request.comments.ShouldEqual("These are users yo!");
            request.dataType.ShouldEqual(typeof(AdminUserResponse).FullName.Hash());
            request.collection.ShouldBeTrue();
        }
    }
}