using System;
using System.Collections.Generic;
using System.Reflection;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Swank.Description;

namespace FubuMVC.Swank
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
            graph.Services.AddService<IDescriptionSource<ActionCall, ModuleDescription>>(_configuration.ModuleDescriptionSource.Type, _configuration.ModuleDescriptionSource.Config);
            graph.Services.AddService<IDescriptionSource<ActionCall, ResourceDescription>>(_configuration.ResourceDescriptionSource.Type, _configuration.ResourceDescriptionSource.Config);
            graph.Services.AddService<IDescriptionSource<ActionCall, EndpointDescription>>(_configuration.EndpointDescriptionSource.Type, _configuration.EndpointDescriptionSource.Config);
            graph.Services.AddService<IDescriptionSource<PropertyInfo, ParameterDescription>>(_configuration.ParameterDescriptionSource.Type, _configuration.ParameterDescriptionSource.Config);
            graph.Services.AddService<IDescriptionSource<FieldInfo, OptionDescription>>(_configuration.OptionDescriptionSource.Type, _configuration.OptionDescriptionSource.Config);
            graph.Services.AddService<IDescriptionSource<ActionCall, List<ErrorDescription>>>(_configuration.ErrorDescriptionSource.Type, _configuration.ErrorDescriptionSource.Config);
            graph.Services.AddService<IDescriptionSource<System.Type, DataTypeDescription>>(_configuration.DataTypeDescriptionSource.Type, _configuration.DataTypeDescriptionSource.Config);
            graph.AddActionFor(_configuration.SpecificationUrl, typeof(SpecificationHandler)).MakeAsymmetricJson();
        }

        public static SwankConvention Create(Action<ConfigurationDsl> configure)
        {
            return new SwankConvention(ConfigurationDsl.CreateConfig(configure));
        }
    }
}