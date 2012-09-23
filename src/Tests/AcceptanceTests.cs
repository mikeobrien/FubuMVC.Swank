using System;
using System.IO;
using Bottles.Commands;
using Bottles.Creation;
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
            _testWebsite = new Website();
            var testHarnessPath = Path.GetFullPath(Environment.CurrentDirectory + @"\..\..\..\TestHarness");
            _testWebsite.Create("FubuMVC.Swank", testHarnessPath, 34534);
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
            _testWebsite.DownloadString("test.html").ShouldEqual("oh hai");

            
        }
    }
}