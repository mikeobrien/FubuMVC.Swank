using System;
using System.Collections.Generic;
using System.Reflection;
using FubuMVC.Core.Registration.Nodes;

namespace Swank
{
    public enum OrphanedActionsBehavior { Ignore, ThrowException, AddToDefaultModule }

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
            DefaultModule = new Module();
            OrphanedActions = OrphanedActionsBehavior.AddToDefaultModule;
        }

        public string Url { get; set; }
        public string SpecificationUrl { get; set; }
        public List<Assembly> AppliesToAssemblies { get; set; }
        public Func<ActionCall, bool> Filter { get; set; }
        public Service<IModuleSource> ModuleSource { get; set; }
        public Service<IResourceSource> ResourceSource { get; set; }
        public Module DefaultModule { get; set; }
        public OrphanedActionsBehavior OrphanedActions { get; set; }
    }
}
