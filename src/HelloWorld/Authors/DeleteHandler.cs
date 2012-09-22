using System;

namespace HelloWorld.Authors
{
    public class DeleteRequest
    {
        public Guid Id { get; set; }
    }

    public class DeleteHandler
    {
        private readonly IRepository<Author> _authors;

        public DeleteHandler(IRepository<Author> authors)
        {
            _authors = authors;
        }

        public void Execute_Id(DeleteRequest request)
        {
            _authors.Delete(request.Id);
        }
    }
}