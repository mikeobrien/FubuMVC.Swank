using System;
using HelloWorld.Infrastructure;

namespace HelloWorld.Books
{
    public class BookGetRequest
    {
        public Guid Id { get; set; }
    }

    public class BookGetHandler
    {
        private readonly IRepository<Book> _books;

        public BookGetHandler(IRepository<Book> books)
        {
            _books = books;
        }

        public Book Execute_Id(BookGetRequest request)
        {
            return _books.Get(request.Id);
        }
    }
}