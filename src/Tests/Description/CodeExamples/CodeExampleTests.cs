using System;
using System.IO;
using System.Linq;
using System.Reflection;
using FubuMVC.Swank.Description;
using NUnit.Framework;
using Should;

namespace Tests.Description.CodeExamples
{
    [TestFixture]
    public class CodeExampleTests
    {
        private static readonly Assembly[] ThisAssembly = { Assembly.GetExecutingAssembly() };
        private const string PreProcessedTemplate = "some\r\ntemplate yo";
        private const string CompiledComments = "<p><em>comments</em></p>";

        [Test]
        [TestCase("CodeExampleWithCommentsAndTemplate", "C#", CompiledComments, PreProcessedTemplate)]
        [TestCase("CodeExampleWithCommentsAndTemplate", null, CompiledComments, PreProcessedTemplate)]
        [TestCase("CodeExampleWithNoComments", null, null, PreProcessedTemplate)]
        [TestCase("CodeExampleWithNoTemplate", null, CompiledComments, null)]
        public void should_pull_code_example_from_embedded_resource(string filename, string name, string comments, string template)
        {
            var examples = CodeExample.FromEmbeddedResource(ThisAssembly, filename, name);
            examples.Name.ShouldEqual(name ?? filename);
            examples.Comments.ShouldEqual(comments);
            examples.Template.ShouldEqual(template);
        }

        [Test]
        public void should_pull_code_example_from_folder()
        {
            var path = Path.GetTempPath() + Guid.NewGuid().ToString();
            Directory.CreateDirectory(path);

            try
            {
                File.WriteAllText(Path.Combine(path, "CodeExampleWithCommentsAndTemplate.md"), "*comments*");
                File.WriteAllText(Path.Combine(path, "CodeExampleWithCommentsAndTemplate.mustache"), "some<br/>template&nbsp;\r\nyo");
                File.WriteAllText(Path.Combine(path, "CodeExampleWithNoTemplate.md"), "*comments*");
                File.WriteAllText(Path.Combine(path, "CodeExampleWithNoComments.mustache"), "some<br/>template&nbsp;\r\nyo");

                var examples = CodeExample.FromDirectory(path).ToList();

                examples[0].Name.ShouldEqual("CodeExampleWithCommentsAndTemplate");
                examples[0].Comments.ShouldEqual(CompiledComments);
                examples[0].Template.ShouldEqual(PreProcessedTemplate);

                examples[1].Name.ShouldEqual("CodeExampleWithNoComments");
                examples[1].Comments.ShouldEqual(null);
                examples[1].Template.ShouldEqual(PreProcessedTemplate);

                examples[2].Name.ShouldEqual("CodeExampleWithNoTemplate");
                examples[2].Comments.ShouldEqual(CompiledComments);
                examples[2].Template.ShouldEqual(null);
            }
            finally
            {
                Directory.Delete(path, true);
            }
        }
    }
}
