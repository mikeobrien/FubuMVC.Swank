using System.Collections.Generic;

namespace FubuMVC.Swank.Documentation
{
    public class Response
    {
        public string Copyright { get; set; }
        public bool ShowXmlFormat { get; set; }
        public bool ShowJsonFormat { get; set; }
        public List<string> Scripts { get; set; }
        public List<string> Stylesheets { get; set; }
        public SpecificationModel Specification { get; set; }
    }

    public class GetHandler
    {
        private readonly SpecificationFactory _specificationFactory;
        private readonly Configuration _configuration;

        public GetHandler(
            SpecificationFactory specificationFactory, 
            Configuration configuration)
        {
            _specificationFactory = specificationFactory;
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
                Specification = _specificationFactory.Create()
            };
        }

        public SpecificationModel DataExecute()
        {
            return _specificationFactory.Create();
        }
    }
}