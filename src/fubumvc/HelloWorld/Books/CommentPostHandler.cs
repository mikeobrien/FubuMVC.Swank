using HelloWorld.Infrastructure;

namespace HelloWorld.Books
{
    public class CommentPostHandler
    {
        private readonly IRepository<Comment> _categories;

        public CommentPostHandler(IRepository<Comment> categories)
        {
            _categories = categories;
        }

        public Comment Execute_BookId_Comments(Comment request)
        {
            return _categories.Add(request);
        }
    }
}