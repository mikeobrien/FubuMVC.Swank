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
            ModuleDescriptionSource = new Service<IDescriptionSource<ActionCall, ModuleDescription>>
                { Type = typeof(ModuleSource) };
            ResourceDescriptionSource = new Service<IDescriptionSource<ActionCall, ResourceDescription>> 
                { Type = typeof(ResourceSource), Config = new ResourceSourceConfig() };
            DefaultModuleFactory = x => new ModuleDescription();
            OrphanedModuleActions = OrphanedActions.UseDefault;
            DefaultResourceFactory = x => new ResourceDescription { Name = x.ParentChain().Route.GetRouteResource() };
            OrphanedResourceActions = OrphanedActions.UseDefault;
            EndpointDescriptionSource = new Service<IDescriptionSource<ActionCall, EndpointDescription>> 
                { Type = typeof(EndpointSource) };
        }

        public string Url { get; set; }
        public string SpecificationUrl { get; set; }
        public List<Assembly> AppliesToAssemblies { get; set; }
        public Func<ActionCall, bool> Filter { get; set; }
        public Service<IDescriptionSource<ActionCall, ModuleDescription>> ModuleDescriptionSource { get; set; }
        public Service<IDescriptionSource<ActionCall, ResourceDescription>> ResourceDescriptionSource { get; set; }
        public Service<IDescriptionSource<ActionCall, EndpointDescription>> EndpointDescriptionSource { get; set; }
        public Func<ActionCall, ModuleDescription> DefaultModuleFactory { get; set; }
        public OrphanedActions OrphanedModuleActions { get; set; }
        public OrphanedActions OrphanedResourceActions { get; set; }
        public Func<ActionCall, ResourceDescription> DefaultResourceFactory { get; set; }
    }
}
