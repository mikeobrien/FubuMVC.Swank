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
            Modules = new ModuleSource();
            DefaultModule = new ModuleDescription();
            OrphanedActions = OrphanedActionsBehavior.AddToDefaultModule;
        }

        public string Url { get; set; }
        public string SpecificationUrl { get; set; }
        public List<Assembly> AppliesToAssemblies { get; set; }
        public Func<ActionCall, bool> Filter { get; set; }
        public IModuleSource Modules { get; set; }
        public ModuleDescription DefaultModule { get; set; }
        public OrphanedActionsBehavior OrphanedActions { get; set; }
    }
}
