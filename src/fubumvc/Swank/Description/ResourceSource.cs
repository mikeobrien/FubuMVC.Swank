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
                .GroupJoin(_actions.GetActions(), x => x.AppliesTo, x => x.HandlerType,
                           (r, a) => new { Resource = r, Group = a.Any() ? _config.Grouping(a.First()) : null })
                .OrderByDescending(x => x.Group)
                .ThenByDescending(x => x.Resource.Namespace)
                .Where(x => (x.Resource.AppliesTo != null && _config.Grouping(action).Equals(x.Group)) ||
                            (x.Resource.AppliesTo == null && action.HandlerType.Namespace.StartsWith(x.Resource.Namespace)))
                .Select(x => x.Resource)
                .FirstOrDefault();
        }
    }
}