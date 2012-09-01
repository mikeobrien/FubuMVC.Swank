using HelloWorld.Infrastructure;

namespace HelloWorld.Books
{
    public class BookPostHandler
    {
        private readonly IRepository<Book> _books;

        public BookPostHandler(IRepository<Book> books)
        {
            _books = books;
        }

        public Book Execute(Book request)
        {
            return _books.Add(request);
        }
    }
}