using FubuMVC.Swank;
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
            new JavaScriptSerializer().Deserialize<IndexModel>(_testWebsite.DownloadString("", "application/json"))
                .Message.ShouldEqual("oh hai");
        }

        [Test]
        public void should_return_specification_html()
        {
            _testWebsite.DownloadString("documentation", "text/html")
                .ShouldContain("<title>Test Harness API</title>");
        }

        [Test]
        public void should_return_specification_json()
        {
            var spec = new JavaScriptSerializer().Deserialize<FubuMVC.Swank.Specification.Specification>(
                _testWebsite.DownloadString("documentation/data", "application/json"));
            spec.Types.ShouldBeEmpty();
            spec.Modules.ShouldBeEmpty();
            spec.Resources.ShouldBeEmpty();
        }

        [Test]
        public void should_return_content()
        {
            _testWebsite.DownloadString("_content/swank/swank.js").ShouldEqual("{}");
        }
    }
}