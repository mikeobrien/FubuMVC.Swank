using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using FubuMVC.Core;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Swank.Description;
using FubuMVC.Swank.Extensions;

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
        /// The spec should be generated from the specified assemblies.
        /// </summary>
        public Swank AppliesTo(params Assembly[] assemblies)
        {
            _configuration.AppliesToAssemblies.AddRange(assemblies);
            return this;
        }

        /// <summary>
        /// This defines the url of the website endpoint. The default is /.
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
        /// The name of the embedded resource that contains the copy that is displayed on the home page. 
        /// The extension is not required. Defaults to "Comments.[md|html|txt]". 
        /// </summary>
        public Swank WithComments(string name)
        {
            _configuration.Comments = name;
            return this;
        }

        /// <summary>
        /// The copyright which is displayed in the footer of the documentaion page. 
        /// The token {year} is replaced by the current year.
        /// </summary>
        public Swank WithCopyright(string copyright)
        {
            _configuration.Copyright = copyright.Replace("{year}", DateTime.Now.Year.ToString());
            return this;
        }

        /// <summary>
        /// Specify stylesheets to be included in the documentation page.
        /// This can be used to override the appearance of the page.
        /// You can use application relative paths a la ~/styles/style.css.
        /// </summary>
        public Swank WithStylesheets(params string[] urls)
        {
            _configuration.Stylesheets.AddRange(urls);
            return this;
        }

        /// <summary>
        /// Specify scripts to be included in the documentation page.
        /// This can be used to override the behavior of the page.
        /// You can use application relative paths a la ~/scripts/script.js.
        /// </summary>
        public Swank WithScripts(params string[] urls)
        {
            _configuration.Scripts.AddRange(urls);
            return this;
        }

        /// <summary>
        /// Do not display a json representation.
        /// </summary>
        public Swank HideJson()
        {
            _configuration.DisplayJsonFormat = false;
            return this;
        }

        /// <summary>
        /// Do not display an xml representation.
        /// </summary>
        public Swank HideXml()
        {
            _configuration.DisplayXmlFormat = false;
            return this;
        }

        /// <summary>
        /// This filters the actions included in the specification.
        /// </summary>
        public Swank Where(Func<BehaviorChain, bool> filter)
        {
            _configuration.Filter = filter;
            return this;
        }

        /// <summary>
        /// This is the default dictionary key name.
        /// </summary>
        public Swank WithDefaultDictionaryKeyName(string keyName)
        {
            _configuration.DefaultDictionaryKeyName = keyName;
            return this;
        }

        /// <summary>
        /// Indicates whether enum values are represented by a number or string.
        /// </summary>
        public Swank WithEnumFormat(EnumFormat type)
        {
            _configuration.EnumFormat = type;
            return this;
        }

        /// <summary>
        /// Adds a code example to the documentation from an embedded resource. 
        /// These are named as [filename].[md] for comments and [filename].[mustache] 
        /// for templates. The name defaults to the filename if not specified.
        /// </summary>
        public Swank WithCodeExampleResource(string filename, string name = null)
        {
            _configuration.CodeExamples.Add(CodeExample.FromEmbeddedResource(
                _configuration.AppliesToAssemblies, filename, name));
            return this;
        }

        /// <summary>
        /// Adds a code examples to the documentation from a folder. 
        /// These are named as [filename].[md] for comments and [filename].[mustache] 
        /// for templates. The name defaults to the filename if not specified.
        /// </summary>
        public Swank WithCodeExampleFolder(string path)
        {
            _configuration.CodeExamples.AddRange(CodeExample.FromDirectory(HostingEnvironment.MapPath(
                (!path.StartsWith("\\") && !path.StartsWith("\\") ? "~\\" : "") + path)));
            return this;
        }

        /// <summary>
        /// Adds a code example to the documentation.  
        /// </summary>
        public Swank WithCodeExample(string name, string comments, string template)
        {
            _configuration.CodeExamples.Add(new CodeExample
            {
                Name = name,
                Comments = comments,
                Template = template
            });
            return this;
        }

        /// <summary>
        /// This is the format of default and sample DateTime values displayed in the documentation.
        /// </summary>
        public Swank WithSampleDateTimeFormat(string format)
        {
            _configuration.SampleDateTimeFormat = format;
            return this;
        }

        /// <summary>
        /// This is the format of default and sample integer values displayed in the documentation.
        /// </summary>
        public Swank WithSampleIntegerFormat(string format)
        {
            _configuration.SampleIntegerFormat = format;
            return this;
        }

        /// <summary>
        /// This is the format of default and sample real values displayed in the documentation.
        /// </summary>
        public Swank WithSampleRealFormat(string format)
        {
            _configuration.SampleRealFormat = format;
            return this;
        }

        /// <summary>
        /// This is the format of default and sample TimeSpan values displayed in the documentation.
        /// </summary>
        public Swank WithSampleTimeSpanFormat(string format)
        {
            _configuration.SampleTimeSpanFormat = format;
            return this;
        }

        /// <summary>
        /// This is the format of default and sample Guid values displayed in the documentation.
        /// </summary>
        public Swank WithSampleGuidFormat(string format)
        {
            _configuration.SampleGuidFormat = format;
            return this;
        }

        /// <summary>
        /// Sample string value displayed in the documentation.
        /// </summary>
        public Swank WithSampleStringValue(string value)
        {
            _configuration.SampleStringValue = value;
            return this;
        }

        /// <summary>
        /// Sample boolean value displayed in the documentation.
        /// </summary>
        public Swank WithSampleBoolValue(bool value)
        {
            _configuration.SampleBoolValue = value;
            return this;
        }

        /// <summary>
        /// Sample DateTime value displayed in the documentation.
        /// </summary>
        public Swank WithSampleDateTimeValue(DateTime value)
        {
            _configuration.SampleDateTimeValue = value;
            return this;
        }

        /// <summary>
        /// Sample integer value displayed in the documentation.
        /// </summary>
        public Swank WithSampleIntegerValue(int value)
        {
            _configuration.SampleIntegerValue = value;
            return this;
        }

        /// <summary>
        /// Sample real value displayed in the documentation.
        /// </summary>
        public Swank WithSampleRealValue(decimal value)
        {
            _configuration.SampleRealValue = value;
            return this;
        }

        /// <summary>
        /// Sample TimeSpan value displayed in the documentation.
        /// </summary>
        public Swank WithSampleTimeSpanValue(TimeSpan value)
        {
            _configuration.SampleTimeSpanValue = value;
            return this;
        }

        /// <summary>
        /// Sample Guid value displayed in the documentation.
        /// </summary>
        public Swank WithSampleGuidValue(Guid value)
        {
            _configuration.SampleGuidValue = value;
            return this;
        }

        /// <summary>
        /// This allows you to set the module convention.
        /// </summary>
        public Swank WithModuleConvention<T>() where T : IDescriptionConvention<BehaviorChain, ModuleDescription>
        {
            return WithModuleConvention<T, object>(null);
        }

        /// <summary>
        /// This allows you to set the module convention as well as pass in configuration.
        /// </summary>
        public Swank WithModuleConvention<T, TConfig>(Action<TConfig> configure)
            where T : IDescriptionConvention<BehaviorChain, ModuleDescription>
            where TConfig : class, new()
        {
            _configuration.ModuleConvention.Type = typeof(T);
            _configuration.ModuleConvention.Config = CreateConfig(configure);
            return this;
        }

        /// <summary>
        /// This allows you to set the resource convention.
        /// </summary>
        public Swank WithResourceConvention<T>() where T : IDescriptionConvention<BehaviorChain, ResourceDescription>
        {
            return WithResourceConvention<T, object>(null);
        }

        /// <summary>
        /// This allows you to set the resource convention as well as pass in configuration.
        /// </summary>
        public Swank WithResourceConvention<T, TConfig>(Action<TConfig> configure)
            where T : IDescriptionConvention<BehaviorChain, ResourceDescription>
            where TConfig : class, new()
        {
            _configuration.ResourceConvention.Type = typeof(T);
            _configuration.ResourceConvention.Config = CreateConfig(configure);
            return this;
        }

        /// <summary>
        /// This allows you to set the endpoint convention.
        /// </summary>
        public Swank WithEndpointConvention<T>() where T : IDescriptionConvention<BehaviorChain, EndpointDescription>
        {
            return WithEndpointConvention<T, object>(null);
        }

        /// <summary>
        /// This allows you to set the endpoint convention as well as pass in configuration.
        /// </summary>
        public Swank WithEndpointConvention<T, TConfig>(Action<TConfig> configure)
            where T : IDescriptionConvention<BehaviorChain, EndpointDescription>
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
        /// This allows you to set the status code convention.
        /// </summary>
        public Swank WithStatusCodeConvention<T>() where T : IDescriptionConvention<BehaviorChain, List<StatusCodeDescription>>
        {
            return WithStatusCodeConvention<T, object>(null);
        }

        /// <summary>
        /// This allows you to set the status code convention as well as pass in configuration.
        /// </summary>
        public Swank WithStatusCodeConvention<T, TConfig>(Action<TConfig> configure)
            where T : IDescriptionConvention<BehaviorChain, List<StatusCodeDescription>>
            where TConfig : class, new()
        {
            _configuration.StatusCodeConvention.Type = typeof(T);
            _configuration.StatusCodeConvention.Config = CreateConfig(configure);
            return this;
        }

        /// <summary>
        /// This allows you to set the header convention.
        /// </summary>
        public Swank WithHeaderConvention<T>() where T : IDescriptionConvention<BehaviorChain, List<HeaderDescription>>
        {
            return WithHeaderConvention<T, object>(null);
        }

        /// <summary>
        /// This allows you to set the header convention as well as pass in configuration.
        /// </summary>
        public Swank WithHeaderConvention<T, TConfig>(Action<TConfig> configure)
            where T : IDescriptionConvention<BehaviorChain, List<HeaderDescription>>
            where TConfig : class, new()
        {
            _configuration.HeaderConvention.Type = typeof(T);
            _configuration.HeaderConvention.Config = CreateConfig(configure);
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
        /// Allows you to define a default module that resources are added to when none are defined for it.
        /// </summary>
        public Swank WithDefaultModule(Func<BehaviorChain, ModuleDescription> factory)
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
        /// Allows you to define a default resource that endpoints are added to when none are defined for it.
        /// </summary>
        public Swank WithDefaultResource(Func<BehaviorChain, ResourceDescription> factory)
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

        /// <summary>
        /// This indicates that you would like to do it for the lulz.
        /// </summary>
        public Swank DoItForTheLulz()
        {
            return this;
        }

        /// <summary>
        /// Allows you to override module values.
        /// </summary>
        public Swank OverrideModules(Action<Specification.Module> @override)
        {
            _configuration.ModuleOverrides.Add(@override);
            return this;
        }

        /// <summary>
        /// Allows you to override module values when a condition is met.
        /// </summary>
        public Swank OverrideModulesWhen(Action<Specification.Module> @override, 
            Func<Specification.Module, bool> when)
        {
            _configuration.ModuleOverrides.Add(OverrideWhen(@override, when));
            return this;
        }

        /// <summary>
        /// Allows you to override resource values.
        /// </summary>
        public Swank OverrideResources(Action<Specification.Resource> @override)
        {
            _configuration.ResourceOverrides.Add(@override);
            return this;
        }

        /// <summary>
        /// Allows you to override resource values when a condition is met.
        /// </summary>
        public Swank OverrideResourcesWhen(Action<Specification.Resource> @override, 
            Func<Specification.Resource, bool> when)
        {
            _configuration.ResourceOverrides.Add(OverrideWhen(@override, when));
            return this;
        }

        /// <summary>
        /// Allows you to override endpoint values.
        /// </summary>
        public Swank OverrideEndpoints(Action<BehaviorChain, Specification.Endpoint> @override)
        {
            _configuration.EndpointOverrides.Add(@override);
            return this;
        }

        /// <summary>
        /// Allows you to override endpoint values when a condition is met.
        /// </summary>
        public Swank OverrideEndpointsWhen(Action<BehaviorChain, Specification.Endpoint> @override,
            Func<BehaviorChain, Specification.Endpoint, bool> when)
        {
            _configuration.EndpointOverrides.Add(OverrideWhen(@override, when));
            return this;
        }

        /// <summary>
        /// Allows you to override url parameter values.
        /// </summary>
        public Swank OverrideUrlParameters(Action<BehaviorChain, PropertyInfo, Specification.UrlParameter> @override)
        {
            _configuration.UrlParameterOverrides.Add(@override);
            return this;
        }

        /// <summary>
        /// Allows you to override url parameter values when a condition is met.
        /// </summary>
        public Swank OverrideUrlParametersWhen(Action<BehaviorChain, PropertyInfo, Specification.UrlParameter> @override,
            Func<BehaviorChain, PropertyInfo, Specification.UrlParameter, bool> when)
        {
            _configuration.UrlParameterOverrides.Add(OverrideWhen(@override, when));
            return this;
        }

        /// <summary>
        /// Allows you to override querystring values.
        /// </summary>
        public Swank OverrideQuerystring(Action<BehaviorChain, PropertyInfo, Specification.QuerystringParameter> @override)
        {
            _configuration.QuerystringOverrides.Add(@override);
            return this;
        }

        /// <summary>
        /// Allows you to override querystring values when a condition is met.
        /// </summary>
        public Swank OverrideQuerystringWhen(Action<BehaviorChain, PropertyInfo, Specification.QuerystringParameter> @override,
            Func<BehaviorChain, PropertyInfo, Specification.QuerystringParameter, bool> when)
        {
            _configuration.QuerystringOverrides.Add(OverrideWhen(@override, when));
            return this;
        }

        /// <summary>
        /// Allows you to override status code values.
        /// </summary>
        public Swank OverrideStatusCodes(Action<BehaviorChain, Specification.StatusCode> @override)
        {
            _configuration.StatusCodeOverrides.Add(@override);
            return this;
        }

        /// <summary>
        /// Allows you to override status code values when a condition is met.
        /// </summary>
        public Swank OverrideStatusCodesWhen(Action<BehaviorChain, Specification.StatusCode> @override,
            Func<BehaviorChain, Specification.StatusCode, bool> when)
        {
            _configuration.StatusCodeOverrides.Add(OverrideWhen(@override, when));
            return this;
        }

        /// <summary>
        /// Allows you to override header values.
        /// </summary>
        public Swank OverrideHeaders(Action<BehaviorChain, Specification.Header> @override)
        {
            _configuration.RequestHeaderOverrides.Add(@override);
            _configuration.ResponseHeaderOverrides.Add(@override);
            return this;
        }

        /// <summary>
        /// Allows you to override header values when a condition is met.
        /// </summary>
        public Swank OverrideHeadersWhen(Action<BehaviorChain, Specification.Header> @override,
            Func<BehaviorChain, Specification.Header, bool> when)
        {
            _configuration.RequestHeaderOverrides.Add(OverrideWhen(@override, when));
            _configuration.ResponseHeaderOverrides.Add(OverrideWhen(@override, when));
            return this;
        }

        /// <summary>
        /// Allows you to override request header values.
        /// </summary>
        public Swank OverrideRequestHeaders(Action<BehaviorChain, Specification.Header> @override)
        {
            _configuration.RequestHeaderOverrides.Add(@override);
            return this;
        }

        /// <summary>
        /// Allows you to override request header values when a condition is met.
        /// </summary>
        public Swank OverrideRequestHeadersWhen(Action<BehaviorChain, Specification.Header> @override,
            Func<BehaviorChain, Specification.Header, bool> when)
        {
            _configuration.RequestHeaderOverrides.Add(OverrideWhen(@override, when));
            return this;
        }

        /// <summary>
        /// Allows you to override response header values.
        /// </summary>
        public Swank OverrideResponseHeaders(Action<BehaviorChain, Specification.Header> @override)
        {
            _configuration.ResponseHeaderOverrides.Add(@override);
            return this;
        }

        /// <summary>
        /// Allows you to override response header values when a condition is met.
        /// </summary>
        public Swank OverrideResponseHeadersWhen(Action<BehaviorChain, Specification.Header> @override,
            Func<BehaviorChain, Specification.Header, bool> when)
        {
            _configuration.ResponseHeaderOverrides.Add(OverrideWhen(@override, when));
            return this;
        }

        /// <summary>
        /// Allows you to override request values.
        /// </summary>
        public Swank OverrideRequest(Action<BehaviorChain, Specification.Data> @override)
        {
            _configuration.RequestOverrides.Add(@override);
            return this;
        }

        /// <summary>
        /// Allows you to override request values when a condition is met.
        /// </summary>
        public Swank OverrideRequestWhen(Action<BehaviorChain, Specification.Data> @override,
            Func<BehaviorChain, Specification.Data, bool> when)
        {
            _configuration.RequestOverrides.Add(OverrideWhen(@override, when));
            return this;
        }

        /// <summary>
        /// Allows you to override response values.
        /// </summary>
        public Swank OverrideResponse(Action<BehaviorChain, Specification.Data> @override)
        {
            _configuration.ResponseOverrides.Add(@override);
            return this;
        }

        /// <summary>
        /// Allows you to override response values when a condition is met.
        /// </summary>
        public Swank OverrideResponseWhen(Action<BehaviorChain, Specification.Data> @override,
            Func<BehaviorChain, Specification.Data, bool> when)
        {
            _configuration.ResponseOverrides.Add(OverrideWhen(@override, when));
            return this;
        }

        /// <summary>
        /// Allows you to override both request and response values.
        /// </summary>
        public Swank OverrideData(Action<BehaviorChain, Specification.Data> @override)
        {
            return OverrideRequest(@override).OverrideResponse(@override);
        }

        /// <summary>
        /// Allows you to override both request and response values when a condition is met.
        /// </summary>
        public Swank OverrideDataWhen(Action<BehaviorChain, Specification.Data> @override,
            Func<BehaviorChain, Specification.Data, bool> when)
        {
            return OverrideRequestWhen(@override, when).OverrideResponseWhen(@override, when);
        }

        /// <summary>
        /// Allows you to override type values.
        /// </summary>
        public Swank OverrideTypes(Action<Type, Specification.DataType> @override)
        {
            _configuration.TypeOverrides.Add(@override);
            return this;
        }

        /// <summary>
        /// Allows you to override type values when a condition is met.
        /// </summary>
        public Swank OverrideTypesWhen(Action<Type, Specification.DataType> @override,
            Func<Type, Specification.DataType, bool> when)
        {
            _configuration.TypeOverrides.Add(OverrideWhen(@override, when));
            return this;
        }

        /// <summary>
        /// Allows you to override member values.
        /// </summary>
        public Swank OverrideMembers(Action<PropertyInfo, Specification.Member> @override)
        {
            _configuration.MemberOverrides.Add(@override);
            return this;
        }

        /// <summary>
        /// Allows you to override member values when a condition is met.
        /// </summary>
        public Swank OverrideMembersWhen(Action<PropertyInfo, Specification.Member> @override,
            Func<PropertyInfo, Specification.Member, bool> when)
        {
            _configuration.MemberOverrides.Add(OverrideWhen(@override, when));
            return this;
        }

        /// <summary>
        /// Allows you to override option values.
        /// </summary>
        public Swank OverrideOptions(Action<FieldInfo, Specification.Option> @override)
        {
            _configuration.OptionOverrides.Add(@override);
            return this;
        }

        /// <summary>
        /// Allows you to override option values when a condition is met.
        /// </summary>
        public Swank OverrideOptionsWhen(Action<FieldInfo, Specification.Option> @override,
            Func<FieldInfo, Specification.Option, bool> when)
        {
            _configuration.OptionOverrides.Add(OverrideWhen(@override, when));
            return this;
        }

        /// <summary>
        /// Allows you to override property values. Properties include members, url parameters and querystring parameters.
        /// </summary>
        public Swank OverrideProperties(Action<PropertyInfo, Specification.IDescription> @override)
        {
            return OverrideMembers(@override)
                .OverrideQuerystring((x, y, z) => @override(y, z))
                .OverrideUrlParameters((x, y, z) => @override(y, z));
        }

        /// <summary>
        /// Allows you to override property values when a condition is met. Properties include members, url parameters and querystring parameters.
        /// </summary>
        public Swank OverridePropertiesWhen(Action<PropertyInfo, Specification.IDescription> @override,
            Func<PropertyInfo, Specification.IDescription, bool> when)
        {
            return OverrideMembersWhen(@override, @when)
                .OverrideQuerystringWhen((x, y, z) => @override(y, z), (x, y, z) => when(y, z))
                .OverrideUrlParametersWhen((x, y, z) => @override(y, z), (x, y, z) => when(y, z));
        }

        private static Action<T> OverrideWhen<T>(Action<T> @override, Func<T, bool> when)
        {
            return x => { if (when(x)) @override(x); };
        }

        private static Action<T1, T2> OverrideWhen<T1, T2>(Action<T1, T2> @override, Func<T1, T2, bool> when)
        {
            return (x, y) => { if (when(x, y)) @override(x, y); };
        }

        private static Action<T1, T2, T3> OverrideWhen<T1, T2, T3>(Action<T1, T2, T3> @override, Func<T1, T2, T3, bool> when)
        {
            return (x, y, z) => { if (when(x, y, z)) @override(x, y, z); };
        }
    }
}