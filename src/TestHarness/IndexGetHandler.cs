namespace TestHarness
{
    public class IndexResponse
    {
        public string Message { get; set; }
    }

    public class IndexGetHandler
    {
        public IndexResponse Execute()
        {
            return new IndexResponse { Message = "oh hai" };
        }
    }
}