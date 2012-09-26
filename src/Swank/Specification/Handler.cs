namespace FubuMVC.Swank.Specification
{
    public class HandlerResponse
    {
        public Specification Specification { get; set; }    
    }

    // TODO: when the public fubu nugets *finally* catch up this 
    // can be simplified down to one execute method and the 
    // HandlerResponse class can go.

    public class Handler
    {
        private readonly ISpecificationService _specificationService;
        private static Specification _specification;

        public Handler(ISpecificationService specificationService)
        {
            _specificationService = specificationService;
        }

        public HandlerResponse Execute()
        {
            return new HandlerResponse {
                Specification = GetSpecification()
            };
        }

        public Specification DataExecute()
        {
            return GetSpecification();
        }

        private Specification GetSpecification()
        {
            return _specification ?? (_specification = _specificationService.Generate());
        }
    }
}