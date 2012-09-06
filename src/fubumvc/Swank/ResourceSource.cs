using FubuMVC.Core.Registration.Nodes;

namespace Swank
{
    public class ResourceDescription<THandler> : ResourceDescription
    {
        public ResourceDescription() { }
        public ResourceDescription(string name, string description) : base(name, description) { }
    }

    public class ResourceDescription : Description
    {
        public ResourceDescription() { }

        public ResourceDescription(string name, string description)
        {
            Name = name;
            Comments = description;
        }
    }

    public interface IResourceSource
    {
        bool HasDescription(ActionCall action);
        Description GetDescription(ActionCall action);
    }

    public class ResourceSource : DescriptionSource<ResourceDescription>, IResourceSource { }
}