using System;
using HelloWorld.Infrastructure;

namespace HelloWorld.Books
{
    public class DeleteRequest
    {
        public Guid Id { get; set; }
    }

    public class DeleteHandler
    {
        private readonly IRepository<Book> _books;

        public DeleteHandler(IRepository<Book> books)
        {
            _books = books;
        }

        public void Execute_Id(DeleteRequest request)
        {
            _books.Delete(request.Id);
        }
    }
}