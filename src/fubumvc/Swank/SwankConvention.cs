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
            graph.Services.AddService<IModuleSource>(_configuration.ModuleSource.Type, _configuration.ModuleSource.Config);
            graph.Services.AddService<IResourceSource>(_configuration.ResourceSource.Type, _configuration.ResourceSource.Config);
            graph.AddActionFor(_configuration.SpecificationUrl, typeof(SpecificationHandler)).MakeAsymmetricJson();
        }

        public static SwankConvention Create(Action<ConfigurationDsl> configure)
        {
            return new SwankConvention(ConfigurationDsl.CreateConfig(configure));
        }
    }
}