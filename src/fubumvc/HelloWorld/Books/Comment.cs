using System;

namespace HelloWorld.Books
{
    public class Comment
    {
        public Guid Id { get; set; }
        public Guid BookId { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
    }
}