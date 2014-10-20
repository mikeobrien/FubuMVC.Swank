using System.Collections.Generic;
using System.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Swank.Description;
using FubuMVC.Swank.Documentation;
using FubuMVC.Swank.Endpoints;
using FubuMVC.Swank.Extensions;

[assembly: FubuModule]

namespace FubuMVC.Swank
{
    public class Conventions : FubuRegistry
    {
        public Conventions(Configuration configuration)
        {
            Actions.FindBy(x => {
                x.Applies.ToThisAssembly();
                x.IncludeTypesNamed(y => y.EndsWith("Handler"));
            });
            
            Routes
                .HomeIs<GetHandler>(x => x.Execute())
                .IgnoreMethodSuffix("Execute")
                .IgnoreControllerNamesEntirely()
                .IgnoreControllerNamespaceEntirely()
                .ConstrainToHttpMethod(action => action.HandlerType.Name.EndsWith("GetHandler"), "GET");

            Services(x =>
            {
                x.AddService(configuration);
                x.AddService<IDescriptionConvention<BehaviorChain, ModuleDescription>>(configuration.ModuleConvention.Type, configuration.ModuleConvention.Config)
                 .AddService<IDescriptionConvention<BehaviorChain, ResourceDescription>>(configuration.ResourceConvention.Type, configuration.ResourceConvention.Config)
                 .AddService<IDescriptionConvention<BehaviorChain, EndpointDescription>>(configuration.EndpointConvention.Type, configuration.EndpointConvention.Config)
                 .AddService<IDescriptionConvention<PropertyInfo, MemberDescription>>(configuration.MemberConvention.Type, configuration.MemberConvention.Config)
                 .AddService<IDescriptionConvention<FieldInfo, OptionDescription>>(configuration.OptionConvention.Type, configuration.OptionConvention.Config)
                 .AddService<IDescriptionConvention<BehaviorChain, List<StatusCodeDescription>>>(configuration.StatusCodeConvention.Type, configuration.StatusCodeConvention.Config)
                 .AddService<IDescriptionConvention<BehaviorChain, List<HeaderDescription>>>(configuration.HeaderConvention.Type, configuration.HeaderConvention.Config)
                 .AddService<IDescriptionConvention<BehaviorChain, List<MimeTypeDescription>>>(configuration.MimeTypeConvention.Type, configuration.MimeTypeConvention.Config)
                 .AddService<IDescriptionConvention<System.Type, TypeDescription>>(configuration.TypeConvention.Type, configuration.TypeConvention.Config);
            });
        }
    }
}