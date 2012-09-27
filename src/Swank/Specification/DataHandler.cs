namespace FubuMVC.Swank.Specification
{
    public class DataHandler
    {
        private readonly ISpecificationService _specificationService;

        public DataHandler(ISpecificationService specificationService)
        {
            _specificationService = specificationService;
        }

        public Specification DataExecute()
        {
            return _specificationService.Generate();
        }
    }
}