using System;
using System.Reflection;
using FubuMVC.Core.Registration.Nodes;

namespace Swank
{
    public class ConfigurationDsl
    {
        private readonly Configuration _configuration;

        public ConfigurationDsl(Configuration configuration)
        {
            _configuration = configuration;
        }

        public ConfigurationDsl AppliesToThisAssembly()
        {
            _configuration.AppliesToAssemblies.Add(Assembly.GetCallingAssembly());
            return this;
        }

        public ConfigurationDsl AppliesTo<T>()
        {
            AppliesTo(typeof (T));
            return this;
        }

        public ConfigurationDsl AppliesTo(Type type)
        {
            _configuration.AppliesToAssemblies.Add(type.Assembly);
            return this;
        }

        public ConfigurationDsl At(string url)
        {
            _configuration.Url = url;
            _configuration.SpecificationUrl = url + "/specs/";
            return this;
        }

        public ConfigurationDsl Where(Func<ActionCall, bool> filter)
        {
            _configuration.Filter = filter;
            return this;
        }

        public ConfigurationDsl WithModules(IModuleSource modules)
        {
            _configuration.Modules = modules;
            return this;
        }

        public ConfigurationDsl WithModules<T>() where T : IModuleSource, new()
        {
            _configuration.Modules = new T();
            return this;
        }

        public ConfigurationDsl WithDefaultModule(string name, string description)
        {
            _configuration.DefaultModule = new ModuleDescription(name, description);
            return this;
        }

        public ConfigurationDsl OnOrphanedAction(OrphanedActionsBehavior behavior)
        {
            _configuration.OrphanedActions = behavior;
            return this;
        }
    }
}
