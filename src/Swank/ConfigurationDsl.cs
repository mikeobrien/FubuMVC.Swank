using System;
using System.Collections.Generic;
using System.Reflection;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Swank.Description;

namespace FubuMVC.Swank
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

        public ConfigurationDsl MergeThisSpecification(string path)
        {
            _configuration.MergeSpecificationPath = path;
            return this;
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

        public ConfigurationDsl AppliesTo(System.Type type)
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

        public ConfigurationDsl WithModuleDescriptionSource<T>() where T : IDescriptionSource<ActionCall, ModuleDescription>
        {
            return WithModuleDescriptionSource<T, object>(null);
        }

        public ConfigurationDsl WithModuleDescriptionSource<T, TConfig>(Action<TConfig> configure)
            where T : IDescriptionSource<ActionCall, ModuleDescription>
            where TConfig : class, new()
        {
            _configuration.ModuleDescriptionSource.Type = typeof(T);
            _configuration.ModuleDescriptionSource.Config = CreateConfig(configure);
            return this;
        }

        public ConfigurationDsl WithResourceDescriptionSource<T>() where T : IDescriptionSource<ActionCall, ResourceDescription>
        {
            return WithResourceDescriptionSource<T, object>(null);
        }

        public ConfigurationDsl WithResourceDescriptionSource<T, TConfig>(Action<TConfig> configure)
            where T : IDescriptionSource<ActionCall, ResourceDescription>
            where TConfig : class, new()
        {
            _configuration.ResourceDescriptionSource.Type = typeof(T);
            _configuration.ResourceDescriptionSource.Config = CreateConfig(configure);
            return this;
        }

        public ConfigurationDsl WithEndpointDescriptionSource<T>() where T : IDescriptionSource<ActionCall, EndpointDescription>
        {
            return WithEndpointDescriptionSource<T, object>(null);
        }

        public ConfigurationDsl WithEndpointDescriptionSource<T, TConfig>(Action<TConfig> configure)
            where T : IDescriptionSource<ActionCall, EndpointDescription>
            where TConfig : class, new()
        {
            _configuration.EndpointDescriptionSource.Type = typeof(T);
            _configuration.EndpointDescriptionSource.Config = CreateConfig(configure);
            return this;
        }

        public ConfigurationDsl WithMemberDescriptionSource<T>() where T : IDescriptionSource<PropertyInfo, MemberDescription>
        {
            return WithMemberDescriptionSource<T, object>(null);
        }

        public ConfigurationDsl WithMemberDescriptionSource<T, TConfig>(Action<TConfig> configure)
            where T : IDescriptionSource<PropertyInfo, MemberDescription>
            where TConfig : class, new()
        {
            _configuration.MemberDescriptionSource.Type = typeof(T);
            _configuration.MemberDescriptionSource.Config = CreateConfig(configure);
            return this;
        }

        public ConfigurationDsl WithOptionDescriptionSource<T>() where T : IDescriptionSource<FieldInfo, OptionDescription>
        {
            return WithOptionDescriptionSource<T, object>(null);
        }

        public ConfigurationDsl WithOptionDescriptionSource<T, TConfig>(Action<TConfig> configure)
            where T : IDescriptionSource<FieldInfo, OptionDescription>
            where TConfig : class, new()
        {
            _configuration.OptionDescriptionSource.Type = typeof(T);
            _configuration.OptionDescriptionSource.Config = CreateConfig(configure);
            return this;
        }

        public ConfigurationDsl WithErrorDescriptionSource<T>() where T : IDescriptionSource<ActionCall, List<ErrorDescription>>
        {
            return WithErrorDescriptionSource<T, object>(null);
        }

        public ConfigurationDsl WithErrorDescriptionSource<T, TConfig>(Action<TConfig> configure)
            where T : IDescriptionSource<ActionCall, List<ErrorDescription>>
            where TConfig : class, new()
        {
            _configuration.ErrorDescriptionSource.Type = typeof(T);
            _configuration.ErrorDescriptionSource.Config = CreateConfig(configure);
            return this;
        }

        public ConfigurationDsl WithDataTypeDescriptionSource<T>() where T : IDescriptionSource<System.Type, DataTypeDescription>
        {
            return WithDataTypeDescriptionSource<T, object>(null);
        }

        public ConfigurationDsl WithDataTypeDescriptionSource<T, TConfig>(Action<TConfig> configure)
            where T : IDescriptionSource<System.Type, DataTypeDescription>
            where TConfig : class, new()
        {
            _configuration.DataTypeDescriptionSource.Type = typeof(T);
            _configuration.DataTypeDescriptionSource.Config = CreateConfig(configure);
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
