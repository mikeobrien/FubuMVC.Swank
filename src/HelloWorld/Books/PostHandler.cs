using HelloWorld.Infrastructure;

namespace HelloWorld.Books
{
    public class PostHandler
    {
        private readonly IRepository<Book> _books;

        public PostHandler(IRepository<Book> books)
        {
            _books = books;
        }

        public Book Execute(Book request)
        {
            return _books.Add(request);
        }
    }
}