using System;
using HelloWorld.Infrastructure;

namespace HelloWorld.Books
{
    public class GetRequest
    {
        public Guid Id { get; set; }
    }

    public class GetHandler
    {
        private readonly IRepository<Book> _books;

        public GetHandler(IRepository<Book> books)
        {
            _books = books;
        }

        public Book Execute_Id(GetRequest request)
        {
            return _books.Get(request.Id);
        }
    }
}