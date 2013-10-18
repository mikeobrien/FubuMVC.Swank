using System;
using System.Linq;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Swank.Extensions;
using FubuMVC.Swank.Specification;

namespace FubuMVC.Swank.Description
{
    public class ResourceConvention : IDescriptionConvention<ActionCall, ResourceDescription>
    {
        private readonly MarkerConvention<ResourceDescription> _descriptions;
        private readonly ActionSource _actions;
        private readonly Func<ActionCall, object> _grouping = x => x.ParentChain().Route.GetRouteResource(); 

        public ResourceConvention(MarkerConvention<ResourceDescription> descriptions, ActionSource actions)
        {
            _descriptions = descriptions;
            _actions = actions;
        }

        public virtual ResourceDescription GetDescription(ActionCall action)
        {
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
                .GroupJoin(_actions.GetActions(), x => x.ResourceHandler, x => x.HandlerType,
                           (r, a) => new { r.Resource, r.ResourceHandler, r.ResourceNamespace, Group = a.Any() ? _grouping(a.First()) : null })
                .OrderByDescending(x => x.Group)
                .ThenByDescending(x => x.ResourceNamespace)
                .Where(x => (x.ResourceHandler != null && _grouping(action).Equals(x.Group)) ||
                            (x.ResourceHandler == null && action.HandlerType.Namespace.StartsWith(x.ResourceNamespace)))
                .Select(x => x.Resource)
                .FirstOrDefault();
        }
    }
}