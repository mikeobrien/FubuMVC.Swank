using System.Linq;
using FubuMVC.Core.Registration.Nodes;

namespace Swank
{
    public class ResourceSource : IResourceSource
    {
        private readonly DescriptionSource<Resource> _descriptions;
        private readonly ActionSource _actions;
        private readonly ResourceSourceConfig _config;

        public ResourceSource(DescriptionSource<Resource> descriptions, ActionSource actions, ResourceSourceConfig config)
        {
            _descriptions = descriptions;
            _actions = actions;
            _config = config;
        }

        public bool HasResource(ActionCall action)
        {
            return GetResource(action) != null;
        }

        public Resource GetResource(ActionCall action)
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