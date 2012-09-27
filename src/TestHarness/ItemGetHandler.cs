using System;

namespace TestHarness
{
    public class GetItemHandlerRequest
    {
        public Guid Id { get; set; }
    }

    public class GetItemHandlerResponse
    {
        public string Message { get; set; }
    }

    public class ItemGetHandler
    {
        public GetItemHandlerResponse Execute_Id(GetItemHandlerRequest request)
        {
            return new GetItemHandlerResponse { Message = "oh hai" };
        }
    }
}