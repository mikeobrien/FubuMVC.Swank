using System;
using HelloWorld.Infrastructure;

namespace HelloWorld.Books
{
    public class CommentGetRequest
    {
        public Guid Id { get; set; }
    }

    public class CommentGetHandler
    {
        private readonly IRepository<Comment> _comments;

        public CommentGetHandler(IRepository<Comment> comments)
        {
            _comments = comments;
        }

        public Comment Execute_Comments_Id(CommentGetRequest request)
        {
            return _comments.Get(request.Id);
        }
    }
}