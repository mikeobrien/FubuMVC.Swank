using FubuMVC.Core.Registration.Nodes;

namespace Swank
{
    public class Module : Description { }

    public interface IModuleSource
    {
        bool HasModule(ActionCall action);
        Module GetModule(ActionCall action);
    }
}