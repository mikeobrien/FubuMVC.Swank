using System.Collections.Generic;
using System.Linq;
using System.Web;
using FubuMVC.Swank.Extensions;
using FubuMVC.Swank.Specification;

namespace FubuMVC.Swank.Documentation
{
    public class Response
    {
        public class Example
        {
            public string Id { get; set; }
            public bool First { get; set; }
            public string Name { get; set; }
            public string Comments { get; set; }
            public string Template { get; set; }
        }

        public string Copyright { get; set; }
        public string DownloadUrl { get; set; }
        public bool ShowXmlFormat { get; set; }
        public bool ShowJsonFormat { get; set; }
        public List<string> Scripts { get; set; }
        public List<string> Stylesheets { get; set; }
        public List<Example> CodeExamples { get; set; }
        public Specification.Specification Specification { get; set; }
    }

    public class GetHandler
    {

        private readonly Configuration _configuration;
        private readonly LazyCache<Specification.Specification> _specification = 
            new LazyCache<Specification.Specification>();

        public GetHandler(
            SpecificationService specificationService, 
            Configuration configuration)
        {
            _specification.UseFactory(specificationService.Generate);
            _configuration = configuration;
        }

        public Response Execute()
        {
            return new Response {
                Copyright = _configuration.Copyright,
                DownloadUrl = ("/" + _configuration.Url + "/" + "spec").Replace("//", "/"),
                Scripts = _configuration.Scripts,
                Stylesheets = _configuration.Stylesheets,
                CodeExamples = _configuration.CodeExamples.Select((x, i) => new Response.Example
                    {
                        Id = x.Name.Hash(),
                        First = i == 0,
                        Name = x.Name,
                        Comments = x.Comments,
                        Template = x.Template
                    }).ToList(),
                    ShowXmlFormat = _configuration.DisplayXmlFormat,
                ShowJsonFormat = _configuration.DisplayJsonFormat,
                Specification = _specification.Value
            };
        }
    }
}