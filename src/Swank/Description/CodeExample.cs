using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FubuMVC.Swank.Extensions;

namespace FubuMVC.Swank.Description
{
    public class CodeExample
    {
        private const string TemplateExtension = ".mustache";
        private const string CommentsExtension = ".md";
        private readonly static string[] CodeExampleExtensions = { CommentsExtension, TemplateExtension };

        public string Name { get; set; }
        public string Comments { get; set; }
        public string Template { get; set; }

        public static CodeExample FromEmbeddedResource(IEnumerable<Assembly> assemblies, string filename, string name = null)
        {
            var template = assemblies.FindTextResourceNamed("*" + filename + TemplateExtension);
            return new CodeExample
            {
                Name = name ?? filename,
                Comments = assemblies.FindTextResourceNamed("*" + filename + CommentsExtension),
                Template = template != null ? PreProcessTemplate(template) : null
            };
        }

        public static IEnumerable<CodeExample> FromDirectory(string path)
        {
            return Directory.GetFiles(path)
                .Where(x => CodeExampleExtensions.Contains(Path.GetExtension(x)))
                .GroupBy(Path.GetFileNameWithoutExtension)
                .Select(x => new
                {
                    Name = Path.GetFileNameWithoutExtension(x.First()),
                    CommentsPath = x.FirstOrDefault(y => Path.GetExtension(y) == CommentsExtension),
                    TemplatePath = x.FirstOrDefault(y => Path.GetExtension(y) == TemplateExtension)
                })
                .Select(x => new CodeExample
                {
                    Name = x.Name,
                    Comments = x.CommentsPath != null ? File.ReadAllText(x.CommentsPath)
                        .TransformIfMarkdownFile(x.CommentsPath) : null,
                    Template = x.TemplatePath != null ? PreProcessTemplate(
                        File.ReadAllText(x.TemplatePath)) : null
                });
        }

        private static string PreProcessTemplate(string template)
        {
            return template.Flatten().ConvertNbspHtmlEntityToSpaces().ConvertBrHtmlTagsToLineBreaks();
        }
    }
}