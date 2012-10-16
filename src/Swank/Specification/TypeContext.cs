using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Swank.Specification
{
    public class TypeContext
    {
        public TypeContext(System.Type type, TypeContext parent = null, ActionCall action = null)
        {
            Type = type;
            Action = action;
            Parent = parent;
        }

        public System.Type Type { get; private set; }
        public TypeContext Parent { get; private set; }
        public ActionCall Action { get; private set; }
    }
}