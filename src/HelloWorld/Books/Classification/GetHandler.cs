using System;
using HelloWorld.Infrastructure;

namespace HelloWorld.Books.Classification
{
    public class GetRequest
    {
        public Guid Id { get; set; }
    }

    public class GetHandler
    {
        private readonly IRepository<Category> _categories;

        public GetHandler(IRepository<Category> categories)
        {
            _categories = categories;
        }

        public Category Execute_Id(GetRequest request)
        {
            return _categories.Get(request.Id);
        }
    }
}