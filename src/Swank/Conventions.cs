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
                .HomeIs<Handler>(x => x.Execute())
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
                x.AddService<ISpecificationService, SpecificationService>();
                x.AddService<IDescriptionSource<ActionCall, ModuleDescription>>(configuration.ModuleDescriptionSource.Type, configuration.ModuleDescriptionSource.Config);
                x.AddService<IDescriptionSource<ActionCall, ResourceDescription>>(configuration.ResourceDescriptionSource.Type, configuration.ResourceDescriptionSource.Config);
                x.AddService<IDescriptionSource<ActionCall, EndpointDescription>>(configuration.EndpointDescriptionSource.Type, configuration.EndpointDescriptionSource.Config);
                x.AddService<IDescriptionSource<PropertyInfo, MemberDescription>>(configuration.MemberDescriptionSource.Type, configuration.MemberDescriptionSource.Config);
                x.AddService<IDescriptionSource<FieldInfo, OptionDescription>>(configuration.OptionDescriptionSource.Type, configuration.OptionDescriptionSource.Config);
                x.AddService<IDescriptionSource<ActionCall, List<ErrorDescription>>>(configuration.ErrorDescriptionSource.Type, configuration.ErrorDescriptionSource.Config);
                x.AddService<IDescriptionSource<System.Type, DataTypeDescription>>(configuration.TypeDescriptionSource.Type, configuration.TypeDescriptionSource.Config);
            });
        }
    }
}