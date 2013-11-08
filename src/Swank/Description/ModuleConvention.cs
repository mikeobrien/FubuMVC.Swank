using System.Linq;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Swank.Description
{
    public class ModuleConvention : IDescriptionConvention<BehaviorChain, ModuleDescription>
    {
        private readonly MarkerConvention<ModuleDescription> _descriptions;

        public ModuleConvention(MarkerConvention<ModuleDescription> descriptions)
        {
            _descriptions = descriptions;
        }

        public virtual ModuleDescription GetDescription(BehaviorChain chain)
        {
            return _descriptions.GetDescriptions(chain.FirstCall().HandlerType.Assembly)
                .FirstOrDefault(x => chain.FirstCall().HandlerType.Namespace.StartsWith(x.GetType().Namespace));
        }
    }
}