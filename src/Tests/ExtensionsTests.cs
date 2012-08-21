using NUnit.Framework;
using Should;
using Swank;

namespace Tests
{
    [TestFixture]
    public class ExtensionsTests
    {
        [Test]
        public void should_strip_url_parameters()
        {
            "{Yada}/books/{Id}/categories/{CategoryId}/classification/{ClassId}".StripUrlParameters()
                .ShouldEqual("books/categories/classification");
        }
    }
}