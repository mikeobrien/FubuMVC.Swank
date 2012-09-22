using System;
using System.Linq;
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
                { if (configure != null) configure(x); x.AppliesToThisAssembly().Where(y => y.HandlerType.InNamespace<TNamespace>()); });
            return new SpecificationBuilder(configuration, new ActionSource(graph, configuration), new TypeDescriptorCache(),
                moduleSource, resourceSource, new EndpointSource(), new MemberSource(), new OptionSource(), new ErrorSource(), new TypeSource()).Build();
        }

        [Test]
        public void should()
        {
            var spec =
                BuildSpec<Handlers.GetHandler>(
                    x => x.MergeThisSpecification(@"MergeSpecificationTests\Merge.json"));

            spec.types.Count.ShouldEqual(4);

            var type = spec.types.Single(x => x.id == "1234");
            type.name.ShouldEqual("status");
            type.members.Count.ShouldEqual(1);
            var member = type.members[0];
            member.name.ShouldEqual("description");
            member.type.ShouldEqual("string");
            member.collection.ShouldBeTrue();

            spec.types.Single(x => x.id == typeof(Handlers.Response).GetHash(typeof(Handlers.GetHandler).GetExecuteMethod()))
                .name.ShouldEqual("Response");

            var method = typeof (Handlers.Administration.Users.PostHandler).GetExecuteMethod();
            spec.types.Single(x => x.id == typeof(Handlers.Administration.Users.Request).GetHash(method))
                .name.ShouldEqual("Request");

            spec.types.Single(x => x.id == typeof(Handlers.Administration.Users.Response).GetHash(method))
                .name.ShouldEqual("Response");

            spec.modules.Count.ShouldEqual(2);



            spec.resources.Count.ShouldEqual(1);
        }
    }
}