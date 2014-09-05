using FubuMVC.Swank.Extensions;
using NUnit.Framework;
using Should;

namespace Tests.ExtensionTests
{
    [TestFixture]
    public class StringTests
    {
        [Test]
        public void should_replace_nbsp_with_space(
            [Values("&nbsp;", "&NBSP;")] string text)
        {
            text.ConvertNbspHtmlEntityToSpaces().ShouldEqual(" ");
        }

        [Test]
        public void should_replace_br_with_line_break(
            [Values("<br>", "<br/>", "<br />")] string text)
        {
            ("oh" + text.ConvertBrHtmlTagsToLineBreaks() + "hai").ShouldEqual("oh\r\nhai");
        }
    }
}