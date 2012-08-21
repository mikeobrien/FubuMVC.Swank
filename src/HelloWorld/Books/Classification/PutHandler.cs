using HelloWorld.Infrastructure;

namespace HelloWorld.Books.Classification
{
    public class PutHandler
    {
        private readonly IRepository<Category> _categories;

        public PutHandler(IRepository<Category> categories)
        {
            _categories = categories;
        }

        public void Execute_Id(Category request)
        {
            _categories.Add(request);
        }
    }
}