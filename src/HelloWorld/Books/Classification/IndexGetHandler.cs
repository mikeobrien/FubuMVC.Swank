using System.Collections.Generic;
using System.Linq;

namespace HelloWorld.Books.Classification
{
    public class IndexGetHandler
    {
        private readonly IRepository<Category> _categories;

        public IndexGetHandler(IRepository<Category> categories)
        {
            _categories = categories;
        }

        public List<Category> Execute()
        {
            return _categories.ToList();
        }
    }
}