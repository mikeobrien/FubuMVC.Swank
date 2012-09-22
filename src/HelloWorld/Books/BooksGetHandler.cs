using System.Collections.Generic;
using System.Linq;

namespace HelloWorld.Books
{
    public class BooksGetHandler
    {
        private readonly IRepository<Book> _books;

        public BooksGetHandler(IRepository<Book> books)
        {
            _books = books;
        }

        public List<Book> Execute()
        {
            return _books.ToList();
        }
    }
}