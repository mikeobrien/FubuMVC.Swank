namespace FubuMVC.Swank.Specification
{
    public class DataGetHandler
    {
        private readonly ISpecificationService _specificationService;

        public DataGetHandler(ISpecificationService specificationService)
        {
            _specificationService = specificationService;
        }

        public Specification DataExecute()
        {
            return _specificationService.Generate();
        }
    }
}