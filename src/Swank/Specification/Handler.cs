namespace FubuMVC.Swank.Specification
{
    public class Handler
    {
        private readonly ISpecificationService _specificationService;
        private static Specification _specification;

        public Handler(ISpecificationService specificationService)
        {
            _specificationService = specificationService;
        }

        public Specification Execute()
        {
            return _specification ?? (_specification = _specificationService.Generate());
        }
    }
}