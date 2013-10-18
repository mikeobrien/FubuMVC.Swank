using System.Linq;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Swank.Description
{
    public class ModuleConvention : IDescriptionConvention<ActionCall, ModuleDescription>
    {
        private readonly MarkerConvention<ModuleDescription> _descriptions;

        public ModuleConvention(MarkerConvention<ModuleDescription> descriptions)
        {
            _descriptions = descriptions;
        }

        public virtual ModuleDescription GetDescription(ActionCall action)
        {
            return _descriptions.GetDescriptions(action.HandlerType.Assembly)
                .FirstOrDefault(x => action.HandlerType.Namespace.StartsWith(x.GetType().Namespace));
        }
    }
}