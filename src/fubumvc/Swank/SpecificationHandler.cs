using Swank.Models;

namespace Swank
{
    public class SpecificationHandler
    {
        private readonly SpecificationBuilder _specificationBuilder;

        public SpecificationHandler(SpecificationBuilder specificationBuilder)
        {
            _specificationBuilder = specificationBuilder;
        }

        public Specification Execute()
        {
            return _specificationBuilder.Build();
        }
    }
}