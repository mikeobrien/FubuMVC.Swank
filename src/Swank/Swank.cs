using System;
using System.Collections.Generic;
using System.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Swank.Description;

namespace FubuMVC.Swank
{
    public class Swank : IFubuRegistryExtension
    {
        private readonly Configuration _configuration = new Configuration();

        public static Configuration CreateConfig(Action<Swank> configure)
        {
            var swank = new Swank();
            configure(swank);
            return swank._configuration;
        }
        
        void IFubuRegistryExtension.Configure(FubuRegistry registry)
        {
            registry.Import(new Conventions(_configuration), _configuration.Url);
        }        

        public Swank MergeThisSpecification(string path)
        {
            _configuration.MergeSpecificationPath = path;
            return this;
        }

        public Swank AppliesToThisAssembly()
        {
            _configuration.AppliesToAssemblies.Add(Assembly.GetCallingAssembly());
            return this;
        }

        public Swank AppliesTo<T>()
        {
            AppliesTo(typeof (T));
            return this;
        }

        public Swank AppliesTo(System.Type type)
        {
            _configuration.AppliesToAssemblies.Add(type.Assembly);
            return this;
        }

        public Swank AtUrl(string url)
        {
            _configuration.Url = url;
            return this;
        }

        public Swank Where(Func<ActionCall, bool> filter)
        {
            _configuration.Filter = filter;
            return this;
        }

        public Swank WithModuleDescriptionSource<T>() where T : IDescriptionSource<ActionCall, ModuleDescription>
        {
            return WithModuleDescriptionSource<T, object>(null);
        }

        public Swank WithModuleDescriptionSource<T, TConfig>(Action<TConfig> configure)
            where T : IDescriptionSource<ActionCall, ModuleDescription>
            where TConfig : class, new()
        {
            _configuration.ModuleDescriptionSource.Type = typeof(T);
            _configuration.ModuleDescriptionSource.Config = CreateConfig(configure);
            return this;
        }

        public Swank WithResourceDescriptionSource<T>() where T : IDescriptionSource<ActionCall, ResourceDescription>
        {
            return WithResourceDescriptionSource<T, object>(null);
        }

        public Swank WithResourceDescriptionSource<T, TConfig>(Action<TConfig> configure)
            where T : IDescriptionSource<ActionCall, ResourceDescription>
            where TConfig : class, new()
        {
            _configuration.ResourceDescriptionSource.Type = typeof(T);
            _configuration.ResourceDescriptionSource.Config = CreateConfig(configure);
            return this;
        }

        public Swank WithEndpointDescriptionSource<T>() where T : IDescriptionSource<ActionCall, EndpointDescription>
        {
            return WithEndpointDescriptionSource<T, object>(null);
        }

        public Swank WithEndpointDescriptionSource<T, TConfig>(Action<TConfig> configure)
            where T : IDescriptionSource<ActionCall, EndpointDescription>
            where TConfig : class, new()
        {
            _configuration.EndpointDescriptionSource.Type = typeof(T);
            _configuration.EndpointDescriptionSource.Config = CreateConfig(configure);
            return this;
        }

        public Swank WithMemberDescriptionSource<T>() where T : IDescriptionSource<PropertyInfo, MemberDescription>
        {
            return WithMemberDescriptionSource<T, object>(null);
        }

        public Swank WithMemberDescriptionSource<T, TConfig>(Action<TConfig> configure)
            where T : IDescriptionSource<PropertyInfo, MemberDescription>
            where TConfig : class, new()
        {
            _configuration.MemberDescriptionSource.Type = typeof(T);
            _configuration.MemberDescriptionSource.Config = CreateConfig(configure);
            return this;
        }

        public Swank WithOptionDescriptionSource<T>() where T : IDescriptionSource<FieldInfo, OptionDescription>
        {
            return WithOptionDescriptionSource<T, object>(null);
        }

        public Swank WithOptionDescriptionSource<T, TConfig>(Action<TConfig> configure)
            where T : IDescriptionSource<FieldInfo, OptionDescription>
            where TConfig : class, new()
        {
            _configuration.OptionDescriptionSource.Type = typeof(T);
            _configuration.OptionDescriptionSource.Config = CreateConfig(configure);
            return this;
        }

        public Swank WithErrorDescriptionSource<T>() where T : IDescriptionSource<ActionCall, List<ErrorDescription>>
        {
            return WithErrorDescriptionSource<T, object>(null);
        }

        public Swank WithErrorDescriptionSource<T, TConfig>(Action<TConfig> configure)
            where T : IDescriptionSource<ActionCall, List<ErrorDescription>>
            where TConfig : class, new()
        {
            _configuration.ErrorDescriptionSource.Type = typeof(T);
            _configuration.ErrorDescriptionSource.Config = CreateConfig(configure);
            return this;
        }

        public Swank WithDataTypeDescriptionSource<T>() where T : IDescriptionSource<System.Type, DataTypeDescription>
        {
            return WithDataTypeDescriptionSource<T, object>(null);
        }

        public Swank WithDataTypeDescriptionSource<T, TConfig>(Action<TConfig> configure)
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

        public Swank WithDefaultModule(Func<ActionCall, ModuleDescription> factory)
        {
            _configuration.DefaultModuleFactory = factory;
            return this;
        }

        public Swank OnOrphanedModuleAction(OrphanedActions behavior)
        {
            _configuration.OrphanedModuleActions = behavior;
            return this;
        }

        public Swank WithDefaultResource(Func<ActionCall, ResourceDescription> factory)
        {
            _configuration.DefaultResourceFactory = factory;
            return this;
        }

        public Swank OnOrphanedResourceAction(OrphanedActions behavior)
        {
            _configuration.OrphanedResourceActions = behavior;
            return this;
        }
    }
}