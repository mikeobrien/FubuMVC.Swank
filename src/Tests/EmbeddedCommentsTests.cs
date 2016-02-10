using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class EmbeddedCommentsTests
    {
        [Test]
        public void embedded_comments_should_match_types()
        {
            FubuMVC.Swank.Description.Assert.AllEmbeddedCommentsMatchTypes(x => 
                !x.StartsWith("Tests.ExtensionTests.") && 
                !x.EndsWith(".Comments.md") &&
                !x.StartsWith("Tests.Description.CodeExamples."));
        }
    }
}