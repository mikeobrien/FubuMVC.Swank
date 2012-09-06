using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration;
using NUnit.Framework;
using Should;
using Swank;

namespace Tests
{
    [TestFixture]
    public class SpecificationBuilderResourceTests
    {
        private BehaviorGraph _graph;

        [SetUp]
        public void Setup()
        {
            _graph = TestBehaviorGraph.Build();
        }

        [Test]
        public void should_()
        {
            //var configuration = new Configuration().AppliesToThisAssembly()
            //    .GroupActionsIntoModulesBy(x => x.ParentChain().Route.FirstPatternSegment())
            //    .AddModule("admin", "Administration", "User administration.")
            //    .AddModule("batches", "Cell Batches", "Cell batch management.");
            //var specBuilder = new SpecificationBuilder(configuration, _graph);

            //var spec = specBuilder.Build();

            //spec.modules.Count.ShouldEqual(2);

            //spec.modules[0].name.ShouldEqual("Administration");
            //spec.modules[0].description.ShouldEqual("User administration.");

            //spec.modules[1].name.ShouldEqual("Cell Batches");
            //spec.modules[1].description.ShouldEqual("Cell batch management.");
        }
    }
}