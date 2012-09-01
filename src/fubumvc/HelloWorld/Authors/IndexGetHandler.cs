using System.Collections.Generic;
using System.Linq;
using HelloWorld.Infrastructure;

namespace HelloWorld.Authors
{
    public class IndexGetHandler
    {
        private readonly IRepository<Author> _authors;

        public IndexGetHandler(IRepository<Author> authors)
        {
            _authors = authors;
        }

        public List<Author> Execute()
        {
            return _authors.ToList();
        }
    }
}