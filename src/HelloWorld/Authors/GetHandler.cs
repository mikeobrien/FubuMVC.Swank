using System;
using HelloWorld.Infrastructure;

namespace HelloWorld.Authors
{
    public class GetRequest
    {
        public Guid Id { get; set; }
    }

    public class GetHandler
    {
        private readonly IRepository<Author> _authors;

        public GetHandler(IRepository<Author> authors)
        {
            _authors = authors;
        }

        public Author Execute_Id(GetRequest request)
        {
            return _authors.Get(request.Id);
        }
    }
}