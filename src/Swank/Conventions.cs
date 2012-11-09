using System.Collections.Generic;
using System.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Spark;
using FubuMVC.Swank.Description;
using FubuMVC.Swank.Extensions;
using FubuMVC.Swank.Specification;

namespace FubuMVC.Swank
{
    public class Conventions : FubuRegistry
    {
        public Conventions(Configuration configuration)
        {
            Actions.IncludeTypesNamed(x => x.EndsWith("Handler"));
            
            Routes
                .HomeIs<ViewHandler>(x => x.Execute())
                .IgnoreMethodSuffix("Execute")
                .IgnoreControllerNamesEntirely()
                .IgnoreControllerNamespaceEntirely()
                .ConstrainToHttpMethod(action => action.Method.Name.EndsWith("Get"), "GET");

            Import<SparkEngine>();
            Views.TryToAttachWithDefaultConventions();

            Media.ApplyContentNegotiationToActions(x => x.HandlerType.Assembly == GetType().Assembly && !x.HasAnyOutputBehavior());

            Services(x =>
            {
                x.AddService(configuration);
                x.AddService<ISpecificationService, CachedSpecificationService>();
                x.AddService<IDescriptionConvention<ActionCall, ModuleDescription>>(configuration.ModuleConvention.Type, configuration.ModuleConvention.Config);
                x.AddService<IDescriptionConvention<ActionCall, ResourceDescription>>(configuration.ResourceConvention.Type, configuration.ResourceConvention.Config);
                x.AddService<IDescriptionConvention<ActionCall, EndpointDescription>>(configuration.EndpointConvention.Type, configuration.EndpointConvention.Config);
                x.AddService<IDescriptionConvention<PropertyInfo, MemberDescription>>(configuration.MemberConvention.Type, configuration.MemberConvention.Config);
                x.AddService<IDescriptionConvention<FieldInfo, OptionDescription>>(configuration.OptionConvention.Type, configuration.OptionConvention.Config);
                x.AddService<IDescriptionConvention<ActionCall, List<ErrorDescription>>>(configuration.ErrorConvention.Type, configuration.ErrorConvention.Config);
                x.AddService<IDescriptionConvention<ActionCall, List<HeaderDescription>>>(configuration.HeaderConvention.Type, configuration.HeaderConvention.Config);
                x.AddService<IDescriptionConvention<System.Type, TypeDescription>>(configuration.TypeConvention.Type, configuration.TypeConvention.Config);
            });
        }
    }
}