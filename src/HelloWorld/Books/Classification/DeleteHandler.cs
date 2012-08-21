using System;
using HelloWorld.Infrastructure;

namespace HelloWorld.Books.Classification
{
    public class DeleteRequest
    {
        public Guid Id { get; set; }
    }

    public class DeleteHandler
    {
        private readonly IRepository<Category> _categories;

        public DeleteHandler(IRepository<Category> categories)
        {
            _categories = categories;
        }

        public void Execute_Id(DeleteRequest request)
        {
            _categories.Delete(request.Id);
        }
    }
}