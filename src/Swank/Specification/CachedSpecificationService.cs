namespace FubuMVC.Swank.Specification
{
    public class CachedSpecificationService : ISpecificationService
    {
        private readonly SpecificationService _specificationService;
        private static Specification _specification;

        public CachedSpecificationService(SpecificationService specificationService)
        {
            _specificationService = specificationService;
        }

        public Specification Generate()
        {
            return _specification ?? (_specification = _specificationService.Generate());
        }
    }
}