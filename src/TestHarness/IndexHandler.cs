namespace TestHarness
{
    public class IndexResponse
    {
        public string Message { get; set; }
    }

    public class IndexHandler
    {
        public IndexResponse ExecuteGet()
        {
            return new IndexResponse { Message = "oh hai" };
        }
    }
}