using FubuMVC.Swank.Extensions;
using NUnit.Framework;
using Should;

namespace Tests.ExtensionTests
{
    [TestFixture]
    public class StringTests
    {
        [Test]
        public void should_unwrap_paragraph_tags()
        {
            "<p><p>Oh<p> </p>hai</p></p>".UnwrapParagraphTags()
                .ShouldEqual("<p>Oh<p> </p>hai</p>");
        }

        [Test]
        [TestCase("", "")]
        [TestCase(null, null)]
        [TestCase("Oh [Hai](http://www.google.com)", 
            "<p>Oh <a href=\"http://www.google.com\">Hai</a></p>")]
        public void should_transform_markdown_block(string text, string result)
        {
            text.TransformMarkdownBlock().ShouldEqual(result);
        }

        [Test]
        [TestCase("", "")]
        [TestCase(null, null)]
        [TestCase("Oh [Hai](http://www.google.com)", 
            "Oh <a href=\"http://www.google.com\">Hai</a>")]
        public void should_transform_markdown_inline(string text, string result)
        {
            text.TransformMarkdownInline().ShouldEqual(result);
        }

        [Test]
        public void should_hash_string()
        {
            "oh hai".Hash().ShouldEqual("7e90827f576c594b6d604a94830d093d");
        }

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