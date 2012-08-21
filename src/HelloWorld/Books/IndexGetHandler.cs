using System.Collections.Generic;
using System.Linq;
using HelloWorld.Infrastructure;

namespace HelloWorld.Books
{
    public class IndexGetHandler
    {
        private readonly IRepository<Book> _books;

        public IndexGetHandler(IRepository<Book> books)
        {
            _books = books;
        }

        public List<Book> Execute()
        {
            return _books.ToList();
        }
    }
}