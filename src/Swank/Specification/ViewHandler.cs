using System.Collections.Generic;

namespace FubuMVC.Swank.Specification
{
    public class HandlerResponse
    {
        public string Copyright { get; set; }
        public List<string> Scripts { get; set; }
        public List<string> Stylesheets { get; set; }
        public Specification Specification { get; set; }    
    }

    public class ViewHandler
    {
        private readonly ISpecificationService _specificationService;
        private readonly Configuration _configuration;

        public ViewHandler(ISpecificationService specificationService, Configuration configuration)
        {
            _specificationService = specificationService;
            _configuration = configuration;
        }

        public HandlerResponse Execute()
        {
            return new HandlerResponse {
                Copyright = _configuration.Copyright,
                Scripts = _configuration.Scripts,
                Stylesheets = _configuration.Stylesheets,
                Specification = _specificationService.Generate() 
            };
        }
    }
}