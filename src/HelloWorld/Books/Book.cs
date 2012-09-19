using System;

namespace HelloWorld.Books
{
    public class Book
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string Isbn { get; set; }
        public int Published { get; set; }
    }
}