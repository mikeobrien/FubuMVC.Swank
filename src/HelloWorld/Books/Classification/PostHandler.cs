namespace HelloWorld.Books.Classification
{
    public class PostHandler
    {
        private readonly IRepository<Category> _categories;

        public PostHandler(IRepository<Category> categories)
        {
            _categories = categories;
        }

        public Category Execute(Category request)
        {
            return _categories.Add(request);
        }
    }
}