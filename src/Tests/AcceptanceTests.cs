using NUnit.Framework;
using Should;

namespace Tests
{
    [TestFixture]
    public class AcceptanceTests
    {
        private TestWebsite _testWebsite;

        [SetUp]
        public void Setup()
        {  
            _testWebsite = new TestWebsite();
            _testWebsite.Create();
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