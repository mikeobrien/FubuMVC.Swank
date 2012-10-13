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
            ModuleDescriptionSource = new Service<IDescriptionSource<ActionCall, ModuleDescription>> { Type = typeof(ModuleSource) };
            ResourceDescriptionSource = new Service<IDescriptionSource<ActionCall, ResourceDescription>> { Type = typeof(ResourceSource) };
            DefaultModuleFactory = x => null;
            OrphanedModuleActions = OrphanedActions.UseDefault;
            DefaultResourceFactory = x => new ResourceDescription { Name = x.ParentChain().Route.GetRouteResource() };
            OrphanedResourceActions = OrphanedActions.UseDefault;
            EndpointDescriptionSource = new Service<IDescriptionSource<ActionCall, EndpointDescription>> { Type = typeof(EndpointSource) };
            MemberDescriptionSource = new Service<IDescriptionSource<PropertyInfo, MemberDescription>> { Type = typeof(MemberSource) };
            OptionDescriptionSource = new Service<IDescriptionSource<FieldInfo, OptionDescription>> { Type = typeof(OptionSource) };
            ErrorDescriptionSource = new Service<IDescriptionSource<ActionCall, List<ErrorDescription>>> { Type = typeof(ErrorSource) };
            TypeDescriptionSource = new Service<IDescriptionSource<Type, TypeDescription>> { Type = typeof(TypeSource) };
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
        public Service<IDescriptionSource<ActionCall, ModuleDescription>> ModuleDescriptionSource { get; set; }
        public Service<IDescriptionSource<ActionCall, ResourceDescription>> ResourceDescriptionSource { get; set; }
        public Service<IDescriptionSource<ActionCall, EndpointDescription>> EndpointDescriptionSource { get; set; }
        public Service<IDescriptionSource<PropertyInfo, MemberDescription>> MemberDescriptionSource { get; set; }
        public Service<IDescriptionSource<FieldInfo, OptionDescription>> OptionDescriptionSource { get; set; }
        public Service<IDescriptionSource<ActionCall, List<ErrorDescription>>> ErrorDescriptionSource { get; set; }
        public Service<IDescriptionSource<Type, TypeDescription>> TypeDescriptionSource { get; set; }
    }
}
