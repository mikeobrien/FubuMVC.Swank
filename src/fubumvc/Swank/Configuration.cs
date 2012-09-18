using System;
using System.Collections.Generic;
using System.Reflection;
using FubuMVC.Core.Registration.Nodes;
using Swank.Description;

namespace Swank
{
    public enum OrphanedActions { Exclude, Fail, UseDefault }

    public class Configuration
    {
        public class Service<T>
        {
            public Type Type { get; set; }
            public object Config { get; set; }
        }

        public Configuration()
        {
            Url = "docs";
            AppliesToAssemblies = new List<Assembly>();
            Filter = x => true;
            ModuleDescriptionSource = new Service<IDescriptionSource<ActionCall, ModuleDescription>> { Type = typeof(ModuleSource) };
            ResourceDescriptionSource = new Service<IDescriptionSource<ActionCall, ResourceDescription>> 
                { Type = typeof(ResourceSource), Config = new ResourceSourceConfig() };
            DefaultModuleFactory = x => null;
            OrphanedModuleActions = OrphanedActions.UseDefault;
            DefaultResourceFactory = x => new ResourceDescription { Name = x.ParentChain().Route.GetRouteResource() };
            OrphanedResourceActions = OrphanedActions.UseDefault;
            EndpointDescriptionSource = new Service<IDescriptionSource<ActionCall, EndpointDescription>> { Type = typeof(EndpointSource) };
            ParameterDescriptionSource = new Service<IDescriptionSource<PropertyInfo, ParameterDescription>> { Type = typeof(ParameterSource) };
            OptionDescriptionSource = new Service<IDescriptionSource<FieldInfo, OptionDescription>> { Type = typeof(OptionSource) };
            ErrorDescriptionSource = new Service<IDescriptionSource<ActionCall, List<ErrorDescription>>> { Type = typeof(ErrorSource) };
            DataTypeDescriptionSource = new Service<IDescriptionSource<Type, DataTypeDescription>> { Type = typeof(TypeSource) };
        }
        
        public string Url { get; set; }
        public string SpecificationUrl { get; set; }
        public List<Assembly> AppliesToAssemblies { get; set; }
        public Func<ActionCall, bool> Filter { get; set; }
        public OrphanedActions OrphanedModuleActions { get; set; }
        public OrphanedActions OrphanedResourceActions { get; set; }
        public Func<ActionCall, ModuleDescription> DefaultModuleFactory { get; set; }
        public Func<ActionCall, ResourceDescription> DefaultResourceFactory { get; set; }
        public Service<IDescriptionSource<ActionCall, ModuleDescription>> ModuleDescriptionSource { get; set; }
        public Service<IDescriptionSource<ActionCall, ResourceDescription>> ResourceDescriptionSource { get; set; }
        public Service<IDescriptionSource<ActionCall, EndpointDescription>> EndpointDescriptionSource { get; set; }
        public Service<IDescriptionSource<PropertyInfo, ParameterDescription>> ParameterDescriptionSource { get; set; }
        public Service<IDescriptionSource<FieldInfo, OptionDescription>> OptionDescriptionSource { get; set; }
        public Service<IDescriptionSource<ActionCall, List<ErrorDescription>>> ErrorDescriptionSource { get; set; }
        public Service<IDescriptionSource<Type, DataTypeDescription>> DataTypeDescriptionSource { get; set; }
    }
}
