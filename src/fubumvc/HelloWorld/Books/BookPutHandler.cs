using HelloWorld.Infrastructure;

namespace HelloWorld.Books
{
    public class BookPutHandler
    {
        private readonly IRepository<Book> _books;

        public BookPutHandler(IRepository<Book> books)
        {
            _books = books;
        }

        public void Execute_Id(Book request)
        {
            _books.Add(request);
        }
    }
}