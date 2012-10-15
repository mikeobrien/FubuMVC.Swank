using System;
using System.Collections.Generic;
using System.Reflection;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Swank.Description;
using FubuMVC.Swank.Extensions;

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
            ModuleConvention = new Service<IDescriptionConvention<ActionCall, ModuleDescription>> { Type = typeof(ModuleConvention) };
            ResourceConvention = new Service<IDescriptionConvention<ActionCall, ResourceDescription>> { Type = typeof(ResourceConvention) };
            EndpointConvention = new Service<IDescriptionConvention<ActionCall, EndpointDescription>> { Type = typeof(EndpointConvention) };
            MemberConvention = new Service<IDescriptionConvention<PropertyInfo, MemberDescription>> { Type = typeof(MemberConvention) };
            OptionConvention = new Service<IDescriptionConvention<FieldInfo, OptionDescription>> { Type = typeof(OptionConvention) };
            ErrorConvention = new Service<IDescriptionConvention<ActionCall, List<ErrorDescription>>> { Type = typeof(ErrorConvention) };
            TypeConvention = new Service<IDescriptionConvention<Type, TypeDescription>> { Type = typeof(TypeConvention) };
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
        public Service<IDescriptionConvention<ActionCall, ModuleDescription>> ModuleConvention { get; set; }
        public Service<IDescriptionConvention<ActionCall, ResourceDescription>> ResourceConvention { get; set; }
        public Service<IDescriptionConvention<ActionCall, EndpointDescription>> EndpointConvention { get; set; }
        public Service<IDescriptionConvention<PropertyInfo, MemberDescription>> MemberConvention { get; set; }
        public Service<IDescriptionConvention<FieldInfo, OptionDescription>> OptionConvention { get; set; }
        public Service<IDescriptionConvention<ActionCall, List<ErrorDescription>>> ErrorConvention { get; set; }
        public Service<IDescriptionConvention<Type, TypeDescription>> TypeConvention { get; set; }
    }
}
