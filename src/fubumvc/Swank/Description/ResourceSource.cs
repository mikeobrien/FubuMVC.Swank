using System;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;

namespace Swank.Description
{
    public class ResourceSourceConfig
    {
        public ResourceSourceConfig()
        {
            Grouping = x => x.ParentChain().Route.GetRouteResource();
        }

        public Func<ActionCall, object> Grouping { get; private set; }

        public ResourceSourceConfig GroupBy(Func<ActionCall, object> grouping)
        {
            Grouping = grouping;
            return this;
        }
    }

    public class ResourceSource : IDescriptionSource<ActionCall, ResourceDescription>
    {
        private readonly MarkerSource<ResourceDescription> _descriptions;
        private readonly ActionSource _actions;
        private readonly ResourceSourceConfig _config;

        public ResourceSource(MarkerSource<ResourceDescription> descriptions, ActionSource actions, ResourceSourceConfig config)
        {
            _descriptions = descriptions;
            _actions = actions;
            _config = config;
        }

        public ResourceDescription GetDescription(ActionCall action)
        {
            return _descriptions.GetDescriptions(action.HandlerType.Assembly)
                .Select(x => new {
                        ResourceHandler = x.GetType().BaseType.GetGenericArguments().FirstOrDefault(),
                        ResourceNamespace = x.GetType().Namespace,
                        Resource = x
                    })
                .GroupJoin(_actions.GetActions(), x => x.ResourceHandler, x => x.HandlerType,
                           (r, a) => new { r.Resource, r.ResourceHandler, r.ResourceNamespace, Group = a.Any() ? _config.Grouping(a.First()) : null })
                .OrderByDescending(x => x.Group)
                .ThenByDescending(x => x.ResourceNamespace)
                .Where(x => (x.ResourceHandler != null && _config.Grouping(action).Equals(x.Group)) ||
                            (x.ResourceHandler == null && action.HandlerType.Namespace.StartsWith(x.ResourceNamespace)))
                .Select(x => x.Resource)
                .FirstOrDefault();
        }
    }
}