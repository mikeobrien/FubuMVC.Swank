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

        /// <summary>
        /// Path to a json formatted specification file that you want to merge with the spec that is generated.
        /// You can use application realitive paths a la ~/myspec.json.
        /// </summary>
        public Swank MergeThisSpecification(string path)
        {
            _configuration.MergeSpecificationPath = path;
            return this;
        }

        /// <summary>
        /// The spec should be generated from types in this assembly.
        /// </summary>
        public Swank AppliesToThisAssembly()
        {
            _configuration.AppliesToAssemblies.Add(Assembly.GetCallingAssembly());
            return this;
        }

        /// <summary>
        /// The spec should be generated from the assembly of the specified type.
        /// This call is additive, so you can specify multiple assemblies.
        /// </summary>
        public Swank AppliesTo<T>()
        {
            AppliesTo(typeof (T));
            return this;
        }

        /// <summary>
        /// The spec should be generated from the assembly of the specified type.
        /// This call is additive, so you can specify multiple assemblies.
        /// </summary>
        public Swank AppliesTo(Type type)
        {
            _configuration.AppliesToAssemblies.Add(type.Assembly);
            return this;
        }

        /// <summary>
        /// This defines the url of the specification endpoint.
        /// The default is /specification.
        /// </summary>
        public Swank AtUrl(string url)
        {
            _configuration.Url = url;
            return this;
        }

        /// <summary>
        /// The name of the specification. Shows up in the documentaion page title and nav bar.
        /// </summary>
        public Swank Named(string title)
        {
            _configuration.Name = title;
            return this;
        }

        /// <summary>
        /// The name of the comments embedded resource. Shows up in the documentaion page. Defaults to "Comments.[md|html|txt]".
        /// </summary>
        public Swank WithComments(string name)
        {
            _configuration.Name = name;
            return this;
        }

        /// <summary>
        /// The copyright which is displayed in the footer of the documentaion page. 
        /// The token {year} is replaced by the current year for use in a copyright.
        /// </summary>
        public Swank WithCopyright(string copyright)
        {
            _configuration.Copyright = copyright.Replace("{year}", DateTime.Now.Year.ToString());
            return this;
        }

        /// <summary>
        /// Specify stylesheets to be included in the documentation page.
        /// This can be used to override the appearance of the page.
        /// </summary>
        public Swank WithStylesheets(params string[] urls)
        {
            _configuration.Stylesheets.AddRange(urls);
            return this;
        }

        /// <summary>
        /// Specify scripts to be included in the documentation page.
        /// This can be used to override the behavior of the page.
        /// </summary>
        public Swank WithScripts(params string[] urls)
        {
            _configuration.Scripts.AddRange(urls);
            return this;
        }

        /// <summary>
        /// Do not display a json example.
        /// </summary>
        public Swank HideJson()
        {
            _configuration.DisplayJson = false;
            return this;
        }

        /// <summary>
        /// Do not display an xml example.
        /// </summary>
        public Swank HideXml()
        {
            _configuration.DisplayXml = false;
            return this;
        }

        /// <summary>
        /// This filters the actions included in the specification.
        /// </summary>
        public Swank Where(Func<ActionCall, bool> filter)
        {
            _configuration.Filter = filter;
            return this;
        }

        /// <summary>
        /// This allows you to set the module convention.
        /// </summary>
        public Swank WithModuleConvention<T>() where T : IDescriptionConvention<ActionCall, ModuleDescription>
        {
            return WithModuleConvention<T, object>(null);
        }

        /// <summary>
        /// This allows you to set the module convention as well as pass in configuration.
        /// </summary>
        public Swank WithModuleConvention<T, TConfig>(Action<TConfig> configure)
            where T : IDescriptionConvention<ActionCall, ModuleDescription>
            where TConfig : class, new()
        {
            _configuration.ModuleConvention.Type = typeof(T);
            _configuration.ModuleConvention.Config = CreateConfig(configure);
            return this;
        }

        /// <summary>
        /// This allows you to set the resource convention.
        /// </summary>
        public Swank WithResourceConvention<T>() where T : IDescriptionConvention<ActionCall, ResourceDescription>
        {
            return WithResourceConvention<T, object>(null);
        }

        /// <summary>
        /// This allows you to set the resource convention as well as pass in configuration.
        /// </summary>
        public Swank WithResourceConvention<T, TConfig>(Action<TConfig> configure)
            where T : IDescriptionConvention<ActionCall, ResourceDescription>
            where TConfig : class, new()
        {
            _configuration.ResourceConvention.Type = typeof(T);
            _configuration.ResourceConvention.Config = CreateConfig(configure);
            return this;
        }

        /// <summary>
        /// This allows you to set the endpoint convention.
        /// </summary>
        public Swank WithEndpointConvention<T>() where T : IDescriptionConvention<ActionCall, EndpointDescription>
        {
            return WithEndpointConvention<T, object>(null);
        }

        /// <summary>
        /// This allows you to set the endpoint convention as well as pass in configuration.
        /// </summary>
        public Swank WithEndpointConvention<T, TConfig>(Action<TConfig> configure)
            where T : IDescriptionConvention<ActionCall, EndpointDescription>
            where TConfig : class, new()
        {
            _configuration.EndpointConvention.Type = typeof(T);
            _configuration.EndpointConvention.Config = CreateConfig(configure);
            return this;
        }

        /// <summary>
        /// This allows you to set the member convention.
        /// </summary>
        public Swank WithMemberConvention<T>() where T : IDescriptionConvention<PropertyInfo, MemberDescription>
        {
            return WithMemberConvention<T, object>(null);
        }

        /// <summary>
        /// This allows you to set the member convention as well as pass in configuration.
        /// </summary>
        public Swank WithMemberConvention<T, TConfig>(Action<TConfig> configure)
            where T : IDescriptionConvention<PropertyInfo, MemberDescription>
            where TConfig : class, new()
        {
            _configuration.MemberConvention.Type = typeof(T);
            _configuration.MemberConvention.Config = CreateConfig(configure);
            return this;
        }

        /// <summary>
        /// This allows you to set the option convention.
        /// </summary>
        public Swank WithOptionConvention<T>() where T : IDescriptionConvention<FieldInfo, OptionDescription>
        {
            return WithOptionConvention<T, object>(null);
        }

        /// <summary>
        /// This allows you to set the option convention as well as pass in configuration.
        /// </summary>
        public Swank WithOptionConvention<T, TConfig>(Action<TConfig> configure)
            where T : IDescriptionConvention<FieldInfo, OptionDescription>
            where TConfig : class, new()
        {
            _configuration.OptionConvention.Type = typeof(T);
            _configuration.OptionConvention.Config = CreateConfig(configure);
            return this;
        }

        /// <summary>
        /// This allows you to set the error convention.
        /// </summary>
        public Swank WithErrorConvention<T>() where T : IDescriptionConvention<ActionCall, List<ErrorDescription>>
        {
            return WithErrorConvention<T, object>(null);
        }

        /// <summary>
        /// This allows you to set the error convention as well as pass in configuration.
        /// </summary>
        public Swank WithErrorConvention<T, TConfig>(Action<TConfig> configure)
            where T : IDescriptionConvention<ActionCall, List<ErrorDescription>>
            where TConfig : class, new()
        {
            _configuration.ErrorConvention.Type = typeof(T);
            _configuration.ErrorConvention.Config = CreateConfig(configure);
            return this;
        }

        /// <summary>
        /// This allows you to set the type convention.
        /// </summary>
        public Swank WithTypeConvention<T>() where T : IDescriptionConvention<Type, TypeDescription>
        {
            return WithTypeConvention<T, object>(null);
        }

        /// <summary>
        /// This allows you to set the type convention as well as pass in configuration.
        /// </summary>
        public Swank WithTypeConvention<T, TConfig>(Action<TConfig> configure)
            where T : IDescriptionConvention<Type, TypeDescription>
            where TConfig : class, new()
        {
            _configuration.TypeConvention.Type = typeof(T);
            _configuration.TypeConvention.Config = CreateConfig(configure);
            return this;
        }

        private TConfig CreateConfig<TConfig>(Action<TConfig> configure) where TConfig : class, new()
        {
            if (configure == null) return null;
            var config = new TConfig();
            configure(config);
            return config;
        }

        /// <summary>
        /// This enables you to define a default module that resources are added to when none are defined for it.
        /// </summary>
        public Swank WithDefaultModule(Func<ActionCall, ModuleDescription> factory)
        {
            _configuration.DefaultModuleFactory = factory;
            return this;
        }

        /// <summary>
        /// This determines what happens when a module is not defined for a resource.
        /// </summary>
        public Swank OnOrphanedModuleAction(OrphanedActions behavior)
        {
            _configuration.OrphanedModuleActions = behavior;
            return this;
        }

        /// <summary>
        /// This enables you to define a default resource that endpoints are added to when none are defined for it.
        /// </summary>
        public Swank WithDefaultResource(Func<ActionCall, ResourceDescription> factory)
        {
            _configuration.DefaultResourceFactory = factory;
            return this;
        }

        /// <summary>
        /// This determines what happens when a resource is not defined for an endpoint.
        /// </summary>
        public Swank OnOrphanedResourceAction(OrphanedActions behavior)
        {
            _configuration.OrphanedResourceActions = behavior;
            return this;
        }
    }
}