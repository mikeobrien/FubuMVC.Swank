using System;
using System.IO;
using System.Linq;
using Bottles.Commands;
using Bottles.Creation;
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
            var swankAssemblyName = typeof(SwankConvention).Assembly.GetName().Name;
            _testWebsite = new Website();
            var testHarnessPath = Path.GetFullPath(Environment.CurrentDirectory + @"\..\..\..\TestHarness");
            Directory.GetFiles(Path.Combine(testHarnessPath, "bin"), swankAssemblyName + ".*", SearchOption.AllDirectories).ToList().ForEach(File.Delete);
            _testWebsite.Create(swankAssemblyName, testHarnessPath, 34534);
            var swankPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\Swank"));
            var testHarnessFubuContentPath = Path.Combine(testHarnessPath, "fubu-content", "fubu-swank.zip");
            if (!Bottles.Create(swankPath, testHarnessFubuContentPath, true, true)) 
                throw new Exception("Could not create bottle.");
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