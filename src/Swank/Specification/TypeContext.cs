using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Swank.Specification
{
    public class TypeContext
    {
        public TypeContext(System.Type type, TypeContext parent = null, BehaviorChain chain = null)
        {
            Type = type;
            Chain = chain;
            Parent = parent;
        }

        public System.Type Type { get; private set; }
        public TypeContext Parent { get; private set; }
        public BehaviorChain Chain { get; private set; }
    }
}