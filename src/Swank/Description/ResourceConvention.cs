using System;
using System.Linq;
using System.Reflection;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Swank.Extensions;
using FubuMVC.Swank.Specification;

namespace FubuMVC.Swank.Description
{
    public class ResourceConvention : IDescriptionConvention<BehaviorChain, ResourceDescription>
    {
        private readonly MarkerConvention<ResourceDescription> _descriptions;
        private readonly BehaviorSource _behaviors;
        private readonly Func<ActionCall, object> _grouping = x => x.ParentChain().Route.GetRouteResource(); 

        public ResourceConvention(MarkerConvention<ResourceDescription> descriptions, BehaviorSource behaviors)
        {
            _descriptions = descriptions;
            _behaviors = behaviors;
        }

        public virtual ResourceDescription GetDescription(BehaviorChain chain)
        {
            var action = chain.FirstCall();
            if (action.HandlerType.HasAttribute<ResourceAttribute>())
            {
                var resource = action.HandlerType.GetCustomAttribute<ResourceAttribute>();
                return new ResourceDescription {
                        Name = resource.Name,
                        Comments = resource.Comments ??
                            action.HandlerType.Assembly.FindTextResourceNamed(action.HandlerType.FullName)
                    };
            }
            return _descriptions.GetDescriptions(action.HandlerType.Assembly)
                .Select(x => new {
                        ResourceHandler = x.GetType().BaseType.GetGenericArguments().FirstOrDefault(),
                        ResourceNamespace = x.GetType().Namespace,
                        Resource = x
                    })
                .GroupJoin(_behaviors.GetChains(), x => x.ResourceHandler, x => x.FirstCall().HandlerType,
                           (r, a) => new { r.Resource, r.ResourceHandler, r.ResourceNamespace, Group = a.Any() ? _grouping(a.First().FirstCall()) : null })
                .OrderByDescending(x => x.Group)
                .ThenByDescending(x => x.ResourceNamespace)
                .Where(x => (x.ResourceHandler != null && _grouping(action).Equals(x.Group)) ||
                            (x.ResourceHandler == null && action.HandlerType.Namespace.StartsWith(x.ResourceNamespace)))
                .Select(x => x.Resource)
                .FirstOrDefault();
        }
    }
}