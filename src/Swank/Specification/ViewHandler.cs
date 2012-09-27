namespace FubuMVC.Swank.Specification
{
    public class HandlerResponse
    {
        public Specification Specification { get; set; }    
    }

    public class ViewHandler
    {
        private readonly ISpecificationService _specificationService;

        public ViewHandler(ISpecificationService specificationService)
        {
            _specificationService = specificationService;
        }

        public HandlerResponse Execute()
        {
            return new HandlerResponse {
                Specification = _specificationService.Generate() };
        }
    }
}