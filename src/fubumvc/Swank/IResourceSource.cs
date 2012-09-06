using FubuMVC.Core.Registration.Nodes;

namespace Swank
{
    public class Resource<THandler> : Resource { }
    public class Resource : Description { }

    public interface IResourceSource
    {
        bool HasResource(ActionCall action);
        Resource GetResource(ActionCall action);
    }
}