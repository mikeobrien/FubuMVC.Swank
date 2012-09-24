namespace FubuMVC.Swank.Specification
{
    public class Handler
    {
        private readonly SpecificationService _SpecificationService;
        private static Specification _specification;

        public Handler(SpecificationService SpecificationService)
        {
            _SpecificationService = SpecificationService;
        }

        public Specification Execute()
        {
            return _specification ?? (_specification = _SpecificationService.Generate());
        }
    }
}