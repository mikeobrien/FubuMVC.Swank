namespace HelloWorld.Authors
{
    public class PostHandler
    {
        private readonly IRepository<Author> _authors;

        public PostHandler(IRepository<Author> authors)
        {
            _authors = authors;
        }

        public Author Execute(Author request)
        {
            return _authors.Add(request);
        }
    }
}