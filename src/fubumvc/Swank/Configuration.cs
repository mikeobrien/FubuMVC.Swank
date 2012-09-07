using System;
using System.Collections.Generic;
using System.Reflection;
using FubuMVC.Core.Registration.Nodes;

namespace Swank
{
    public enum OrphanedActions { Exclude, Fail, Default }

    public class Configuration
    {
        public Configuration()
        {
            Url = "docs";
            AppliesToAssemblies = new List<Assembly>();
            Filter = x => true;
            ModuleSource = new Service<IModuleSource> 
                { Type = typeof(ModuleSource) };
            ResourceSource = new Service<IResourceSource> 
                { Type = typeof(ResourceSource), Config = new ResourceSourceConfig() };
            DefaultModuleFactory = x => new Module();
            OrphanedModuleActions = OrphanedActions.Default;
            DefaultResourceFactory = x => new Resource { Name = x.ParentChain().Route.GetRouteResource() };
            OrphanedResourceActions = OrphanedActions.Default;
        }

        public string Url { get; set; }
        public string SpecificationUrl { get; set; }
        public List<Assembly> AppliesToAssemblies { get; set; }
        public Func<ActionCall, bool> Filter { get; set; }
        public Service<IModuleSource> ModuleSource { get; set; }
        public Service<IResourceSource> ResourceSource { get; set; }
        public Func<ActionCall, Module> DefaultModuleFactory { get; set; }
        public OrphanedActions OrphanedModuleActions { get; set; }
        public OrphanedActions OrphanedResourceActions { get; set; }
        public Func<ActionCall, Resource> DefaultResourceFactory { get; set; }
    }
}
