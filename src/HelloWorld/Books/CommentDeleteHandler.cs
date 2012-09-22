using System;

namespace HelloWorld.Books
{
    public class CommentDeleteRequest
    {
        public Guid Id { get; set; }
    }

    public class CommentDeleteHandler
    {
        private readonly IRepository<Comment> _comments;

        public CommentDeleteHandler(IRepository<Comment> comments)
        {
            _comments = comments;
        }

        public void Execute_Comments_Id(CommentDeleteRequest request)
        {
            _comments.Delete(request.Id);
        }
    }
}