using FubuMVC.Swank;
using FubuMVC.Swank.Extensions;
using NUnit.Framework;
using Should;
using System.Web.Script.Serialization;

namespace Tests
{
    [TestFixture]
    public class AcceptanceTests
    {
        private Website _testWebsite;

        [TestFixtureSetUp]
        public void Setup()
        {
            _testWebsite = new Website();
            _testWebsite.Create(typeof(Swank).Assembly.GetName().Name, Paths.TestHarness);
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            _testWebsite.Remove();
        }

        public class IndexModel { public string Message { get; set; } }

        [Test]
        public void should_have_connectivity_to_the_test_harness_site()
        {
            _testWebsite.DownloadString("", "application/json").DeserializeJson<IndexModel>()
                .Message.ShouldEqual("oh hai");
        }

        [Test]
        public void should_return_specification_page()
        {
            _testWebsite.DownloadString("documentation", "text/html")
                .ShouldContain("<title>Test Harness API</title>");
        }

        [Test]
        public void should_return_specification_data()
        {
            var spec = _testWebsite.DownloadString("documentation/data", "application/json")
                .DeserializeJson<FubuMVC.Swank.Specification.Specification>();
            spec.Types.ShouldNotBeNull();
            spec.Modules.ShouldNotBeNull();
            spec.Resources.ShouldNotBeNull();
        }

        [Test]
        public void should_return_style_content()
        {
            _testWebsite.DownloadString("_content/swank/swank.css").ShouldNotBeEmpty();
        }

        [Test]
        public void should_return_js_content()
        {
            _testWebsite.DownloadString("_content/swank/swank.js").ShouldNotBeEmpty();
        }
    }
}