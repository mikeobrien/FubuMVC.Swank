namespace FubuMVC.Swank
{
    public class SpecificationHandler
    {
        private readonly SpecificationBuilder _specificationBuilder;
        private static Specification _specification;

        public SpecificationHandler(SpecificationBuilder specificationBuilder)
        {
            _specificationBuilder = specificationBuilder;
        }

        public Specification Execute()
        {
            return _specification ?? (_specification = _specificationBuilder.Build());
        }
    }
}