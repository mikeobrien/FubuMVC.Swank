using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Swank.Specification
{
    public class TypeContext
    {
        private readonly BehaviorChain _chain;

        public TypeContext(System.Type type, TypeContext parent = null, BehaviorChain chain = null)
        {
            Type = type;
            Parent = parent;
            _chain = chain;
        }

        public System.Type Type { get; private set; }
        public TypeContext Parent { get; private set; }

        public BehaviorChain Chain 
        {
            get
            {
                if (_chain == null && Parent != null)
                {
                    return Parent.Chain;
                }

                return _chain;
            }
        }
    }
}