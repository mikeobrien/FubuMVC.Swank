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
    public enum EnumValue { AsNumber, AsString }

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
            DefaultResourceFactory = x => new ResourceDescription { Name = x.Route.GetRouteResource() };
            OrphanedResourceActions = OrphanedActions.UseDefault;

            EnumValue = EnumValue.AsNumber;

            DefaultValueDateTimeFormat = "g";
            DefaultValueIntegerFormat = "0";
            DefaultValueRealFormat = "0.0";
            DefaultValueTimeSpanFormat = "g";
            DefaultValueGuidFormat = "D";

            SampleDateTimeValue = DateTime.Now;
            SampleIntegerValue = 0;
            SampleRealValue = 0;
            SampleTimeSpanValue = TimeSpan.FromHours(0);
            SampleGuidValue = Guid.Empty;

            TypeIdConvention = x => x.GetHash();
            InputTypeIdConvention = (t, m) => t.GetHash(m);

            ModuleConvention = new Service<IDescriptionConvention<ActionCall, ModuleDescription>> { Type = typeof(ModuleConvention) };
            ResourceConvention = new Service<IDescriptionConvention<ActionCall, ResourceDescription>> { Type = typeof(ResourceConvention) };
            EndpointConvention = new Service<IDescriptionConvention<ActionCall, EndpointDescription>> { Type = typeof(EndpointConvention) };
            MemberConvention = new Service<IDescriptionConvention<PropertyInfo, MemberDescription>> { Type = typeof(MemberConvention) };
            OptionConvention = new Service<IDescriptionConvention<FieldInfo, OptionDescription>> { Type = typeof(OptionConvention) };
            StatusCodeConvention = new Service<IDescriptionConvention<ActionCall, List<StatusCodeDescription>>> { Type = typeof(StatusCodeConvention) };
            HeaderConvention = new Service<IDescriptionConvention<ActionCall, List<HeaderDescription>>> { Type = typeof(HeaderConvention) };
            TypeConvention = new Service<IDescriptionConvention<Type, TypeDescription>> { Type = typeof(TypeConvention) };

            ModuleOverrides = new List<Action<Module>>();
            ResourceOverrides = new List<Action<Resource>>();
            EndpointOverrides = new List<Action<BehaviorChain, Endpoint>>();
            UrlParameterOverrides = new List<Action<ActionCall, PropertyInfo, UrlParameter>>();
            QuerystringOverrides = new List<Action<ActionCall, PropertyInfo, QuerystringParameter>>();
            StatusCodeOverrides = new List<Action<ActionCall, StatusCode>>();
            HeaderOverrides = new List<Action<ActionCall, Header>>();
            RequestOverrides = new List<Action<BehaviorChain, Data>>();
            ResponseOverrides = new List<Action<BehaviorChain, Data>>();
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
        public Func<BehaviorChain, bool> Filter { get; set; }

        public OrphanedActions OrphanedModuleActions { get; set; }
        public OrphanedActions OrphanedResourceActions { get; set; }
        public Func<BehaviorChain, ModuleDescription> DefaultModuleFactory { get; set; }
        public Func<BehaviorChain, ResourceDescription> DefaultResourceFactory { get; set; }

        public EnumValue EnumValue { get; set; }

        public string DefaultValueDateTimeFormat { get; set; }
        public string DefaultValueIntegerFormat { get; set; }
        public string DefaultValueRealFormat { get; set; }
        public string DefaultValueTimeSpanFormat { get; set; }
        public string DefaultValueGuidFormat { get; set; }

        public DateTime SampleDateTimeValue { get; set; }
        public int SampleIntegerValue { get; set; }
        public decimal SampleRealValue { get; set; }
        public TimeSpan SampleTimeSpanValue { get; set; }
        public Guid SampleGuidValue { get; set; }

        public Func<Type, string> TypeIdConvention { get; set; }
        public Func<Type, MethodInfo, string> InputTypeIdConvention { get; set; }

        public Service<IDescriptionConvention<ActionCall, ModuleDescription>> ModuleConvention { get; set; }
        public Service<IDescriptionConvention<ActionCall, ResourceDescription>> ResourceConvention { get; set; }
        public Service<IDescriptionConvention<ActionCall, EndpointDescription>> EndpointConvention { get; set; }
        public Service<IDescriptionConvention<ActionCall, List<StatusCodeDescription>>> StatusCodeConvention { get; set; }
        public Service<IDescriptionConvention<ActionCall, List<HeaderDescription>>> HeaderConvention { get; set; }
        public Service<IDescriptionConvention<Type, TypeDescription>> TypeConvention { get; set; }
        public Service<IDescriptionConvention<PropertyInfo, MemberDescription>> MemberConvention { get; set; }
        public Service<IDescriptionConvention<FieldInfo, OptionDescription>> OptionConvention { get; set; }

        public List<Action<Module>> ModuleOverrides { get; set; }
        public List<Action<Resource>> ResourceOverrides { get; set; }
        public List<Action<BehaviorChain, Endpoint>> EndpointOverrides { get; set; }
        public List<Action<ActionCall, PropertyInfo, UrlParameter>> UrlParameterOverrides { get; set; }
        public List<Action<ActionCall, PropertyInfo, QuerystringParameter>> QuerystringOverrides { get; set; }
        public List<Action<ActionCall, StatusCode>> StatusCodeOverrides { get; set; }
        public List<Action<ActionCall, Header>> HeaderOverrides { get; set; }
        public List<Action<BehaviorChain, Data>> RequestOverrides { get; set; }
        public List<Action<BehaviorChain, Data>> ResponseOverrides { get; set; }
        public List<Action<Type, Specification.Type>> TypeOverrides { get; set; }
        public List<Action<PropertyInfo, Member>> MemberOverrides { get; set; }
        public List<Action<FieldInfo, Option>> OptionOverrides { get; set; }
    }
}
