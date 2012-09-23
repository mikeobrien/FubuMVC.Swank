namespace FubuMVC.Swank.Specification
{
    public class Handler
    {
        private readonly SpecificationBuilder _specificationBuilder;
        private static Specification _specification;

        public Handler(SpecificationBuilder specificationBuilder)
        {
            _specificationBuilder = specificationBuilder;
        }

        public Specification Execute()
        {
            return _specification ?? (_specification = _specificationBuilder.Build());
        }
    }
}