using System;
using System.Reflection;
using FubuMVC.Core.Registration.Nodes;
using Swank.Description;

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

        public ConfigurationDsl WithModuleDescriptionSource<T>() where T : IDescriptionSource<ActionCall, ModuleDescription>, new()
        {
            return WithModuleDescriptionSource<T, object>(null);
        }

        public ConfigurationDsl WithModuleDescriptionSource<T, TConfig>(Action<TConfig> configure)
            where T : IDescriptionSource<ActionCall, ModuleDescription>, new() 
            where TConfig : class, new()
        {
            _configuration.ModuleDescriptionSource.Type = typeof(T);
            _configuration.ModuleDescriptionSource.Config = CreateConfig(configure);
            return this;
        }

        public ConfigurationDsl WithResourceDescriptionSource<T>() where T : IDescriptionSource<ActionCall, ResourceDescription>, new()
        {
            return WithResourceDescriptionSource<T, object>(null);
        }

        public ConfigurationDsl WithResourceDescriptionSource<T, TConfig>(Action<TConfig> configure)
            where T : IDescriptionSource<ActionCall, ResourceDescription>, new()
            where TConfig : class, new()
        {
            _configuration.ResourceDescriptionSource.Type = typeof(T);
            _configuration.ModuleDescriptionSource.Config = CreateConfig(configure);
            return this;
        }

        public ConfigurationDsl WithEndpointDescriptionSource<T>() where T : IDescriptionSource<ActionCall, EndpointDescription>, new()
        {
            return WithEndpointDescriptionSource<T, object>(null);
        }

        public ConfigurationDsl WithEndpointDescriptionSource<T, TConfig>(Action<TConfig> configure)
            where T : IDescriptionSource<ActionCall, EndpointDescription>, new()
            where TConfig : class, new()
        {
            _configuration.ResourceDescriptionSource.Type = typeof(T);
            _configuration.EndpointDescriptionSource.Config = CreateConfig(configure);
            return this;
        }

        private TConfig CreateConfig<TConfig>(Action<TConfig> configure) where TConfig : class, new()
        {
            if (configure == null) return null;
            var config = new TConfig();
            configure(config);
            return config;
        }

        public ConfigurationDsl WithDefaultModule(Func<ActionCall, ModuleDescription> factory)
        {
            _configuration.DefaultModuleFactory = factory;
            return this;
        }

        public ConfigurationDsl OnOrphanedModuleAction(OrphanedActions behavior)
        {
            _configuration.OrphanedModuleActions = behavior;
            return this;
        }

        public ConfigurationDsl WithDefaultResource(Func<ActionCall, ResourceDescription> factory)
        {
            _configuration.DefaultResourceFactory = factory;
            return this;
        }

        public ConfigurationDsl OnOrphanedResourceAction(OrphanedActions behavior)
        {
            _configuration.OrphanedResourceActions = behavior;
            return this;
        }
    }
}
