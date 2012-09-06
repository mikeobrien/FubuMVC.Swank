using System.Linq;
using FubuMVC.Core.Registration.Nodes;

namespace Swank
{
    public class ModuleSource : IModuleSource
    {
        private readonly DescriptionSource<Module> _descriptions;

        public ModuleSource(DescriptionSource<Module> descriptions)
        {
            _descriptions = descriptions;
        }

        public bool HasModule(ActionCall action)
        {
            return GetModule(action) != null;
        }

        public Module GetModule(ActionCall action)
        {
            return _descriptions.GetDescriptions(action.HandlerType.Assembly)
                .FirstOrDefault(x => action.HandlerType.Namespace.StartsWith(x.Namespace));
        }
    }
}