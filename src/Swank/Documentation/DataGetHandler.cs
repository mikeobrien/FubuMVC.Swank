using FubuMVC.Swank.Specification;

namespace FubuMVC.Swank.Documentation
{
    public class DataGetHandler
    {
        private readonly LazyCache<Specification.Specification> _specification =
            new LazyCache<Specification.Specification>();

        public DataGetHandler(
            SpecificationService specificationService)
        {
            _specification.UseFactory(specificationService.Generate);
        }

        public Specification.Specification SpecExecute()
        {
            return _specification.Value;
        }
    }
}