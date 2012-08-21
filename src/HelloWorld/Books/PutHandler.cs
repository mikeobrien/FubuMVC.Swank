using HelloWorld.Infrastructure;

namespace HelloWorld.Books
{
    public class PutHandler
    {
        private readonly IRepository<Book> _books;

        public PutHandler(IRepository<Book> books)
        {
            _books = books;
        }

        public void Execute_Id(Book request)
        {
            _books.Add(request);
        }
    }
}