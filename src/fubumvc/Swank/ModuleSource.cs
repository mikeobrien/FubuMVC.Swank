using FubuMVC.Core.Registration.Nodes;

namespace Swank
{
    public class ModuleDescription : Description
    {
        public ModuleDescription() { }

        public ModuleDescription(string name, string description = null)
        {
            Name = name;
            Comments = description;
        }
    }

    public interface IModuleSource
    {
        bool HasDescription(ActionCall action);
        Description GetDescription(ActionCall action);
    }

    public class ModuleSource : DescriptionSource<ModuleDescription>, IModuleSource { }
}