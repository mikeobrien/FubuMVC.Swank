using System.Linq;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Swank.Description
{
    public class ModuleSource : IDescriptionSource<ActionCall, ModuleDescription>
    {
        private readonly MarkerSource<ModuleDescription> _descriptions;

        public ModuleSource(MarkerSource<ModuleDescription> descriptions)
        {
            _descriptions = descriptions;
        }

        public ModuleDescription GetDescription(ActionCall action)
        {
            return _descriptions.GetDescriptions(action.HandlerType.Assembly)
                .FirstOrDefault(x => action.HandlerType.Namespace.StartsWith(x.GetType().Namespace));
        }
    }
}