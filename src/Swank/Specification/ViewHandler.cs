using System.Collections.Generic;
using FubuMVC.Swank.Extensions;

namespace FubuMVC.Swank.Specification
{
    public class HandlerResponse
    {
        public string Copyright { get; set; }
        public List<string> Scripts { get; set; }
        public List<string> Stylesheets { get; set; }
        public bool ShowJson { get; set; }
        public bool ShowXml { get; set; }
        public Specification Specification { get; set; }
        public SampleValues SampleValues { get; set; }
    }

    public class SampleValues
    {
        public string DateTime { get; set; }
        public string Integer { get; set; }
        public string Real { get; set; }
        public string TimeSpan { get; set; }
        public string Guid { get; set; }
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
                ShowXml = _configuration.DisplayXml,
                ShowJson = _configuration.DisplayJson,
                Specification = _specificationService.Generate(),
                SampleValues = new SampleValues {
                        DateTime = _configuration.SampleDateTimeValue.ToDefaultValueString(_configuration),
                        Guid = _configuration.SampleGuidValue.ToDefaultValueString(_configuration),
                        Integer = _configuration.SampleIntegerValue.ToDefaultValueString(_configuration),
                        Real = _configuration.SampleRealValue.ToDefaultValueString(_configuration),
                        TimeSpan = _configuration.SampleTimeSpanValue.ToDefaultValueString(_configuration),
                    }
            };
        }
    }
}