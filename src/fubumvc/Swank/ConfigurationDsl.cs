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

        public static Configuration CreateConfig(Action<ConfigurationDsl> configure)
        {
            var config = new Configuration();
            configure(new ConfigurationDsl(config));
            return config;
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

        public ConfigurationDsl WithModuleSource<T>() where T : IModuleSource, new()
        {
            return WithModuleSource<T, object>(null);
        }

        public ConfigurationDsl WithModuleSource<T, TConfig>(Action<TConfig> configure) 
            where T : IModuleSource, new() 
            where TConfig : class, new()
        {
            _configuration.ModuleSource.Type = typeof(T);
            if (configure != null)
            {
                var config = new TConfig();
                configure(config);
                _configuration.ModuleSource.Config = config;
            }
            return this;
        }

        public ConfigurationDsl WithResourceSource<T>() where T : IResourceSource, new()
        {
            return WithResourceSource<T, object>(null);
        }

        public ConfigurationDsl WithResourceSource<T, TConfig>(Action<TConfig> configure)
            where T : IResourceSource, new()
            where TConfig : class, new()
        {
            _configuration.ResourceSource.Type = typeof(T);
            if (configure != null)
            {
                var config = new TConfig();
                configure(config);
                _configuration.ModuleSource.Config = config;
            }
            return this;
        }

        public ConfigurationDsl WithDefaultModule(string name, string comments)
        {
            _configuration.DefaultModule = new Module { Name = name, Comments = comments };
            return this;
        }

        public ConfigurationDsl OnOrphanedAction(OrphanedActionsBehavior behavior)
        {
            _configuration.OrphanedActions = behavior;
            return this;
        }
    }
}
