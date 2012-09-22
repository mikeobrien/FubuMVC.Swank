namespace HelloWorld.Authors
{
    public class PutHandler
    {
        private readonly IRepository<Author> _authors;

        public PutHandler(IRepository<Author> authors)
        {
            _authors = authors;
        }

        public void Execute_Id(Author request)
        {
            _authors.Add(request);
        }
    }
}