using HelloWorld.Infrastructure;

namespace HelloWorld.Books
{
    public class CommentPutHandler
    {
        private readonly IRepository<Comment> _categories;

        public CommentPutHandler(IRepository<Comment> categories)
        {
            _categories = categories;
        }

        public void Execute_Comments_Id(Comment request)
        {
            _categories.Add(request);
        }
    }
}