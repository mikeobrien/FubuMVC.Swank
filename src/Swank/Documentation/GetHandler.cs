using System.Collections.Generic;
using FubuMVC.Swank.Specification;

namespace FubuMVC.Swank.Documentation
{
    public class Response
    {
        public string Copyright { get; set; }
        public bool ShowXmlFormat { get; set; }
        public bool ShowJsonFormat { get; set; }
        public List<string> Scripts { get; set; }
        public List<string> Stylesheets { get; set; }
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
                Scripts = _configuration.Scripts,
                Stylesheets = _configuration.Stylesheets,
                ShowXmlFormat = _configuration.DisplayXmlFormat,
                ShowJsonFormat = _configuration.DisplayJsonFormat,
                Specification = _specification.Value
            };
        }

        public Specification.Specification DataExecute()
        {
            return _specification.Value;
        }
    }
}