using System;
using System.Collections.Generic;
using System.Reflection;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Swank.Description;
using FubuMVC.Swank.Extensions;
using FubuMVC.Swank.Specification;
using Module = FubuMVC.Swank.Specification.Module;

namespace FubuMVC.Swank
{
    public enum OrphanedActions { Exclude, Fail, UseDefault }
    public enum EnumFormat { AsNumber, AsString }

    public class Configuration
    {
        public class Service<T>
        {
            public Type Type { get; set; }
            public object Config { get; set; }
        }

        public Configuration()
        {
            Url = "";
            Name = "API";
            Comments = "Comments";
            Copyright = "Copyright &copy; {0}".ToFormat(DateTime.Now.Year);
            Scripts = new List<string>();
            Stylesheets = new List<string>();
            DisplayJsonFormat = true;
            DisplayXmlFormat = true;
            AppliesToAssemblies = new List<Assembly>();
            Filter = x => true;
            DefaultModuleFactory = x => null;
            OrphanedModuleActions = OrphanedActions.UseDefault;
            DefaultResourceFactory = x => new ResourceDescription { Name = x.Route.GetRouteResource() };
            OrphanedResourceActions = OrphanedActions.UseDefault;

            DefaultDictionaryKeyName = "key";

            EnumFormat = EnumFormat.AsNumber;

            CodeExamples = new List<CodeExample>();

            SampleDateTimeFormat = "g";
            SampleIntegerFormat = "0";
            SampleRealFormat = "0.#####";
            SampleTimeSpanFormat = "g";
            SampleGuidFormat = "D";

            SampleStringValue = "";
            SampleBoolValue = false;
            SampleDateTimeValue = DateTime.Now;
            SampleIntegerValue = 0;
            SampleRealValue = 0;
            SampleTimeSpanValue = TimeSpan.FromHours(0);
            SampleGuidValue = Guid.Empty;

            ModuleConvention = new Service<IDescriptionConvention<BehaviorChain, ModuleDescription>> { Type = typeof(ModuleConvention) };
            ResourceConvention = new Service<IDescriptionConvention<BehaviorChain, ResourceDescription>> { Type = typeof(ResourceConvention) };
            EndpointConvention = new Service<IDescriptionConvention<BehaviorChain, EndpointDescription>> { Type = typeof(EndpointConvention) };
            MemberConvention = new Service<IDescriptionConvention<PropertyInfo, MemberDescription>> { Type = typeof(MemberConvention) };
            OptionConvention = new Service<IDescriptionConvention<FieldInfo, OptionDescription>> { Type = typeof(OptionConvention) };
            StatusCodeConvention = new Service<IDescriptionConvention<BehaviorChain, List<StatusCodeDescription>>> { Type = typeof(StatusCodeConvention) };
            HeaderConvention = new Service<IDescriptionConvention<BehaviorChain, List<HeaderDescription>>> { Type = typeof(HeaderConvention) };
            MimeTypeConvention = new Service<IDescriptionConvention<BehaviorChain, List<MimeTypeDescription>>> { Type = typeof(MimeTypeConvention) };
            TypeConvention = new Service<IDescriptionConvention<Type, TypeDescription>> { Type = typeof(TypeConvention) };

            ModuleOverrides = new List<Action<Module>>();
            ResourceOverrides = new List<Action<Resource>>();
            EndpointOverrides = new List<Action<BehaviorChain, Endpoint>>();
            UrlParameterOverrides = new List<Action<BehaviorChain, PropertyInfo, UrlParameter>>();
            QuerystringOverrides = new List<Action<BehaviorChain, PropertyInfo, QuerystringParameter>>();
            StatusCodeOverrides = new List<Action<BehaviorChain, StatusCode>>();
            RequestHeaderOverrides = new List<Action<BehaviorChain, Header>>();
            ResponseHeaderOverrides = new List<Action<BehaviorChain, Header>>();
            RequestOverrides = new List<Action<BehaviorChain, Data>>();
            ResponseOverrides = new List<Action<BehaviorChain, Data>>();
            TypeOverrides = new List<Action<Type, DataType>>();
            MemberOverrides = new List<Action<PropertyInfo, Member>>();
            OptionOverrides = new List<Action<FieldInfo, EnumOption>>();
        }

        public string Url { get; set; }
        public string Name { get; set; }
        public string Comments { get; set; }
        public string Copyright { get; set; }
        public List<string> Scripts { get; set; }
        public List<string> Stylesheets { get; set; }
        public bool DisplayJsonFormat { get; set; }
        public bool DisplayXmlFormat { get; set; }
        public List<Assembly> AppliesToAssemblies { get; set; }
        public Func<BehaviorChain, bool> Filter { get; set; }
        public bool ExcludeAutoBoundProperties { get; set; }

        public OrphanedActions OrphanedModuleActions { get; set; }
        public OrphanedActions OrphanedResourceActions { get; set; }
        public Func<BehaviorChain, ModuleDescription> DefaultModuleFactory { get; set; }
        public Func<BehaviorChain, ResourceDescription> DefaultResourceFactory { get; set; }

        public string DefaultDictionaryKeyName { get; set; }

        public EnumFormat EnumFormat { get; set; }

        public List<CodeExample> CodeExamples { get; set; }

        public string SampleDateTimeFormat { get; set; }
        public string SampleIntegerFormat { get; set; }
        public string SampleRealFormat { get; set; }
        public string SampleTimeSpanFormat { get; set; }
        public string SampleGuidFormat { get; set; }

        public string SampleStringValue { get; set; }
        public bool SampleBoolValue { get; set; }
        public DateTime SampleDateTimeValue { get; set; }
        public int SampleIntegerValue { get; set; }
        public decimal SampleRealValue { get; set; }
        public TimeSpan SampleTimeSpanValue { get; set; }
        public Guid SampleGuidValue { get; set; }

        public Service<IDescriptionConvention<BehaviorChain, ModuleDescription>> ModuleConvention { get; set; }
        public Service<IDescriptionConvention<BehaviorChain, ResourceDescription>> ResourceConvention { get; set; }
        public Service<IDescriptionConvention<BehaviorChain, EndpointDescription>> EndpointConvention { get; set; }
        public Service<IDescriptionConvention<BehaviorChain, List<StatusCodeDescription>>> StatusCodeConvention { get; set; }
        public Service<IDescriptionConvention<BehaviorChain, List<HeaderDescription>>> HeaderConvention { get; set; }
        public Service<IDescriptionConvention<BehaviorChain, List<MimeTypeDescription>>> MimeTypeConvention { get; set; }
        public Service<IDescriptionConvention<Type, TypeDescription>> TypeConvention { get; set; }
        public Service<IDescriptionConvention<PropertyInfo, MemberDescription>> MemberConvention { get; set; }
        public Service<IDescriptionConvention<FieldInfo, OptionDescription>> OptionConvention { get; set; }

        public List<Action<Module>> ModuleOverrides { get; set; }
        public List<Action<Resource>> ResourceOverrides { get; set; }
        public List<Action<BehaviorChain, Endpoint>> EndpointOverrides { get; set; }
        public List<Action<BehaviorChain, PropertyInfo, UrlParameter>> UrlParameterOverrides { get; set; }
        public List<Action<BehaviorChain, PropertyInfo, QuerystringParameter>> QuerystringOverrides { get; set; }
        public List<Action<BehaviorChain, StatusCode>> StatusCodeOverrides { get; set; }
        public List<Action<BehaviorChain, Header>> RequestHeaderOverrides { get; set; }
        public List<Action<BehaviorChain, Header>> ResponseHeaderOverrides { get; set; }
        public List<Action<BehaviorChain, Data>> RequestOverrides { get; set; }
        public List<Action<BehaviorChain, Data>> ResponseOverrides { get; set; }
        public List<Action<Type, DataType>> TypeOverrides { get; set; }
        public List<Action<PropertyInfo, Member>> MemberOverrides { get; set; }
        public List<Action<FieldInfo, EnumOption>> OptionOverrides { get; set; }
    }
}
