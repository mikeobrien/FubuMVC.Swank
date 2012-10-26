using System;
using System.Collections.Generic;
using System.Reflection;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Swank.Description;
using FubuMVC.Swank.Extensions;
using FubuMVC.Swank.Specification;
using Module = FubuMVC.Swank.Specification.Module;
using Type = System.Type;

namespace FubuMVC.Swank
{
    public enum OrphanedActions { Exclude, Fail, UseDefault }

    public class Configuration
    {
        public class Service<T>
        {
            public Type Type { get; set; }
            public object Config { get; set; }
        }

        public Configuration()
        {
            Url = "specification";
            Name = "API";
            Comments = "Comments";
            Copyright = "Copyright &copy; {0}".ToFormat(DateTime.Now.Year);
            Scripts = new List<string>();
            Stylesheets = new List<string>();
            DisplayJson = true;
            DisplayXml = true;
            AppliesToAssemblies = new List<Assembly>();
            Filter = x => true;
            DefaultModuleFactory = x => null;
            OrphanedModuleActions = OrphanedActions.UseDefault;
            DefaultResourceFactory = x => new ResourceDescription { Name = x.ParentChain().Route.GetRouteResource() };
            OrphanedResourceActions = OrphanedActions.UseDefault;

            TypeIdConvention = x => x.GetHash();
            InputTypeIdConvention = (t, m) => t.GetHash(m);

            ModuleConvention = new Service<IDescriptionConvention<ActionCall, ModuleDescription>> { Type = typeof(ModuleConvention) };
            ResourceConvention = new Service<IDescriptionConvention<ActionCall, ResourceDescription>> { Type = typeof(ResourceConvention) };
            EndpointConvention = new Service<IDescriptionConvention<ActionCall, EndpointDescription>> { Type = typeof(EndpointConvention) };
            MemberConvention = new Service<IDescriptionConvention<PropertyInfo, MemberDescription>> { Type = typeof(MemberConvention) };
            OptionConvention = new Service<IDescriptionConvention<FieldInfo, OptionDescription>> { Type = typeof(OptionConvention) };
            ErrorConvention = new Service<IDescriptionConvention<ActionCall, List<ErrorDescription>>> { Type = typeof(ErrorConvention) };
            TypeConvention = new Service<IDescriptionConvention<Type, TypeDescription>> { Type = typeof(TypeConvention) };

            ModuleOverrides = new List<Action<Module>>();
            ResourceOverrides = new List<Action<Resource>>();
            EndpointOverrides = new List<Action<ActionCall, Endpoint>>();
            UrlParameterOverrides = new List<Action<ActionCall, PropertyInfo, UrlParameter>>();
            QuerystringOverrides = new List<Action<ActionCall, PropertyInfo, QuerystringParameter>>();
            ErrorOverrides = new List<Action<ActionCall, Error>>();
            RequestOverrides = new List<Action<ActionCall, Data>>();
            ResponseOverrides = new List<Action<ActionCall, Data>>();
            TypeOverrides = new List<Action<Type, Specification.Type>>();
            MemberOverrides = new List<Action<PropertyInfo, Member>>();
            OptionOverrides = new List<Action<FieldInfo, Option>>();
        }
        
        public string Url { get; set; }
        public string Name { get; set; }
        public string Comments { get; set; }
        public string Copyright { get; set; }
        public List<string> Scripts { get; set; }
        public List<string> Stylesheets { get; set; }
        public bool DisplayJson { get; set; }
        public bool DisplayXml { get; set; }
        public string MergeSpecificationPath { get; set; }
        public List<Assembly> AppliesToAssemblies { get; set; }
        public Func<ActionCall, bool> Filter { get; set; }

        public OrphanedActions OrphanedModuleActions { get; set; }
        public OrphanedActions OrphanedResourceActions { get; set; }
        public Func<ActionCall, ModuleDescription> DefaultModuleFactory { get; set; }
        public Func<ActionCall, ResourceDescription> DefaultResourceFactory { get; set; }

        public Func<Type, string> TypeIdConvention { get; set; }
        public Func<Type, MethodInfo, string> InputTypeIdConvention { get; set; }

        public Service<IDescriptionConvention<ActionCall, ModuleDescription>> ModuleConvention { get; set; }
        public Service<IDescriptionConvention<ActionCall, ResourceDescription>> ResourceConvention { get; set; }
        public Service<IDescriptionConvention<ActionCall, EndpointDescription>> EndpointConvention { get; set; }
        public Service<IDescriptionConvention<ActionCall, List<ErrorDescription>>> ErrorConvention { get; set; }
        public Service<IDescriptionConvention<Type, TypeDescription>> TypeConvention { get; set; }
        public Service<IDescriptionConvention<PropertyInfo, MemberDescription>> MemberConvention { get; set; }
        public Service<IDescriptionConvention<FieldInfo, OptionDescription>> OptionConvention { get; set; }

        public List<Action<Module>> ModuleOverrides { get; set; }
        public List<Action<Resource>> ResourceOverrides { get; set; }
        public List<Action<ActionCall, Endpoint>> EndpointOverrides { get; set; }
        public List<Action<ActionCall, PropertyInfo, UrlParameter>> UrlParameterOverrides { get; set; }
        public List<Action<ActionCall, PropertyInfo, QuerystringParameter>> QuerystringOverrides { get; set; }
        public List<Action<ActionCall, Error>> ErrorOverrides { get; set; }
        public List<Action<ActionCall, Data>> RequestOverrides { get; set; }
        public List<Action<ActionCall, Data>> ResponseOverrides { get; set; }
        public List<Action<Type, Specification.Type>> TypeOverrides { get; set; }
        public List<Action<PropertyInfo, Member>> MemberOverrides { get; set; }
        public List<Action<FieldInfo, Option>> OptionOverrides { get; set; }
    }
}
