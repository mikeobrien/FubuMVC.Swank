using System;
using System.Collections.Generic;
using System.Linq;

namespace HelloWorld.Books
{
    public class CommentsGetRequest
    {
        public Guid BookId { get; set; }    
    }

    public class CommentsGetHandler
    {
        private readonly IRepository<Comment> _comments;

        public CommentsGetHandler(IRepository<Comment> comments)
        {
            _comments = comments;
        }

        public List<Comment> Execute_BookId_Comments(CommentsGetRequest request)
        {
            return _comments.Where(x => x.BookId == request.BookId).ToList();
        }
    }
}