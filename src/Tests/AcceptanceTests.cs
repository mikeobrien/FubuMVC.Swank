using System;
using System.IO;
using FubuMVC.Swank;
using NUnit.Framework;
using Should;

namespace Tests
{
    [TestFixture]
    public class AcceptanceTests
    {
        private Website _testWebsite;

        [SetUp]
        public void Setup()
        {
            if (!Bottles.Create(Paths.Swank, Path.Combine(Paths.TestHarness, "fubu-content", "fubu-swank.zip"), true, true)) 
                throw new Exception("Could not create bottle.");
            _testWebsite = new Website();
            _testWebsite.Create(typeof(SwankConvention).Assembly.GetName().Name, Paths.TestHarness);
        }

        [TearDown]
        public void TearDown()
        {
            _testWebsite.Remove();
        }

        [Test]
        public void should_have_connectivity_to_the_test_harness_site()
        {
            _testWebsite.DownloadString().ShouldEqual("oh hai");
        }
    }
}