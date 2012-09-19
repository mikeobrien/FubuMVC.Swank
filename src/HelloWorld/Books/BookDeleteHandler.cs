using System;
using HelloWorld.Infrastructure;

namespace HelloWorld.Books
{
    public class BookDeleteRequest
    {
        public Guid Id { get; set; }
    }

    public class BookDeleteHandler
    {
        private readonly IRepository<Book> _books;

        public BookDeleteHandler(IRepository<Book> books)
        {
            _books = books;
        }

        public void Execute_Id(BookDeleteRequest request)
        {
            _books.Delete(request.Id);
        }
    }
}