using System;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Resources.Conneg;

namespace Swank
{
    public class SwankConvention : IConfigurationAction
    {
        private readonly Configuration _configuration;

        public SwankConvention(Configuration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(BehaviorGraph graph)
        {
            graph.Services.AddService(_configuration);
            graph.AddActionFor(_configuration.SpecificationUrl, typeof(SpecificationHandler)).MakeAsymmetricJson();
        }

        public static SwankConvention Create(Action<Configuration> configure)
        {
            var configuration = new Configuration();
            configure(configuration);
            return new SwankConvention(configuration);
        }
    }
}